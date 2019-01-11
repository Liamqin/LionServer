using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.ComponentModel;
using CYQ.Data.SQL;
using CYQ.Data.Cache;
using CYQ.Data.Table;

using CYQ.Data.Aop;
using CYQ.Data.Tool;
using CYQ.Data.UI;


namespace CYQ.Data
{
    /// <summary>
    /// Manipulate��table / view / custom statement
    ///<para>��������/��ͼ/�Զ������</para>
    /// </summary>
    public partial class MAction : IDisposable
    {
        #region ȫ�ֱ���

        internal DbBase dalHelper;//���ݲ���
        private SqlCreate _sqlCreate;
        private InsertOp _option = InsertOp.ID;
        private NoSqlAction _noSqlAction = null;
        private MDataRow _Data;//��ʾһ��
        /// <summary>
        /// Archive the rows of the data structure
        /// <para>�浵���ݽṹ����</para>
        /// </summary>
        public MDataRow Data
        {
            get
            {
                return _Data;
            }
        }
        /// <summary>
        /// ԭʼ�������ı���/��ͼ����δ����[�����ݿ�ת������]��ʽ����
        /// </summary>
        private string _sourceTableName;
        private string _TableName; //����
        /// <summary>
        /// Table name (if the view statement is the operation, the final view of the name)
        ///<para>��ǰ�����ı���(������������ͼ��䣬��Ϊ������ͼ����</para> ��
        /// </summary>
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;
            }
        }
        /// <summary>
        /// The database connection string
        ///<para>���ݿ������ַ���</para> 
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (dalHelper != null)
                {
                    return dalHelper.conn;
                }
                return string.Empty;
            }
        }
        private string _debugInfo = string.Empty;
        /// <summary>
        /// Get or set debugging information [need to set App Config.Debug.Open DebugInfo to ture]
        ///<para>��ȡ�����õ�����Ϣ[��Ҫ����AppConfig.Debug.OpenDebugInfoΪture]</para>
        /// </summary>
        public string DebugInfo
        {
            get
            {
                if (dalHelper != null)
                {
                    return dalHelper.debugInfo.ToString();
                }
                return _debugInfo;
            }
            set
            {
                if (dalHelper != null)
                {
                    dalHelper.debugInfo.Length = 0;
                    dalHelper.debugInfo.Append(value);
                }
            }
        }
        /// <summary>
        /// The database type
        /// <para>���ݿ�����</para>
        /// </summary>
        public DalType DalType
        {
            get
            {
                return dalHelper.dalType;
            }
        }
        /// <summary>
        /// The database name
        /// <para>���ݿ�����</para>
        /// </summary>
        public string DataBase
        {
            get
            {
                return dalHelper.DataBase;
            }
        }
        /// <summary>
        /// The database version
        /// ���ݿ�İ汾��
        /// </summary>
        public string DalVersion
        {
            get
            {
                return dalHelper.Version;
            }
        }
        /// <summary>
        /// The number of rows affected when executing a SQL command (-2 is an exception)
        /// <para>ִ��SQL����ʱ��Ӱ���������-2��Ϊ�쳣����</para>
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return dalHelper.recordsAffected;
            }
        }
        /// <summary>
        /// Command Timeout[seconds]
        ///<para>���ʱ����[��λ��]</para>
        /// </summary>
        public int TimeOut
        {
            get
            {
                if (dalHelper.Com != null)
                {
                    return dalHelper.Com.CommandTimeout;
                }
                return -1;
            }
            set
            {
                if (dalHelper.Com != null)
                {
                    dalHelper.Com.CommandTimeout = value;
                }
            }
        }
        private bool _AllowInsertID = false;
        /// <summary>
        /// Whether to allow manual insertion of IDs for self-incrementing primary key identification
        ///<para>����������ʶ�ģ��Ƿ������ֶ�����ID</para> 
        /// </summary>
        public bool AllowInsertID
        {
            get
            {
                return _AllowInsertID;
            }
            set
            {
                _AllowInsertID = value;
            }
        }
        private bool _isInsertCommand; //�Ƿ�ִ�в�������(������Update����)

        private bool _setIdentityResult = true;
        /// <summary>
        /// MSSQL�����ʶIDʱ������ѡ��[������ʱ��Ч]
        /// </summary>
        internal void SetIdentityInsertOn()
        {
            _setIdentityResult = true;
            if (dalHelper != null && dalHelper.isOpenTrans)
            {
                switch (dalHelper.dalType)
                {
                    case DalType.MsSql:
                    case DalType.Sybase:
                        if (_Data.Columns.FirstPrimary.IsAutoIncrement)//������
                        {
                            try
                            {
                                string lastTable = Convert.ToString(CacheManage.LocalInstance.Get("MAction_IdentityInsertForSql"));
                                if (!string.IsNullOrEmpty(lastTable))
                                {
                                    lastTable = "set identity_insert " + SqlFormat.Keyword(lastTable, dalHelper.dalType) + " off";
                                    dalHelper.ExeNonQuery(lastTable, false);
                                }
                                _setIdentityResult = dalHelper.ExeNonQuery("set identity_insert " + SqlFormat.Keyword(_TableName, dalHelper.dalType) + " on", false) > -2;
                                if (_setIdentityResult)
                                {
                                    CacheManage.LocalInstance.Set("MAction_IdentityInsertForSql", _TableName, 30);
                                }
                            }
                            catch
                            {
                            }
                        }
                        break;
                }

            }
            _AllowInsertID = true;
        }
        /// <summary>
        /// MSSQL�����ʶID��رմ�ѡ��[������ʱ��Ч]
        /// </summary>;
        internal void SetIdentityInsertOff()
        {
            if (_setIdentityResult && dalHelper != null && dalHelper.isOpenTrans)
            {
                switch (dalHelper.dalType)
                {
                    case DalType.MsSql:
                    case DalType.Sybase:
                        if (_Data.Columns.FirstPrimary.IsAutoIncrement)//������
                        {
                            try
                            {
                                if (dalHelper.ExeNonQuery("set identity_insert " + SqlFormat.Keyword(_TableName, DalType.MsSql) + " off", false) > -2)
                                {
                                    _setIdentityResult = false;
                                    CacheManage.LocalInstance.Remove("MAction_IdentityInsertForSql");
                                }
                            }
                            catch
                            {
                            }
                        }
                        break;
                }

            }
            _AllowInsertID = false;
        }
        /// <summary>
        /// ��DeleteField�����ú�ɾ��ת���²��������������ɾ���������ɽ���������Ϊtrue��
        /// </summary>
        internal bool IsIgnoreDeleteField = false;
        #endregion

        #region ���캯��

        /// <summary>
        /// Instantiation
        /// <para>ʵ����</para>
        /// </summary>
        /// <param name="tableNamesEnum">Parameters: table name, view, custom statement, DataRow
        /// <para>��������������ͼ���Զ�����䡢MDataRow</para></param>
        public MAction(object tableNamesEnum)
        {
            Init(tableNamesEnum, AppConfig.DB.DefaultConn);
        }

        /// <param name="conn">Database connection statement or configuration key
        /// <para>���ݿ�������������Key</para></param>
        public MAction(object tableNamesEnum, string conn)
        {
            Init(tableNamesEnum, conn);
        }
        #endregion

        #region ��ʼ��

        private void Init(object tableObj, string conn)
        {
            tableObj = SqlCreate.SqlToViewSql(tableObj);
            string dbName = StaticTool.GetDbName(ref tableObj);
            if (conn == AppConfig.DB.DefaultConn && !string.IsNullOrEmpty(dbName))
            {
                if (dbName.ToLower().EndsWith("conn") && !string.IsNullOrEmpty(AppConfig.GetConn(dbName)))
                {
                    conn = dbName;
                }
                else
                {
                    conn = dbName + "Conn";
                }
            }
            MDataRow newRow;
            InitConn(tableObj, conn, out newRow);//���Դ�MDataRow��ȡ�µ�Conn����
            InitSqlHelper(newRow, dbName);
            InitRowSchema(newRow, true);
            InitGlobalObject(true);
            //Aop.IAop myAop = Aop.InterAop.Instance.GetFromConfig();//��ͼ�������ļ������Զ���Aop
            //if (myAop != null)
            //{
            //    SetAop(myAop);
            //}
        }
        private void InitConn(object tableObj, string conn, out MDataRow newRow)
        {
            if (tableObj is MDataRow)
            {
                newRow = tableObj as MDataRow;
            }
            else
            {
                newRow = new MDataRow();
                newRow.TableName = SqlFormat.NotKeyword(tableObj.ToString());
            }
            if (!string.IsNullOrEmpty(conn))
            {
                newRow.Conn = conn;
            }
            else if (_Data != null)//�¼ӵĴ���
            {
                newRow.Conn = _Data.Conn;
            }
            _sourceTableName = newRow.TableName;
        }
        private void InitSqlHelper(MDataRow newRow, string newDbName)
        {
            if (dalHelper == null)// || newCreate
            {
                dalHelper = DalCreate.CreateDal(newRow.Conn);
                //���������¼���
                if (dalHelper.IsOnExceptionEventNull)
                {
                    dalHelper.OnExceptionEvent += new DbBase.OnException(_DataSqlHelper_OnExceptionEvent);
                }
            }
            else
            {
                dalHelper.ClearParameters();//oracle 11g(ĳ�û��ĵ����ϻ�����⣬�л������������δ�壩
            }
            if (!string.IsNullOrEmpty(newDbName))//��Ҫ�л����ݿ⡣
            {
                if (string.Compare(dalHelper.DataBase, newDbName, StringComparison.OrdinalIgnoreCase) != 0)//���ݿ����Ʋ���ͬ��
                {
                    if (newRow.TableName.Contains(" "))//��ͼ��䣬��ֱ���л����ݿ����ӡ�
                    {
                        dalHelper.ChangeDatabase(newDbName);
                    }
                    else
                    {
                        bool isWithDbName = newRow.TableName.Contains(".");//�Ƿ�DBName.TableName
                        string fullTableName = isWithDbName ? newRow.TableName : newDbName + "." + newRow.TableName;
                        string sourceDbName = dalHelper.DataBase;
                        DbResetResult result = dalHelper.ChangeDatabaseWithCheck(fullTableName);
                        switch (result)
                        {
                            case DbResetResult.Yes://���ݿ��л��� (����Ҫǰ׺��
                            case DbResetResult.No_SaveDbName:
                            case DbResetResult.No_DBNoExists:
                                if (isWithDbName) //����ǰ׺�ģ�ȡ��ǰ׺
                                {
                                    _sourceTableName = newRow.TableName = SqlFormat.NotKeyword(fullTableName);
                                }
                                break;
                            case DbResetResult.No_Transationing:
                                if (!isWithDbName)//�����ͬ�����ݿ⣬��Ҫ�������ݿ�ǰ׺
                                {
                                    _sourceTableName = newRow.TableName = fullTableName;
                                }
                                break;
                        }
                    }
                }
            }
        }
        void _DataSqlHelper_OnExceptionEvent(string msg)
        {
            _aop.OnError(msg);
        }
        private static DateTime lastGCTime = DateTime.Now;
        private void InitRowSchema(MDataRow row, bool resetState)
        {
            _Data = row;
            _TableName = SqlCompatible.Format(_sourceTableName, dalHelper.dalType);
            _TableName = DBTool.GetMapTableName(dalHelper.useConnBean.ConfigName, _TableName);//�������ݿ�ӳ����ݡ�
            if (_Data.Count == 0)
            {
                if (!TableSchema.FillTableSchema(ref _Data, ref dalHelper, _TableName, _sourceTableName))
                {
                    if (!dalHelper.TestConn(AllowConnLevel.MaterBackupSlave))
                    {
                        Error.Throw(dalHelper.dalType + "." + dalHelper.DataBase + ":open database failed! check the connectionstring is be ok!" + AppConst.NewLine + "error:" + dalHelper.debugInfo.ToString());
                    }
                    Error.Throw(dalHelper.dalType + "." + dalHelper.DataBase + ":check the tablename  \"" + _TableName + "\" is exist?" + AppConst.NewLine + "error:" + dalHelper.debugInfo.ToString());
                }
            }
            else if (resetState)
            {
                _Data.SetState(0);
            }
            _Data.Conn = row.Conn;//FillTableSchema��ı�_Row�Ķ���
        }


        /// <summary>
        /// Toggle Table Action: To switch between other tables, use this method
        /// <para>�л���������������������ͨ���˷����л�</para>
        /// </summary>
        /// <param name="tableObj">Parameters: table name, view, custom statement, DataRow
        /// <para>��������������ͼ���Զ�����䡢MDataRow</para></param>
        public void ResetTable(object tableObj)
        {
            ResetTable(tableObj, true, null);
        }

        /// <param name="resetState">Reset Row State (defaultValue:true)
        /// <para>�Ƿ�����ԭ�е�����״̬��Ĭ��true)</para></param>
        public void ResetTable(object tableObj, bool resetState)
        {
            ResetTable(tableObj, resetState, null);
        }
        /// <param name="newDbName">Other DataBaseName
        /// <para>�������ݿ�����</para></param>
        public void ResetTable(object tableObj, bool resetState, string newDbName)
        {
            tableObj = SqlCreate.SqlToViewSql(tableObj);
            newDbName = newDbName ?? StaticTool.GetDbName(ref tableObj);
            MDataRow newRow;
            InitConn(tableObj, string.Empty, out newRow);

            //newRow.Conn = newDbName;//����ָ�����ӣ������л����ݿ�
            InitSqlHelper(newRow, newDbName);
            InitRowSchema(newRow, resetState);
            InitGlobalObject(false);
        }

        private void InitGlobalObject(bool allowCreate)
        {
            if (_Data != null)
            {
                if (_sqlCreate == null)
                {
                    _sqlCreate = new SqlCreate(this);
                }
                if (_UI != null)
                {
                    _UI._Data = _Data;
                }
                else if (allowCreate)
                {
                    _UI = new MActionUI(ref _Data, dalHelper, _sqlCreate);
                }
                if (_noSqlAction != null)
                {
                    _noSqlAction.Reset(ref _Data, _TableName, dalHelper.Con.DataSource, dalHelper.dalType);
                }
                else if (allowCreate)
                {
                    switch (dalHelper.dalType)
                    {
                        case DalType.Txt:
                        case DalType.Xml:
                            _noSqlAction = new NoSqlAction(ref _Data, _TableName, dalHelper.Con.DataSource, dalHelper.dalType);
                            break;
                    }
                }
            }
        }

        #endregion

        #region ���ݿ��������

        private bool InsertOrUpdate(string sqlCommandText)
        {
            bool returnResult = false;
            if (_sqlCreate.isCanDo)
            {
                #region ִ��Insert��Update����
                if (_isInsertCommand) //����
                {
                    _isInsertCommand = false;
                    object ID;
                    switch (dalHelper.dalType)
                    {
                        case DalType.MsSql:
                        case DalType.Sybase:
                            ID = dalHelper.ExeScalar(sqlCommandText, false);
                            if (ID == null && AllowInsertID && dalHelper.recordsAffected > -2)
                            {
                                ID = _Data.PrimaryCell.Value;
                            }
                            break;
                        default:
                            #region MyRegion
                            bool isTrans = dalHelper.isOpenTrans;
                            int groupID = DataType.GetGroup(_Data.PrimaryCell.Struct.SqlType);
                            bool isNum = groupID == 1 && _Data.PrimaryCell.Struct.Scale <= 0;
                            if (!isTrans && (isNum || _Data.PrimaryCell.Struct.IsAutoIncrement) && (!AllowInsertID || _Data.PrimaryCell.IsNullOrEmpty)) // ����������
                            {
                                dalHelper.isOpenTrans = true;//��������
                                dalHelper.TranLevel = IsolationLevel.ReadCommitted;//Ĭ�����񼶱������������������һ�£������ⲿ�����Դ˵�Ӱ�졣
                            }
                            ID = dalHelper.ExeNonQuery(sqlCommandText, false);//���ص�����Ӱ�������
                            if (_option != InsertOp.None && ID != null && Convert.ToInt32(ID) > 0)
                            {
                                if (AllowInsertID && !_Data.PrimaryCell.IsNullOrEmpty)//�ֹ���ID
                                {
                                    ID = _Data.PrimaryCell.Value;
                                }
                                else if (isNum)
                                {
                                    ClearParameters();
                                    ID = dalHelper.ExeScalar(_sqlCreate.GetMaxID(), false);
                                }
                                else
                                {
                                    ID = null;
                                    returnResult = true;
                                }

                            }
                            if (!isTrans)
                            {
                                dalHelper.EndTransaction();
                            }
                            #endregion
                            break;
                    }
                    if ((ID != null && Convert.ToString(ID) != "-2") || (dalHelper.recordsAffected > -2 && _option == InsertOp.None))
                    {
                        if (_option != InsertOp.None)
                        {
                            _Data.PrimaryCell.Value = ID;
                        }
                        returnResult = (_option == InsertOp.Fill) ? Fill(ID) : true;
                    }
                }
                else //����
                {
                    returnResult = dalHelper.ExeNonQuery(sqlCommandText, false) > 0;
                }
                #endregion
            }
            else if (!_isInsertCommand && _Data.GetState() == 1 && dalHelper.recordsAffected != -2) // ���²�����
            {
                //���������Ϣ
                return true;
            }
            else if (dalHelper.isOpenTrans && dalHelper.recordsAffected == -2) // �������У���ع�
            {
                dalHelper.WriteError(_isInsertCommand ? "Insert" : "Update" + "():");
            }
            return returnResult;
        }

        #region ����

        /// <summary>
        /// Insert To DataBase
        ///  <para>��������</para>
        /// </summary>
        public bool Insert()
        {
            return Insert(false, _option);
        }

        /// <param name="option">InsertOp
        /// <para>����ѡ��</para></param>
        public bool Insert(InsertOp option)
        {
            return Insert(false, option);
        }


        /// <param name="autoSetValue">Automatic get values from context
        /// <para>�Զ�ȡֵ���������Ļ����У�</para></param>
        public bool Insert(bool autoSetValue)
        {
            return Insert(autoSetValue, _option);
        }

        /// <param name="option">InsertOp
        /// <para>����ѡ��</para></param>
        public bool Insert(bool autoSetValue, InsertOp option)
        {
            if (CheckDisposed()) { return false; }
            if (autoSetValue)
            {
                _UI.GetAll(!AllowInsertID);//�������IDʱ��Ҳ��Ҫ��ȡ������
            }
            AopResult aopResult = AopResult.Default;
            if (_aop.IsLoadAop)
            {
                _aop.Para.MAction = this;
                _aop.Para.TableName = _sourceTableName;
                _aop.Para.Row = _Data;
                _aop.Para.AutoSetValue = autoSetValue;
                _aop.Para.InsertOp = option;
                _aop.Para.IsTransaction = dalHelper.isOpenTrans;
                aopResult = _aop.Begin(Aop.AopEnum.Insert);
            }
            if (aopResult == AopResult.Default || aopResult == AopResult.Continue)
            {
                switch (dalHelper.dalType)
                {
                    case DalType.Txt:
                    case DalType.Xml:
                        _aop.Para.IsSuccess = _noSqlAction.Insert(dalHelper.isOpenTrans);
                        dalHelper.recordsAffected = _aop.Para.IsSuccess ? 1 : 0;
                        break;
                    default:
                        ClearParameters();
                        string sql = _sqlCreate.GetInsertSql();
                        _isInsertCommand = true;
                        _option = option;
                        _aop.Para.IsSuccess = InsertOrUpdate(sql);
                        break;
                }
            }
            else if (option != InsertOp.None)
            {
                _Data = _aop.Para.Row;
                InitGlobalObject(false);
            }
            if (_aop.IsLoadAop && (aopResult == AopResult.Break || aopResult == AopResult.Continue))
            {
                _aop.End(Aop.AopEnum.Insert);
            }
            if (dalHelper.recordsAffected == -2)
            {
                OnError();
            }
            return _aop.Para.IsSuccess;
        }
        #endregion

        #region ����
        /// <summary>
        /// Update to database [Will automatically try to fetch from the UI when conditions are not passed]
        /// <para>��������[����where����ʱ���Զ����Դ�UI��ȡ]</para>
        /// </summary>
        public bool Update()
        {
            return Update(null, true);
        }


        /// <param name="where">Sql statement where the conditions: 88, "id = 88"
        /// <para>sql����where������88��"id=88"</para></param>
        public bool Update(object where)
        {
            return Update(where, false);
        }


        /// <param name="autoSetValue">Automatic get values from context
        /// <para>�Զ�ȡֵ���������Ļ����У�</para></param>
        public bool Update(bool autoSetValue)
        {
            return Update(null, autoSetValue);
        }

        /// <param name="autoSetValue">Automatic get values from context
        /// <para>�Զ�ȡֵ���������Ļ����У�</para></param>
        public bool Update(object where, bool autoSetValue)
        {
            if (CheckDisposed()) { return false; }
            if (autoSetValue)
            {
                _UI.GetAll(false);
            }
            if (where == null || Convert.ToString(where) == "")
            {
                where = _sqlCreate.GetPrimaryWhere();
            }
            AopResult aopResult = AopResult.Default;
            if (_aop.IsLoadAop)
            {
                _aop.Para.MAction = this;
                _aop.Para.TableName = _sourceTableName;
                _aop.Para.Row = _Data;
                _aop.Para.Where = where;
                //_aop.Para.AopPara = aopPara;
                _aop.Para.AutoSetValue = autoSetValue;
                _aop.Para.UpdateExpression = _sqlCreate.updateExpression;
                _aop.Para.IsTransaction = dalHelper.isOpenTrans;
                aopResult = _aop.Begin(Aop.AopEnum.Update);
            }
            if (aopResult == AopResult.Default || aopResult == AopResult.Continue)
            {
                switch (dalHelper.dalType)
                {
                    case DalType.Txt:
                    case DalType.Xml:
                        int count;
                        _aop.Para.IsSuccess = _noSqlAction.Update(_sqlCreate.FormatWhere(where), out count);
                        dalHelper.recordsAffected = count;
                        break;
                    default:
                        ClearParameters();
                        string sql = _sqlCreate.GetUpdateSql(where);
                        _aop.Para.IsSuccess = InsertOrUpdate(sql);
                        break;
                }
            }
            if (_aop.IsLoadAop && (aopResult == AopResult.Break || aopResult == AopResult.Continue))
            {
                _aop.End(Aop.AopEnum.Update);
            }
            if (dalHelper.recordsAffected == -2)
            {
                OnError();
            }
            return _aop.Para.IsSuccess;
        }
        #endregion

        /// <summary>
        /// Delete from database [Will automatically try to fetch from the UI when conditions are not passed]
        /// <para>ɾ������[����where����ʱ���Զ����Դ�UI��ȡ]</para>
        /// </summary>
        public bool Delete()
        {
            return Delete(null);
        }

        /// <param name="where">Sql statement where the conditions: 88, "id = 88"
        /// <para>sql����where������88��"id=88"</para></param>
        public bool Delete(object where)
        {
            if (CheckDisposed()) { return false; }
            if (where == null || Convert.ToString(where) == "")
            {
                _UI.PrimayAutoGetValue();
                where = _sqlCreate.GetPrimaryWhere();
            }
            AopResult aopResult = AopResult.Default;
            if (_aop.IsLoadAop)
            {
                _aop.Para.MAction = this;
                _aop.Para.TableName = _sourceTableName;
                _aop.Para.Row = _Data;
                _aop.Para.Where = where;
                //_aop.Para.AopPara = aopPara;
                _aop.Para.IsTransaction = dalHelper.isOpenTrans;
                aopResult = _aop.Begin(Aop.AopEnum.Delete);
            }
            if (aopResult == AopResult.Default || aopResult == AopResult.Continue)
            {
                string deleteField = AppConfig.DB.DeleteField;
                bool isToUpdate = !IsIgnoreDeleteField && !string.IsNullOrEmpty(deleteField) && _Data.Columns.Contains(deleteField);
                switch (dalHelper.dalType)
                {
                    case DalType.Txt:
                    case DalType.Xml:
                        string sqlWhere = _sqlCreate.FormatWhere(where);
                        int count;
                        if (isToUpdate)
                        {
                            _Data.Set(deleteField, true);
                            _aop.Para.IsSuccess = _noSqlAction.Update(sqlWhere, out count);
                        }
                        else
                        {
                            _aop.Para.IsSuccess = _noSqlAction.Delete(sqlWhere, out count);
                        }
                        dalHelper.recordsAffected = count;
                        break;
                    default:
                        ClearParameters();
                        string sql = isToUpdate ? _sqlCreate.GetDeleteToUpdateSql(where) : _sqlCreate.GetDeleteSql(where);
                        _aop.Para.IsSuccess = dalHelper.ExeNonQuery(sql, false) > 0;
                        break;
                }
            }
            if (_aop.IsLoadAop && (aopResult == AopResult.Break || aopResult == AopResult.Continue))
            {
                _aop.End(Aop.AopEnum.Delete);
            }
            if (dalHelper.recordsAffected == -2)
            {
                OnError();
            }
            return _aop.Para.IsSuccess;
        }

        /// <summary>
        /// select all data
        ///<para>ѡ����������</para>
        /// </summary>
        public MDataTable Select()
        {
            int count;
            return Select(0, 0, null, out count);
        }

        /// <param name="where">Sql statement where the conditions: 88, "id = 88"
        /// <para>sql����where������88��"id=88"</para></param>
        public MDataTable Select(object where)
        {
            int count;
            return Select(0, 0, where, out count);
        }
        public MDataTable Select(int topN, object where)
        {
            int count;
            return Select(0, topN, where, out count);
        }
        /// <param name="pageIndex">pageIndex<para>�ڼ�ҳ</para></param>
        /// <param name="pageSize">pageSize<para>ÿҳ����[Ϊ0ʱĬ��ѡ������]</para></param>
        public MDataTable Select(int pageIndex, int pageSize)
        {
            int count;
            return Select(pageIndex, pageSize, null, out count);
        }
        public MDataTable Select(int pageIndex, int pageSize, object where)
        {
            int count;
            return Select(pageIndex, pageSize, where, out count);
        }

        /// <param name="rowCount">The total number of records returned
        /// <para>���صļ�¼����</para></param>
        public MDataTable Select(int pageIndex, int pageSize, object where, out int rowCount)
        {
            if (CheckDisposed()) { rowCount = -1; return new MDataTable(_TableName); }
            rowCount = 0;
            AopResult aopResult = AopResult.Default;
            if (_aop.IsLoadAop)
            {
                _aop.Para.MAction = this;
                _aop.Para.PageIndex = pageIndex;
                _aop.Para.PageSize = pageSize;
                _aop.Para.TableName = _sourceTableName;
                _aop.Para.Row = _Data;
                _aop.Para.Where = where;
                _aop.Para.SelectColumns = _sqlCreate.selectColumns;
                //_aop.Para.AopPara = aopPara;
                _aop.Para.IsTransaction = dalHelper.isOpenTrans;
                aopResult = _aop.Begin(Aop.AopEnum.Select);
            }
            if (aopResult == AopResult.Default || aopResult == AopResult.Continue)
            {
                string primaryKey = SqlFormat.Keyword(_Data.Columns.FirstPrimary.ColumnName, dalHelper.dalType);//����������
                switch (dalHelper.dalType)
                {
                    case DalType.Txt:
                    case DalType.Xml:
                        _aop.Para.Table = _noSqlAction.Select(pageIndex, pageSize, _sqlCreate.FormatWhere(where), out rowCount, _sqlCreate.selectColumns);
                        dalHelper.recordsAffected = rowCount;
                        break;
                    default:
                        _aop.Para.Table = new MDataTable(_TableName.Contains("(") ? "SysDefaultCustomTable" : _TableName);
                        _aop.Para.Table.LoadRow(_Data);
                        ClearParameters();//------------------------�������
                        DbDataReader sdReader = null;
                        string whereSql = string.Empty;//�Ѹ�ʽ������ԭ��whereSql���
                        if (_sqlCreate != null)
                        {
                            whereSql = _sqlCreate.FormatWhere(where);
                        }
                        else
                        {
                            whereSql = SqlFormat.Compatible(where, dalHelper.dalType, dalHelper.Com.Parameters.Count == 0);
                        }
                        bool byPager = pageIndex > 0 && pageSize > 0;//��ҳ��ѯ(��һҳҲҪ��ҳ��ѯ����ΪҪ����������
                        if (byPager && AppConfig.DB.PagerBySelectBase && dalHelper.dalType == DalType.MsSql && !dalHelper.Version.StartsWith("08"))// || dalHelper.dalType == DalType.Oracle
                        {
                            #region �洢����ִ��
                            if (dalHelper.Com.Parameters.Count > 0)
                            {
                                dalHelper.debugInfo.Append(AppConst.HR + "error : select method deny call SetPara() method to add custom parameters!");
                            }
                            dalHelper.AddParameters("@PageIndex", pageIndex, DbType.Int32, -1, ParameterDirection.Input);
                            dalHelper.AddParameters("@PageSize", pageSize, DbType.Int32, -1, ParameterDirection.Input);
                            dalHelper.AddParameters("@TableName", _sqlCreate.GetSelectTableName(ref whereSql), DbType.String, -1, ParameterDirection.Input);

                            whereSql = _sqlCreate.AddOrderByWithCheck(whereSql, primaryKey);

                            dalHelper.AddParameters("@Where", whereSql, DbType.String, -1, ParameterDirection.Input);
                            sdReader = dalHelper.ExeDataReader("SelectBase", true);
                            #endregion
                        }
                        else
                        {
                            #region SQL����ҳִ��
                            if (byPager)
                            {
                                rowCount = GetCount(whereSql);//�����Զ����棬����ÿ�η�ҳ��Ҫ����������
                                _aop.Para.Where = where;//�ָ�Ӱ�������������Ӱ�컺��key
                                _aop.isHasCache = false;//����Ӱ��Select�ĺ���������
                                //rowCount = Convert.ToInt32(dalHelper.ExeScalar(_sqlCreate.GetCountSql(whereSql), false));//��ҳ��ѯ�ȼ�������
                            }
                            if (!byPager || (rowCount > 0 && (pageIndex - 1) * pageSize < rowCount))
                            {
                                string sql = SqlCreateForPager.GetSql(dalHelper.dalType, dalHelper.Version, pageIndex, pageSize, whereSql, SqlFormat.Keyword(_TableName, dalHelper.dalType), rowCount, _sqlCreate.GetColumnsSql(), primaryKey, _Data.PrimaryCell.Struct.IsAutoIncrement);
                                sdReader = dalHelper.ExeDataReader(sql, false);
                            }
                            else if (_sqlCreate.selectColumns != null)
                            {
                                _aop.Para.Table = _aop.Para.Table.Select(0, 0, null, _sqlCreate.selectColumns);
                            }
                            #endregion
                        }
                        if (sdReader != null)
                        {
                            // _aop.Para.Table.ReadFromDbDataReader(sdReader);//�ڲ��йرա�
                            _aop.Para.Table = sdReader;
                            if (!byPager)
                            {
                                rowCount = _aop.Para.Table.Rows.Count;
                            }
                            else if (dalHelper.dalType == DalType.MsSql && AppConfig.DB.PagerBySelectBase)
                            {
                                rowCount = dalHelper.ReturnValue;
                            }
                            _aop.Para.Table.RecordsAffected = rowCount;
                        }
                        else
                        {
                            _aop.Para.Table.Rows.Clear();//Ԥ��֮ǰ�Ĳ������������һ�������С�
                        }
                        _aop.Para.IsSuccess = _aop.Para.Table.Rows.Count > 0;
                        ClearParameters();//------------------------�������
                        break;
                }
            }
            else if (_aop.Para.Table.RecordsAffected > 0)
            {
                rowCount = _aop.Para.Table.RecordsAffected;//���ؼ�¼����
            }
            if (_aop.IsLoadAop && (aopResult == AopResult.Break || aopResult == AopResult.Continue))
            {
                _aop.End(Aop.AopEnum.Select);
            }
            _aop.Para.Table.TableName = TableName;//Aop��Json�������ʱ�ᶪʧ������
            _aop.Para.Table.Conn = _Data.Conn;
            //����DataType��Size��Scale
            for (int i = 0; i < _aop.Para.Table.Columns.Count; i++)
            {
                MCellStruct msTable = _aop.Para.Table.Columns[i];
                MCellStruct ms = _Data.Columns[msTable.ColumnName];
                if (ms != null)
                {
                    msTable.Load(ms);
                }
            }
            if (_sqlCreate != null)
            {
                _sqlCreate.selectColumns = null;
            }
            return _aop.Para.Table;
        }

        /// <summary>
        /// Select top one row to fill this.Data
        /// <para>ѡ��һ�����ݣ�����䵽Data����[����where����ʱ���Զ����Դ�UI��ȡ]</para>
        /// </summary>
        public bool Fill()
        {
            return Fill(null);
        }

        /// <param name="where">Sql statement where the conditions: 88, "id = 88"
        /// <para>sql����where������88��"id=88"</para></param>
        public bool Fill(object where)
        {
            if (CheckDisposed()) { return false; }
            if (where == null || Convert.ToString(where) == "")
            {
                _UI.PrimayAutoGetValue();
                where = _sqlCreate.GetPrimaryWhere();
            }
            AopResult aopResult = AopResult.Default;
            if (_aop.IsLoadAop)
            {
                _aop.Para.MAction = this;
                _aop.Para.TableName = _sourceTableName;
                _aop.Para.Row = _Data;
                _aop.Para.Where = where;
                _aop.Para.SelectColumns = _sqlCreate.selectColumns;
                //_aop.Para.AopPara = aopPara;
                _aop.Para.IsTransaction = dalHelper.isOpenTrans;
                aopResult = _aop.Begin(Aop.AopEnum.Fill);
            }
            if (aopResult == AopResult.Default || aopResult == AopResult.Continue)
            {
                switch (dalHelper.dalType)
                {
                    case DalType.Txt:
                    case DalType.Xml:
                        _aop.Para.IsSuccess = _noSqlAction.Fill(_sqlCreate.FormatWhere(where));
                        dalHelper.recordsAffected = _aop.Para.IsSuccess ? 1 : 0;
                        break;
                    default:
                        ClearParameters();
                        MDataTable mTable = dalHelper.ExeDataReader(_sqlCreate.GetTopOneSql(where), false);
                        // dalHelper.ResetConn();//����Slave
                        if (mTable != null && mTable.Rows.Count > 0)
                        {
                            _Data.Clear();//�����ֵ��
                            _Data.LoadFrom(mTable.Rows[0], RowOp.None, true);//setselectcolumn("aa as bb")ʱ
                            _aop.Para.IsSuccess = true;
                        }
                        else
                        {
                            _aop.Para.IsSuccess = false;
                        }
                        break;
                }
            }
            else if (_aop.Para.IsSuccess)
            {
                _Data.Clear();//�����ֵ��
                _Data.LoadFrom(_aop.Para.Row, RowOp.None, true);
            }

            if (_aop.IsLoadAop && (aopResult == AopResult.Break || aopResult == AopResult.Continue))
            {
                _aop.Para.Row = _Data;
                _aop.End(Aop.AopEnum.Fill);
            }
            if (_aop.Para.IsSuccess)
            {
                if (_sqlCreate.selectColumns != null)
                {
                    string name;
                    string[] items;
                    foreach (object columnName in _sqlCreate.selectColumns)
                    {
                        items = columnName.ToString().Split(' ');
                        name = items[items.Length - 1];
                        MDataCell cell = _Data[name];
                        if (cell != null)
                        {
                            cell.State = 1;
                        }
                    }
                    items = null;
                }
                else
                {
                    _Data.SetState(1, BreakOp.Null);//��ѯʱ����λ״̬Ϊ1
                }
            }
            if (dalHelper.recordsAffected == -2)
            {
                OnError();
            }
            if (_sqlCreate != null)
            {
                _sqlCreate.selectColumns = null;
            }
            return _aop.Para.IsSuccess;
        }
        /// <summary>
        /// Returns the number of records
        /// <para>���ؼ�¼��</para>
        /// </summary>
        public int GetCount()
        {
            return GetCount(null);
        }

        /// <param name="where">Sql statement where the conditions: 88, "id = 88"
        /// <para>sql����where������88��"id=88"</para></param>
        public int GetCount(object where)
        {
            if (CheckDisposed()) { return -1; }
            AopResult aopResult = AopResult.Default;
            if (_aop.IsLoadAop)
            {
                _aop.Para.MAction = this;
                _aop.Para.TableName = _sourceTableName;
                _aop.Para.Row = _Data;
                _aop.Para.Where = where;
                _aop.Para.IsTransaction = dalHelper.isOpenTrans;
                aopResult = _aop.Begin(Aop.AopEnum.GetCount);
            }
            if (aopResult == AopResult.Default || aopResult == AopResult.Continue)
            {
                switch (dalHelper.dalType)
                {
                    case DalType.Txt:
                    case DalType.Xml:
                        _aop.Para.RowCount = _noSqlAction.GetCount(_sqlCreate.FormatWhere(where));
                        _aop.Para.IsSuccess = _aop.Para.RowCount > 0;
                        dalHelper.recordsAffected = _aop.Para.RowCount;
                        break;
                    default:
                        ClearParameters();//���ϵͳ����
                        string countSql = _sqlCreate.GetCountSql(where);
                        object result = dalHelper.ExeScalar(countSql, false);

                        _aop.Para.IsSuccess = result != null;
                        if (_aop.Para.IsSuccess)
                        {
                            _aop.Para.RowCount = Convert.ToInt32(result);
                        }
                        else
                        {
                            _aop.Para.RowCount = -1;
                        }

                        //ClearSysPara(); //����ڲ��Զ������[FormatWhere���Զ������]
                        break;
                }
            }
            if (_aop.IsLoadAop && (aopResult == AopResult.Break || aopResult == AopResult.Continue))
            {
                _aop.End(Aop.AopEnum.GetCount);
            }
            if (dalHelper.recordsAffected == -2)
            {
                OnError();
            }
            return _aop.Para.RowCount;
        }
        /// <summary>
        /// Whether or not the specified condition exists
        /// <para>�Ƿ����ָ������������</para>
        /// </summary>
        /// <param name="where">Sql statement where the conditions: 88, "id = 88"
        /// <para>sql����where������88��"id=88"</para></param>
        public bool Exists(object where)
        {
            if (CheckDisposed()) { return false; }
            AopResult aopResult = AopResult.Default;
            if (_aop.IsLoadAop)
            {
                _aop.Para.MAction = this;
                _aop.Para.TableName = _sourceTableName;
                _aop.Para.Row = _Data;
                _aop.Para.Where = where;
                _aop.Para.IsTransaction = dalHelper.isOpenTrans;
                aopResult = _aop.Begin(Aop.AopEnum.Exists);
            }
            if (aopResult == AopResult.Default || aopResult == AopResult.Continue)
            {
                switch (dalHelper.dalType)
                {
                    case DalType.Txt:
                    case DalType.Xml:
                        _aop.Para.IsSuccess = _noSqlAction.Exists(_sqlCreate.FormatWhere(where));
                        _aop.Para.ExeResult = _aop.Para.IsSuccess;
                        dalHelper.recordsAffected = _aop.Para.IsSuccess ? 1 : 0;
                        break;
                    default:
                        ClearParameters();//���ϵͳ����
                        string countSql = _sqlCreate.GetExistsSql(where);
                        _aop.Para.ExeResult = Convert.ToString(dalHelper.ExeScalar(countSql, false)) == "1" ? true : false;
                        _aop.Para.IsSuccess = dalHelper.recordsAffected != -2;
                        break;
                }
            }
            if (_aop.IsLoadAop && (aopResult == AopResult.Break || aopResult == AopResult.Continue))
            {
                _aop.End(Aop.AopEnum.Exists);
            }
            if (dalHelper.recordsAffected == -2)
            {
                OnError();
            }
            return Convert.ToBoolean(_aop.Para.ExeResult);

            //switch (dalHelper.dalType)
            //{
            //    case DalType.Txt:
            //    case DalType.Xml:
            //        return _noSqlAction.Exists(_sqlCreate.FormatWhere(where));
            //    default:
            //        return GetCount(where) > 0;
            //}
        }
        #endregion

        #region ��������



        /// <summary>
        /// Get value from this.Data
        /// <para>��Data������ȡֵ</para>
        /// </summary>
        public T Get<T>(object key)
        {
            return _Data.Get<T>(key);
        }


        /// <param name="defaultValue">defaultValue<para>ֵΪNullʱ��Ĭ���滻ֵ</para></param>
        public T Get<T>(object key, T defaultValue)
        {
            return _Data.Get<T>(key, defaultValue);
        }
        /// <summary>
        /// Set value to this.Data
        /// <para>ΪData��������ֵ</para>
        /// </summary>
        /// <param name="key">columnName<para>����</para></param>
        /// <param name="value">value<para>ֵ</para></param>
        public MAction Set(object key, object value)
        {
            return Set(key, value, -1);
        }

        /// <param name="state">set value state (0: unchanged; 1: assigned, the same value [insertable]; 2: assigned, different values [updateable])
        /// <para>����ֵ״̬[0:δ���ģ�1:�Ѹ�ֵ,ֵ��ͬ[�ɲ���]��2:�Ѹ�ֵ,ֵ��ͬ[�ɸ���]]</para></param>
        public MAction Set(object key, object value, int state)
        {
            MDataCell cell = _Data[key];
            if (cell != null)
            {
                cell.Value = value;
                if (state >= 0 && state < 3)
                {
                    cell.State = state;
                }
            }
            else
            {
                dalHelper.debugInfo.Append(AppConst.HR + "Alarm : can't find the ColumnName:" + key);
            }
            return this;
        }
        /// <summary>
        /// Sets a custom expression for the Update operation.
        /// <para>ΪUpdate���������Զ�����ʽ��</para>
        /// </summary>
        /// <param name="updateExpression">as��"a=a+1"<para>�磺"a=a+1"</para></param>
        public MAction SetExpression(string updateExpression)
        {
            _sqlCreate.updateExpression = updateExpression;
            return this;
        }
        /// <summary>
        /// Parameterized pass [used when the Where condition is a parameterized (such as: name = @ name) statement]
        /// <para>����������[��Where����Ϊ������(�磺name=@name)���ʱʹ��]</para>
        /// </summary>
        /// <param name="paraName">paraName<para>��������</para></param>
        /// <param name="value">value<para>����ֵ</para></param>
        public MAction SetPara(object paraName, object value)
        {
            return SetPara(paraName, value, DbType.String);
        }
        List<AopCustomDbPara> customParaNames = new List<AopCustomDbPara>();

        /// <param name="dbType">dbType<para>��������</para></param>
        public MAction SetPara(object paraName, object value, DbType dbType)
        {
            if (dalHelper.AddParameters(Convert.ToString(paraName), value, dbType, -1, ParameterDirection.Input))
            {
                AopCustomDbPara para = new AopCustomDbPara();
                para.ParaName = Convert.ToString(paraName).Replace(":", "").Replace("@", "");
                para.Value = value;
                para.ParaDbType = dbType;
                customParaNames.Add(para);
                if (_aop.IsLoadAop)
                {
                    _aop.Para.CustomDbPara = customParaNames;
                }
            }
            return this;
        }

        /// <summary>
        /// Aop scenes can only be used by the parameterization of the Senate
        /// <para>Aop�����ſ���ʹ�õĲ���������</para>
        /// </summary>
        /// <param name="customParas">Paras<para>�����б�</para></param>
        public MAction SetPara(List<AopCustomDbPara> customParas)
        {
            if (customParas != null && customParas.Count > 0)
            {
                foreach (AopCustomDbPara para in customParas)
                {
                    SetPara(para.ParaName, para.Value, para.ParaDbType);
                }
            }
            return this;
        }
        /// <summary>
        /// Clears paras (from SetPara method)
        /// <para>���(SetPara���õ�)�Զ������</para>
        /// </summary>
        public void ClearPara()
        {
            if (customParaNames.Count > 0)
            {
                if (dalHelper != null && dalHelper.Com.Parameters.Count > 0)
                {
                    string paraName = string.Empty;
                    foreach (AopCustomDbPara item in customParaNames)
                    {
                        for (int i = dalHelper.Com.Parameters.Count - 1; i > -1; i--)
                        {
                            if (string.Compare(dalHelper.Com.Parameters[i].ParameterName.TrimStart(dalHelper.Pre), item.ParaName.ToString()) == 0)
                            {
                                dalHelper.Com.Parameters.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
                customParaNames.Clear();
            }
        }


        /// <summary>
        /// ���ϵͳ����[�����Զ������]
        /// </summary>
        private void ClearParameters()
        {
            if (dalHelper != null)
            {
                if (customParaNames.Count > 0)//�����Զ������
                {
                    if (dalHelper.Com.Parameters.Count > 0)
                    {
                        bool isBreak = false;
                        for (int i = dalHelper.Com.Parameters.Count - 1; i > -1; i--)
                        {
                            for (int j = 0; j < customParaNames.Count; j++)
                            {
                                if (string.Compare(dalHelper.Com.Parameters[i].ParameterName.TrimStart(dalHelper.Pre), customParaNames[j].ParaName.ToString()) == 0)
                                {
                                    isBreak = true;
                                }
                            }
                            if (!isBreak)
                            {
                                dalHelper.Com.Parameters.RemoveAt(i);
                                isBreak = false;
                            }
                        }
                    }
                }
                else
                {
                    dalHelper.ClearParameters();
                }
            }
        }

        /// <summary>
        /// Specifies the column to read
        /// <para>ָ����ȡ����</para>
        /// </summary>
        /// <param name="columnNames">as��"columnA","columnB as B"</param>
        public MAction SetSelectColumns(params object[] columnNames)
        {
            bool isSplit = false;
            if (columnNames.Length == 1)
            {
                string column = Convert.ToString(columnNames[0]);
                if (column.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) == -1)//�ܿ�"'xx,xx' as A
                {
                    string[] items = Convert.ToString(columnNames[0]).Split(',');
                    if (items.Length > 1)
                    {
                        isSplit = true;
                        _sqlCreate.selectColumns = items;
                    }
                }
            }
            if (!isSplit)
            {
                _sqlCreate.selectColumns = columnNames;
            }
            return this;
        }

        /// <summary>
        ///  Get where statement
        /// <para>����Ԫ���������where������</para>
        /// </summary>
        /// <param name="isAnd">connect by and/or<para>trueΪand���ӣ���֮Ϊor����</para></param>
        public string GetWhere(bool isAnd, params MDataCell[] cells)
        {
            List<MDataCell> cs = new List<MDataCell>(cells.Length);
            if (cells.Length > 0)
            {
                cs.AddRange(cells);
            }
            return SqlCreate.GetWhere(DalType, isAnd, cs);
        }

        /// <param name="cells">MDataCell<para>��Ԫ��</para></param>
        public string GetWhere(params MDataCell[] cells)
        {
            return GetWhere(true, cells);
        }



        #endregion

        #region �������
        /// <summary>
        /// Set the transaction level
        /// <para>�������񼶱�</para>
        /// </summary>
        /// <param name="level">IsolationLevel</param>
        public MAction SetTransLevel(IsolationLevel level)
        {
            dalHelper.TranLevel = level;
            return this;
        }

        /// <summary>
        /// Begin Transation
        /// <para>��ʼ����</para>
        /// </summary>
        public void BeginTransation()
        {
            dalHelper.isOpenTrans = true;
        }
        /// <summary>
        /// Commit Transation
        /// <para>�ύ����</para>
        /// </summary>
        public bool EndTransation()
        {
            if (dalHelper != null && dalHelper.isOpenTrans)
            {
                return dalHelper.EndTransaction();
            }
            return false;
        }
        /// <summary>
        /// RollBack Transation
        /// <para>����ع�</para>
        /// </summary>
        public bool RollBack()
        {
            if (dalHelper != null && dalHelper.isOpenTrans)
            {
                return dalHelper.RollBack();
            }
            return false;
        }
        #endregion

        #region IDisposable ��Ա
        public void Dispose()
        {
            Dispose(false);
        }
        /// <summary>
        /// Dispose
        /// <para>�ͷ���Դ</para>
        /// </summary>
        internal void Dispose(bool isOnError)
        {
            hasDisposed = true;
            if (dalHelper != null)
            {
                if (!dalHelper.IsOnExceptionEventNull)
                {
                    dalHelper.OnExceptionEvent -= new DbBase.OnException(_DataSqlHelper_OnExceptionEvent);
                }
                _debugInfo = dalHelper.debugInfo.ToString();
                dalHelper.Dispose();
                if (!isOnError)
                {
                    dalHelper = null;

                    if (_sqlCreate != null)
                    {
                        _sqlCreate = null;
                    }
                }
            }
            if (!isOnError)
            {
                if (_noSqlAction != null)
                {
                    _noSqlAction.Dispose();
                }
                if (_aop != null)
                {
                    _aop = null;
                }
            }
        }
        internal void OnError()
        {
            if (dalHelper != null && dalHelper.isOpenTrans)
            {
                Dispose(true);
            }
        }
        bool hasDisposed = false;
        private bool CheckDisposed()
        {
            if (hasDisposed || _Data.Columns.Count == 0)
            {
                Error.Throw("The current object 'MAction' has been disposed");
                return true;
            }
            return false;
        }
        #endregion


    }
    //AOP ����
    public partial class MAction
    {
        #region Aop����
        private InterAop _aop = new InterAop();
        /// <summary>
        /// Set Aop State
        /// <para>����Aop״̬</para>
        /// </summary>
        public MProc SetAopState(AopOp op)
        {
            _aop.aopOp = op;
            return this;
        }
        /// <summary>
        /// Pass additional parameters for Aop use
        /// <para>���ݶ���Ĳ�����Aopʹ��</para>
        /// </summary>
        public MAction SetAopPara(object para)
        {
            _aop.Para.AopPara = para;
            return this;
        }

        #endregion
    }
    //UI ����
    public partial class MAction
    {
        private MActionUI _UI;
        /// <summary>
        /// Manipulate UI
        /// <para>UI����</para>
        /// </summary>
        public MActionUI UI
        {
            get
            {
                return _UI;
            }
        }
    }

}
