using System;
using System.Collections.Generic;
using System.Text;
using CYQ.Data.Table;
using System.Reflection;

using CYQ.Data.Tool;
using CYQ.Data.Cache;
using CYQ.Data.SQL;
using System.Data;
using CYQ.Data.Aop;


namespace CYQ.Data.Orm
{
    /// <summary>
    /// ORM CodeFirst �ֶ���Դ
    /// </summary>
    public enum FieldSource
    {
        /// <summary>
        /// ��ʵ��������
        /// </summary>
        Property,
        /// <summary>
        /// �����ݿ���ı�������
        /// </summary>
        Data,
        /// <summary>
        /// �ۺ���������
        /// </summary>
        BothOfAll
    }
    /// <summary>
    /// ��ORM���ࣨ�����ݽ������ܣ�
    /// </summary>
    public class SimpleOrmBase : IDisposable
    {
        internal MDataColumn Columns = null;
        /// <summary>
        /// ��ʶ�Ƿ�����д��־��
        /// </summary>
        internal bool AllowWriteLog
        {
            set
            {
                Action.dalHelper.isAllowInterWriteLog = value;
            }
        }
        /// <summary>
        /// �Ƿ�������AOP���������ֶ�ֵͬ����
        /// </summary>
        internal bool IsUseAop = false;
        private static FieldSource _FieldSource = FieldSource.BothOfAll;
        /// <summary>
        ///  �ֶ���Դ�����ֶα��ʱ���������ô��������л����£�
        /// </summary>
        public static FieldSource FieldSource
        {
            get
            {
                return _FieldSource;
            }
            set
            {
                _FieldSource = value;
            }
        }

        Object entity;//ʵ�����
        Type typeInfo;//ʵ���������
        private MAction _Action;
        internal MAction Action
        {
            get
            {
                if (_Action == null)
                {
                    SetDelayInit(_entityInstance, _tableName, _conn, _op);//�ӳټ���
                }
                return _Action;
            }
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        public SimpleOrmBase()
        {

        }
        /// <summary>
        /// ����Aop״̬
        /// </summary>
        /// <param name="op"></param>
        public void SetAopState(AopOp op)
        {
            Action.SetAopState(op);
        }
        /// <summary>
        /// ��ʼ��״̬[�̳д˻����ʵ���ڹ��캯��������ô˷���]
        /// </summary>
        /// <param name="entityInstance">ʵ�����,һ��д:this</param>
        protected void SetInit(Object entityInstance)
        {
            SetInit(entityInstance, null, AppConfig.DB.DefaultConn);
        }
        /// <summary>
        /// ��ʼ��״̬[�̳д˻����ʵ���ڹ��캯��������ô˷���]
        /// </summary>
        /// <param name="entityInstance">ʵ�����,һ��д:this</param>
        /// <param name="tableName">����,��:Users</param>
        protected void SetInit(Object entityInstance, string tableName)
        {
            SetInit(entityInstance, tableName, AppConfig.DB.DefaultConn);
        }
        /// <summary>
        /// ��ʼ��״̬[�̳д˻����ʵ���ڹ��캯��������ô˷���]
        /// </summary>
        /// <param name="entityInstance">ʵ�����,һ��д:this</param>
        /// <param name="tableName">����,��:Users</param>
        /// <param name="conn">��������,�����ݿ�ʱ��дNull,��дĬ������������:"Conn",��ֱ�����ݿ������ַ���</param>
        protected void SetInit(Object entityInstance, string tableName, string conn)
        {
            SetInit(entityInstance, tableName, conn, AopOp.OpenAll);
        }
        private object _entityInstance;
        private string _tableName;
        private string _conn;
        private AopOp _op;
        protected void SetInit(Object entityInstance, string tableName, string conn, AopOp op)
        {
            _entityInstance = entityInstance;
            _tableName = tableName;
            _conn = conn;
            _op = op;
        }
        /// <summary>
        /// ��ԭ�еĳ�ʼ���������ʱ���ء�
        /// </summary>
        private void SetDelayInit(Object entityInstance, string tableName, string conn, AopOp op)
        {
            conn = string.IsNullOrEmpty(conn) ? AppConfig.DB.DefaultConn : conn;
            entity = entityInstance;
            typeInfo = entity.GetType();
            try
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = typeInfo.Name;
                    if (tableName.EndsWith(AppConfig.EntitySuffix))
                    {
                        tableName = tableName.Substring(0, tableName.Length - AppConfig.EntitySuffix.Length);
                    }
                }

                string key = tableName + StaticTool.GetHashKey(conn);
                if (!CacheManage.LocalInstance.Contains(key))
                {
                    DalType dal = DBTool.GetDalType(conn);
                    bool isTxtDal = dal == DalType.Txt || dal == DalType.Xml;
                    string errMsg = string.Empty;
                    Columns = DBTool.GetColumns(tableName, conn, out errMsg);//�ڲ����Ӵ���ʱ���쳣��
                    if (Columns == null || Columns.Count == 0)
                    {
                        if (errMsg != string.Empty)
                        {
                            Error.Throw(errMsg);
                        }
                        Columns = TableSchema.GetColumns(typeInfo);
                        if (!DBTool.ExistsTable(tableName, conn))
                        {
                            DBTool.ErrorMsg = null;
                            if (!DBTool.CreateTable(tableName, Columns, conn))
                            {
                                Error.Throw("SimpleOrmBase ��Create Table Error:" + tableName + DBTool.ErrorMsg);
                            }
                        }
                    }
                    else if (isTxtDal)//�ı����ݿ�
                    {
                        if (FieldSource != FieldSource.Data)
                        {
                            MDataColumn c2 = TableSchema.GetColumns(typeInfo);
                            if (FieldSource == FieldSource.BothOfAll)
                            {
                                Columns.AddRange(c2);
                            }
                            else
                            {
                                Columns = c2;
                            }
                        }
                    }

                    if (Columns != null && Columns.Count > 0)
                    {
                        CacheManage.LocalInstance.Set(key, Columns, 1440, null);
                    }
                }
                else
                {
                    Columns = CacheManage.LocalInstance.Get(key) as MDataColumn;
                }

                _Action = new MAction(Columns.ToRow(tableName), conn);
                if (typeInfo.Name == "SysLogs")
                {
                    _Action.SetAopState(Aop.AopOp.CloseAll);
                }
                else
                {
                    _Action.SetAopState(op);
                }
                _Action.EndTransation();
            }
            catch (Exception err)
            {
                if (typeInfo.Name != "SysLogs")
                {
                    Log.WriteLogToTxt(err);
                }
                throw;
            }
        }
        internal void SetInit2(Object entityInstance, string tableName, string conn, AopOp op)
        {
            SetInit(entityInstance, tableName, conn, op);
        }
        internal void SetInit2(Object entityInstance, string tableName, string conn)
        {
            SetInit(entityInstance, tableName, conn);
        }
        internal void Set(object key, object value)
        {
            if (Action != null)
            {
                Action.Set(key, value);
            }
        }
        #region ������ɾ�Ĳ� ��Ա

        #region ����
        /// <summary>
        /// ��������
        /// </summary>
        public bool Insert()
        {
            return Insert(InsertOp.ID);
        }
        /// <summary>
        ///  ��������
        /// </summary>
        /// <param name="option">����ѡ��</param>
        public bool Insert(InsertOp option)
        {
            return Insert(false, option, false);
        }
        /// <summary>
        ///  ��������
        /// </summary>
        /// <param name="insertID">��������</param>
        public bool Insert(InsertOp option, bool insertID)
        {
            return Insert(false, option, insertID);
        }
        /*
        /// <summary>
        ///  ��������
        /// </summary>
        /// <param name="autoSetValue">�Զ��ӿ��ƻ�ȡֵ</param>
        internal bool Insert(bool autoSetValue)
        {
            return Insert(autoSetValue, InsertOp.ID);
        }
        internal bool Insert(bool autoSetValue, InsertOp option)
        {
            return Insert(autoSetValue, InsertOp.ID, false);
        }*/
        /// <summary>
        ///  ��������
        /// </summary>
        /// <param name="autoSetValue">�Զ��ӿ��ƻ�ȡֵ</param>
        /// <param name="option">����ѡ��</param>
        /// <param name="insertID">��������</param>
        internal bool Insert(bool autoSetValue, InsertOp option, bool insertID)
        {
            if (autoSetValue)
            {
                Action.UI.GetAll(!insertID);
            }
            GetValueFromEntity();
            Action.AllowInsertID = insertID;
            bool result = Action.Insert(false, option);
            if (autoSetValue || option != InsertOp.None)
            {
                SetValueToEntity();
            }
            return result;
        }
        #endregion

        #region ����
        /// <summary>
        ///  ��������
        /// </summary>
        public bool Update()
        {
            return Update(null, false);
        }
        /// <summary>
        ///  ��������
        /// </summary>
        /// <param name="where">where����,��ֱ�Ӵ�id��ֵ��:[88],������where������:[id=88 and name='·������']</param>
        public bool Update(object where)
        {
            return Update(where, false);
        }
        /// <summary>
        ///  ��������
        /// </summary>
        /// <param name="where">where����,��ֱ�Ӵ�id��ֵ��:[88],������where������:[id=88 and name='·������']</param>
        /// <param name="autoSetValue">�Ƿ��Զ���ȡֵ[�Զ��ӿؼ���ȡֵ,��Ҫ�ȵ���SetAutoPrefix��SetAutoParentControl�������ÿؼ�ǰ׺]</param>
        internal bool Update(object where, bool autoSetValue)
        {
            if (autoSetValue)
            {
                Action.UI.GetAll(false);
            }
            GetValueFromEntity();
            bool result = Action.Update(where);
            if (autoSetValue)
            {
                SetValueToEntity();
            }
            return result;
        }
        #endregion

        #region ɾ��
        /// <summary>
        ///  ɾ������
        /// </summary>
        public bool Delete()
        {
            return Delete(null);
        }
        /// <summary>
        ///  ɾ������
        /// </summary>
        /// <param name="where">where����,��ֱ�Ӵ�id��ֵ��:[88],������where������:[id=88 and name='·������']</param>
        public bool Delete(object where)
        {
            GetValueFromEntity();
            return Action.Delete(where);
        }
        #endregion

        #region ��ѯ

        /// <summary>
        /// ��ѯ1������
        /// </summary>
        public bool Fill()
        {
            return Fill(null);
        }
        /// <summary>
        /// ��ѯ1������
        /// </summary>
        public bool Fill(object where)
        {
            bool result = Action.Fill(where);
            if (result)
            {
                SetValueToEntity();
            }
            return result;
        }

        /// <summary>
        /// �б��ѯ
        /// </summary>
        public List<T> Select<T>()
        {
            int count = 0;
            return Select<T>(0, 0, null, out count);
        }
        /// <summary>
        /// �б��ѯ
        /// </summary>
        /// <param name="where">��ѯ����[�ɸ��� order by ���]</param>
        /// <returns></returns>
        public List<T> Select<T>(string where)
        {
            int count = 0;
            return Select<T>(0, 0, where, out count);
        }
        /// <summary>
        /// �б��ѯ
        /// </summary>
        /// <param name="topN">��ѯ����</param>
        /// <param name="where">��ѯ����[�ɸ��� order by ���]</param>
        /// <returns></returns>
        public List<T> Select<T>(int topN, string where)
        {
            int count = 0;
            return Select<T>(0, topN, where, out count);
        }
        public List<T> Select<T>(int pageIndex, int pageSize)
        {
            int count = 0;
            return Select<T>(pageIndex, pageSize, null, out count);
        }
        public List<T> Select<T>(int pageIndex, int pageSize, string where)
        {
            int count = 0;
            return Select<T>(pageIndex, pageSize, where, out count);
        }
        /// <summary>
        /// ���ֲ����ܵ�ѡ��[��������ѯ,ѡ������ʱֻ���PageIndex/PageSize����Ϊ0]
        /// </summary>
        /// <param name="pageIndex">�ڼ�ҳ</param>
        /// <param name="pageSize">ÿҳ����[Ϊ0ʱĬ��ѡ������]</param>
        /// <param name="where"> ��ѯ����[�ɸ��� order by ���]</param>
        /// <param name="count">���صļ�¼����</param>
        public List<T> Select<T>(int pageIndex, int pageSize, string where, out int count)
        {
            return Action.Select(pageIndex, pageSize, where, out count).ToList<T>();
        }
        internal MDataTable Select(int pageIndex, int pageSize, string where, out int count)
        {
            return Action.Select(pageIndex, pageSize, where, out count);
        }
        /// <summary>
        /// ��ȡ��¼����
        /// </summary>
        public int GetCount(object where)
        {
            return Action.GetCount(where);
        }
        /// <summary>
        /// ��ѯ�Ƿ����ָ��������������
        /// </summary>
        public bool Exists(object where)
        {
            return Action.Exists(where);
        }

        #endregion

        #endregion
        internal void SetValueToEntity()
        {
            SetValueToEntity(null);
        }
        internal void SetValueToEntity(string propName)
        {
            if (!string.IsNullOrEmpty(propName))
            {
                PropertyInfo pi = typeInfo.GetProperty(propName);
                if (pi != null)
                {
                    MDataCell cell = Action.Data[propName];
                    if (cell != null && !cell.IsNull)
                    {
                        try
                        {
                            pi.SetValue(entity, cell.Value, null);
                        }
                        catch
                        {

                        }

                    }
                }
            }
            else
            {
                Action.Data.SetToEntity(entity);
            }
        }
        private void GetValueFromEntity()
        {
            if (!IsUseAop || AppConfig.IsAspNetCore)//ASPNETCore�£���̬�����Aop����Ч��
            {
                Action.Data.LoadFrom(entity, BreakOp.Null);
            }
        }
        #region IDisposable ��Ա
        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            if (Action != null)
            {
                Action.Dispose();
            }
        }

        #endregion
    }
}
