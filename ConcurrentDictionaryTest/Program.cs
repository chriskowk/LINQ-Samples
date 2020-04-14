using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ConcurrentDictionaryTest
{
    class Program
    {
        static Dictionary<string, ILog> _loggerDic;
        static object loggerDicLocker = new object();
        static void Main(string[] args)
        {
            //RunNormalDictionary();
            //RunConcurrentOrLockDictionary();
            DictionaryHowTo.DoTasks();
        }

        private static void RunNormalDictionary()
        {
            _loggerDic = new Dictionary<string, ILog>();
            for (int i = 0; i < 1000; i++)
            {
                ThreadPool.QueueUserWorkItem(a =>
                {
                    try
                    {
                        var logger = GetLogger("AAA");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("弟{0}个线程出现问题：{1}", a, ex.Message));
                    }

                }, i);
            }

            Console.ReadKey();
        }

        static ILog GetLogger(string cmdId)
        {
            if (!_loggerDic.ContainsKey(cmdId))
            {
                _loggerDic.Add(cmdId, LogManager.GetLogger(string.Format("ChinaPnrApi.{0}", cmdId)));
            }
            return _loggerDic[cmdId];
        }

        static void RunConcurrentOrLockDictionary()
        {
            IGetLogger conLogger = new ConcurrentDictionaryLogger();

            IGetLogger lockerLogger = new LockerDictionaryLogger();

            CodeTimer.Time("使用ConcurrentDictionary", 1000000, () =>
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        var logger = conLogger.GetLogger("AAA");
                        if (logger == null)
                        {
                            Console.WriteLine(string.Format("弟{0}个线程获取到的值是 NULL", o));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("弟{0}个线程出现问题, {1}", o, ex.Message));
                    }
                });
            });

            CodeTimer.Time("使用LockDictionary", 1000000, () =>
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        var logger = lockerLogger.GetLogger("AAA");
                        if (logger == null)
                        {
                            Console.WriteLine(string.Format("弟{0}个线程获取到的值是 NULL", o));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("弟{0}个线程出现问题, {1}", o, ex.Message));
                    }
                });
            });

            Console.WriteLine("已执行完成");
            Console.ReadKey();
        }
    }
}
