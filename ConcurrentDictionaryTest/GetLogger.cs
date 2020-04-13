using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentDictionaryTest
{
    public interface IGetLogger
    {
        ILog GetLogger(string cmdId);
    }

    public class ConcurrentDictionaryLogger : IGetLogger
    {
        readonly ConcurrentDictionary<string, ILog> _loggerDic = new ConcurrentDictionary<string, ILog>();
        public ILog GetLogger(string cmdId)
        {
            if (!_loggerDic.ContainsKey(cmdId))
            {
                _loggerDic.TryAdd(cmdId, LogManager.GetLogger(string.Format("ChinaPnrApi.{0}", cmdId)));
            }
            return _loggerDic[cmdId];
        }
    }

    public class LockerDictionaryLogger : IGetLogger
    {
        readonly Dictionary<string, ILog> _loggerDic = new Dictionary<string, ILog>();
        object locker = new object();
        public ILog GetLogger(string cmdId)
        {
            lock (locker)
            {
                if (!_loggerDic.ContainsKey(cmdId))
                {
                    _loggerDic.Add(cmdId, LogManager.GetLogger(string.Format("ChinaPnrApi.{0}", cmdId)));
                }
            }
            return _loggerDic[cmdId];
        }
    }
}
