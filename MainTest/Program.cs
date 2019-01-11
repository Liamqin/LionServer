using System;
using System.Threading;

namespace MainTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //普通模式
            var csredis = new CSRedis.CSRedisClient(null,
                  "127.0.0.1:6379,password=,defaultDatabase=11,poolsize=10,ssl=false,writeBuffer=10240,prefix=key01",
                  "123.149.255.4:6379,password=123456,defaultDatabase=12,poolsize=11,ssl=false,writeBuffer=10240,prefix=key2554",
                  "47.93.59.199:6379,password=redis123,defaultDatabase=13,poolsize=12,ssl=false,writeBuffer=10240,prefix=key199");
            //初始化 RedisHelper
            RedisHelper.Initialization(csredis);
            //Install-Package Caching.CSRedis (本篇不需要) 
            //注册mvc分布式缓存
            //services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));
            Test();
            Console.ReadKey();
        }

        static void Test()
        {

            RedisHelper.Set("name", "祝雷");//设置值。默认永不过期
            //RedisHelper.SetAsync("name", "祝雷");//异步操作
            Console.WriteLine(RedisHelper.Get<String>("name"));

            RedisHelper.Set("time", DateTime.Now, 1);
            Console.WriteLine(RedisHelper.Get<DateTime>("time"));
            Thread.Sleep(1100);
            Console.WriteLine(RedisHelper.Get<DateTime>("time"));

            // 列表
            RedisHelper.RPush("list", "第一个元素");
            RedisHelper.RPush("list", "第二个元素");
            RedisHelper.LInsertBefore("list", "第二个元素", "我是新插入的第二个元素！");
            Console.WriteLine($"list的长度为{RedisHelper.LLen("list")}");
            //Console.WriteLine($"list的长度为{RedisHelper.LLenAsync("list")}");//异步
            Console.WriteLine($"list的第二个元素为{RedisHelper.LIndex("list", 1)}");
            //Console.WriteLine($"list的第二个元素为{RedisHelper.LIndexAsync("list",1)}");//异步
            // 哈希
            RedisHelper.HSet("person", "name", "zhulei");
            RedisHelper.HSet("person", "sex", "男");
            RedisHelper.HSet("person", "age", "28");
            RedisHelper.HSet("person", "adress", "hefei");
            Console.WriteLine($"person这个哈希中的age为{RedisHelper.HGet<int>("person", "age")}");
            //Console.WriteLine($"person这个哈希中的age为{RedisHelper.HGetAsync<int>("person", "age")}");//异步


            // 集合
            RedisHelper.SAdd("students", "zhangsan", "lisi");
            RedisHelper.SAdd("students", "wangwu");
            RedisHelper.SAdd("students", "zhaoliu");
            Console.WriteLine($"students这个集合的大小为{RedisHelper.SCard("students")}");
            Console.WriteLine($"students这个集合是否包含wagnwu:{RedisHelper.SIsMember("students", "wangwu")}");
        }
    }
}
