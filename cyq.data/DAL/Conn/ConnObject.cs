using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;

namespace CYQ.Data
{
    internal class ConnBean
    {
        /// <summary>
        /// ��Ӧ��ConnectionString��Name
        /// </summary>
        internal string ConfigName = string.Empty;
        /// <summary>
        /// ���ӵ�״̬�Ƿ�������
        /// </summary>
        internal bool IsOK = true;
        /// <summary>
        /// �Ƿ�ӿ�
        /// </summary>
        internal bool IsSlave = false;
        /// <summary>
        /// ���Ӵ���ʱ���쳣��Ϣ��
        /// </summary>
        internal string ErrorMsg = string.Empty;
        private string _Conn = string.Empty;
        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public string Conn
        {
            get { return _Conn; }
            set { _Conn = value; }
        }
        private string _ProviderName;
        /// <summary>
        /// ���������ṩ��
        /// </summary>
        public string ProviderName
        {
            get { return _ProviderName; }
            set { _ProviderName = value; }
        }
        private DalType _ConnDalType;
        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public DalType ConnDalType
        {
            get { return _ConnDalType; }
            set { _ConnDalType = value; }
        }
        public ConnBean Clone()
        {
            ConnBean cb = new ConnBean();
            cb.Conn = this.Conn;
            cb.ProviderName = this.ProviderName;
            cb.ConnDalType = this.ConnDalType;
            cb.ConfigName = this.ConfigName;
            cb.IsOK = this.IsOK;
            return cb;
        }
        //public string TryTestConn()
        //{
        //    string err;
        //    return TryTestConn(out err);
        //}
        public string TryTestConn()
        {
            string version = string.Empty;
            //err = string.Empty;
            if (!string.IsNullOrEmpty(Conn))
            {
                DbBase helper = DalCreate.CreateDal(Conn);
                try
                {

                    helper.Con.Open();
                    version = helper.Con.ServerVersion;
                    if (string.IsNullOrEmpty(version)) { version = helper.dalType.ToString(); }
                    helper.Con.Close();
                    IsOK = true;
                    ErrorMsg = string.Empty;
                }
                catch (Exception er)
                {
                    ErrorMsg = er.Message;
                    //err = er.Message;
                    IsOK = false;
                }
                finally
                {
                    helper.Dispose();
                }
            }
            else
            {
                IsOK = false;
            }
            return version;
        }
    }
    internal class ConnObject
    {
        public ConnBean Master = new ConnBean();
        public ConnBean BackUp;
        public List<ConnBean> Slave = new List<ConnBean>();
        internal void InterChange()
        {
            if (BackUp != null)
            {
                ConnBean middle = Master;
                Master = BackUp;
                BackUp = middle;
            }
        }
        static int index = -1, times = 1;
        public ConnBean GetSlave()
        {
            if (Slave.Count > 0)
            {
                if (index == -1)
                {
                    index = new Random().Next(Slave.Count);
                }
                if (times % 3 == 0)
                {
                    index++;
                }
                times++; if (times == 100000) { times = 1; }
                if (index == Slave.Count)//2
                {
                    index = 0;
                }
                ConnBean slaveBean = Slave[index];
                if (slaveBean.IsOK)//�����������򷵻ء�
                {
                    return slaveBean;
                }
                else if (Slave.Count > 1)
                {
                    //int i = index + 1;//����һ�¸�����
                    for (int i = index + 1; i < Slave.Count + 1; i++)
                    {
                        if (i == Slave.Count)
                        {
                            i = 0;
                        }
                        if (i == index) { break; }
                        if (Slave[i].IsOK)
                        {
                            return Slave[i];
                        }
                    }
                }
                //��ȫ�����ˣ�������
                if (Master != null && Master.IsOK)
                {
                    return Master;
                }
                else if (BackUp != null && BackUp.IsOK)
                {
                    return BackUp;
                }
            }
            return null;
        }

        public void SetNotAllowSlave()
        {
            if (Slave.Count > 0)
            {
                string id = GetIdentity();//��ȡ��ǰ�ı�ʶ
                Cache.CacheManage.LocalInstance.Set(id, 1, AppConfig.DB.MasterSlaveTime / 60.0);
            }
        }
        public bool IsAllowSlave()
        {
            if (Slave.Count == 0) { return false; }
            string id = GetIdentity();//��ȡ��ǰ�ı�ʶ
            return !Cache.CacheManage.LocalInstance.Contains(id);
        }
        private string GetIdentity()
        {
            string id = string.Empty;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    id = HttpContext.Current.Session.SessionID;
                }
                else if (HttpContext.Current.Request["MasterSlaveID"] != null)
                {
                    id = HttpContext.Current.Request["MasterSlaveID"];
                }
                if (string.IsNullOrEmpty(id))
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies["MasterSlaveID"];
                    if (cookie != null)
                    {
                        id = cookie.Value;
                    }
                    else
                    {
                        id = Guid.NewGuid().ToString().Replace("-", "");
                        cookie = new HttpCookie("MasterSlaveID", id);
                        cookie.Expires = DateTime.Now.AddMonths(1);
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                }
            }
            if (string.IsNullOrEmpty(id))
            {
                id = DateTime.Now.Minute + Thread.CurrentThread.ManagedThreadId.ToString();
            }
            return "MasterSlave_" + id;
        }
    }
    /*
    /// <summary>
    /// �洢��������
    /// </summary>
    internal class ConnEntity
    {
        private string _Conn = string.Empty;

        public string Conn
        {
            get { return _Conn; }
            set { _Conn = value; }
        }
        private string _ProviderName;

        public string ProviderName
        {
            get { return _ProviderName; }
            set { _ProviderName = value; }
        }
        private DalType _ConnDalType;

        public DalType ConnDalType
        {
            get { return _ConnDalType; }
            set { _ConnDalType = value; }
        }

        private string _ConnBak = string.Empty;

        public string ConnBak
        {
            get { return _ConnBak; }
            set { _ConnBak = value; }
        }
        private string _ProviderNameBak;

        public string ProviderNameBak
        {
            get { return _ProviderNameBak; }
            set { _ProviderNameBak = value; }
        }

        private DalType _ConnBakDalType;

        public DalType ConnBakDalType
        {
            get { return _ConnBakDalType; }
            set { _ConnBakDalType = value; }
        }
        /// <summary>
        /// ������������
        /// </summary>
        /// <returns></returns>
        public bool ExchangeConn()
        {
            if (!string.IsNullOrEmpty(_ConnBak))
            {
                string temp = _Conn;
                _Conn = _ConnBak;
                _ConnBak = temp;

                temp = _ProviderName;
                _ProviderName = _ProviderNameBak;
                _ProviderNameBak = temp;

                DalType tempDal = _ConnDalType;
                _ConnDalType = _ConnBakDalType;
                _ConnBakDalType = tempDal;

                return true;
            }
            return false;
        }
    }

    //internal class ConnFactory
    //{
    //    private static Dictionary<string, ConnEntity> connDic = new Dictionary<string, ConnEntity>(StringComparer.OrdinalIgnoreCase);
    //    public ConnEntity Get(string connName)
    //    {
    //        if (!connDic.ContainsKey(connName))
    //        {
    //            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
    //            {
    //                if (!connDic.ContainsKey(item.Name))
    //                {
    //                    ConnEntity entity = new ConnEntity();
    //                    entity.Conn = item.ConnectionString;
    //                    entity.s
    //                    connDic.Add(item.Name, entity);
    //                }
    //            }

    //        }
    //        if (connDic.ContainsKey(connName))
    //        {
    //            return connDic[connName];
    //        }
    //        return null;
    //    }
    //}
     */
}
