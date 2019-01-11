using System;
using System.Collections.Generic;
using System.Text;
using CYQ.Data.Cache;
using CYQ.Data.Table;
using CYQ.Data.Tool;

namespace CYQ.Data.Cache
{
    /// <summary>
    /// Redis�ֲ�ʽ������
    /// </summary>
    internal class RedisCache : CacheManage
    {
        RedisClient client;
        internal RedisCache()
        {
            if (string.IsNullOrEmpty(AppConfig.Cache.RedisServers))
            {
                Error.Throw("AppConfig.Cache.RedisServers cant' be Empty!");
            }
            client = RedisClient.Setup("RedisCache", AppConfig.Cache.RedisServers.Split(','));
            if (!string.IsNullOrEmpty(AppConfig.Cache.RedisServersBak))
            {
                RedisClient clientBak = RedisClient.Setup("RedisCacheBak", AppConfig.Cache.RedisServersBak.Split(','));
                client.serverPool.serverPoolBak = clientBak.serverPool;
            }


        }
        public override void Set(string key, object value)
        {
            Set(key, value, 60 * 24 * 30);
        }

        public override void Set(string key, object value, double cacheMinutes)
        {
            Set(key, value, cacheMinutes, null);
        }

        public override void Set(string key, object value, double cacheMinutes, string fileName)
        {
            client.Set(key, value, Convert.ToInt32(cacheMinutes * 60));
        }

        DateTime allowCacheTableTime = DateTime.Now;
        private MDataTable cacheTable = null;
        public override MDataTable CacheInfo
        {
            get
            {
                if (cacheTable == null || DateTime.Now > allowCacheTableTime)
                {
                    cacheTable = null;
                    cacheTable = new MDataTable();
                    Dictionary<string, Dictionary<string, string>> status = client.Stats();
                    if (status != null)
                    {

                        foreach (KeyValuePair<string, Dictionary<string, string>> item in status)
                        {
                            if (item.Value.Count > 0)
                            {
                                MDataTable dt = MDataTable.CreateFrom(item.Value);
                                if (cacheTable.Columns.Count == 0)//��һ��
                                {
                                    cacheTable = dt;
                                }
                                else
                                {
                                    cacheTable.JoinOnName = "Key";
                                    cacheTable = cacheTable.Join(dt, "Value");
                                }
                                cacheTable.Columns["Value"].ColumnName = item.Key;
                            }
                        }
                    }
                    cacheTable.TableName = "Redis";
                    allowCacheTableTime = DateTime.Now.AddMinutes(1);
                }
                return cacheTable;
            }
        }

        public override void Clear()
        {
            client.FlushAll();
        }

        public override bool Contains(string key)
        {
            return client.ContainsKey(key);
        }

        //int count = -1;
        //DateTime allowGetCountTime = DateTime.Now;
        public override int Count
        {
            get
            {
                int count = 0;
                IList<MDataRow> rows = CacheInfo.FindAll("Key like 'db%'");
                if (rows != null && rows.Count > 0)
                {
                    foreach (MDataRow row in rows)
                    {
                        for (int i = 1; i < row.Columns.Count; i++)
                        {
                            count += int.Parse(row[i].StringValue.Split(',')[0].Split('=')[1]);
                        }
                    }

                }
                return count;
            }
        }

        public override object Get(string key)
        {
            return client.Get(key);
        }


        public override void Remove(string key)
        {
            client.Delete(key);
        }


        DateTime allowGetWorkInfoTime = DateTime.Now;
        string workInfo = string.Empty;
        public override string WorkInfo
        {
            get
            {
                if (workInfo == string.Empty || DateTime.Now > allowGetWorkInfoTime)
                {
                    workInfo = null;
                    Dictionary<string, Dictionary<string, string>> status = client.Status();
                    if (status != null)
                    {
                        JsonHelper js = new JsonHelper(false, false);
                        js.Add("OKServerCount", client.okServer.ToString());
                        js.Add("DeadServerCount", client.errorServer.ToString());
                        foreach (KeyValuePair<string, Dictionary<string, string>> item in status)
                        {
                            js.Add(item.Key, JsonHelper.ToJson(item.Value));
                        }
                        js.AddBr();
                        workInfo = js.ToString();
                    }
                    allowGetWorkInfoTime = DateTime.Now.AddMinutes(5);
                }
                return workInfo;
            }
        }

        public override CacheType CacheType
        {
            get { return CacheType.Redis; }
        }
    }
}
