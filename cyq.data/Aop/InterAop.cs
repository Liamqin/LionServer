using CYQ.Data.Cache;
using System;
using CYQ.Data.Table;

namespace CYQ.Data.Aop
{
    /// <summary>
    /// �ڲ�Ԥ��ʵ��CacheAop
    /// </summary>
    internal class InterAop
    {
        private CacheManage _Cache = CacheManage.LocalInstance;//Cache����
        // private AutoCache cacheAop = new AutoCache();
        private static readonly object lockObj = new object();
        internal bool isHasCache = false;
        public AopOp aopOp = AopOp.OpenAll;
        internal bool IsLoadAop
        {
            get
            {
                return aopOp != AopOp.CloseAll && (AppConfig.Cache.IsAutoCache || outerAop != null);
            }
        }
        internal bool IsTxtDataBase
        {
            get
            {
                return Para.DalType == DalType.Txt || Para.DalType == DalType.Xml;
            }
        }
        private AopInfo _AopInfo;
        /// <summary>
        /// Aop����
        /// </summary>
        public AopInfo Para
        {
            get
            {
                if (_AopInfo == null)
                {
                    _AopInfo = new AopInfo();
                }
                return _AopInfo;
            }
        }

        private IAop outerAop;
        public InterAop()
        {
            outerAop = GetFromConfig();
        }
        #region IAop ��Ա

        public AopResult Begin(AopEnum action)
        {
            AopResult ar = AopResult.Continue;
            if (outerAop != null && (aopOp == AopOp.OpenAll || aopOp == AopOp.OnlyOuter))
            {
                ar = outerAop.Begin(action, Para);
                if (ar == AopResult.Return)
                {
                    return ar;
                }
            }
            if (aopOp == AopOp.OpenAll || aopOp == AopOp.OnlyInner)
            {
                if (AppConfig.Cache.IsAutoCache && !IsTxtDataBase) // ֻҪ����ֱ�ӷ���
                {
                    isHasCache = AutoCache.GetCache(action, Para); //�ҿ���û��Cache
                }
                if (isHasCache)  //�ҵ�Cache
                {

                    if (outerAop == null || ar == AopResult.Default)//��ִ��End
                    {
                        return AopResult.Return;
                    }
                    return AopResult.Break;//�ⲿAop˵������Ҫִ��End
                }
            }
            return ar;// û��Cache��Ĭ�Ϸ���
        }

        public void End(AopEnum action)
        {
            if (outerAop != null && (aopOp == AopOp.OpenAll || aopOp == AopOp.OnlyOuter))
            {
                outerAop.End(action, Para);
            }
            if (aopOp == AopOp.OpenAll || aopOp == AopOp.OnlyInner)
            {
                if (!isHasCache && !IsTxtDataBase && Para.IsSuccess)//Select�ڲ�������GetCount��GetCount���ڲ�isHasCacheΪtrueӰ����
                {
                    AutoCache.SetCache(action, Para); //�ҿ���û��Cache
                }
            }
        }

        public void OnError(string msg)
        {
            if (outerAop != null)
            {
                outerAop.OnError(msg);
            }
        }

        #endregion
        static bool _IsLoadCompleted = false;
        private IAop GetFromConfig()
        {

            IAop aop = null;
            string aopApp = AppConfig.Aop;
            if (!string.IsNullOrEmpty(aopApp))
            {
                string key = "OuterAop_Instance";
                if (_Cache.Contains(key))
                {
                    aop = _Cache.Get(key) as IAop;
                }
                else
                {
                    #region AOP����

                    string[] aopItem = aopApp.Split(',');
                    if (aopItem.Length == 2)//��������,����(dll)����
                    {
                        if (!_IsLoadCompleted)
                        {
                            try
                            {
                                lock (lockObj)
                                {
                                    if (_IsLoadCompleted)
                                    {
                                        return GetFromConfig();//����ȥ�������á�
                                    }
                                    _IsLoadCompleted = true;
                                    System.Reflection.Assembly ass = System.Reflection.Assembly.Load(aopItem[1]);
                                    if (ass != null)
                                    {
                                        object instance = ass.CreateInstance(aopItem[0]);
                                        if (instance != null)
                                        {
                                            _Cache.Set(key, instance, 1440, AppConst.RunFolderPath + aopItem[1].Replace(".dll", "") + ".dll");
                                            aop = instance as IAop;
                                            aop.OnLoad();
                                        }
                                    }
                                }

                            }
                            catch (Exception err)
                            {
                                string errMsg = err.Message + "--Web.config need add a config item,for example:<add key=\"Aop\" value=\"Web.Aop.AopAction,Aop\" />(value format:namespace.Classname,Assembly name) ";
                                Error.Throw(errMsg);
                            }
                        }
                    }
                    #endregion
                }
            }
            if (aop != null)
            {
                return aop.Clone();
            }
            return null;
        }

        #region �ڲ�����
        //public static InterAop Instance
        //{
        //    get
        //    {
        //        return Shell.instance;
        //    }
        //}

        //class Shell
        //{
        //    internal static readonly InterAop instance = new InterAop();
        //}
        #endregion
    }
}
