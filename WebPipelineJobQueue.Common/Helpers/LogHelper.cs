using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebPipelineJobQueue.Common.CommonConstants;
using WebPipelineJobQueue.Common.Extensions;
using WebPipelineJobQueue.Common.Generic;

namespace WebPipelineJobQueue.Common.Helpers
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class LogHelper : Singleton<LogHelper>
    {
        public void Write(string log, int exceptionId, EventLogEntryType eventType = EventLogEntryType.Error, object data = null, string methodName = "")
        {
            Task.Run(() =>
            {
                try
                {
                    CheckEventSource();

                    log += data != null ? $"{Environment.NewLine}{data.Serialize()}" : string.Empty;

                    log += $"{Environment.NewLine}MethodName: {methodName}";

                    EventLog.WriteEntry(Constants.EventSource, log, eventType, exceptionId);
                }
                catch
                { }
            });
        }

        public void CheckEventSource()
        {
            try
            {
                if (!EventLog.SourceExists(Constants.EventSource))
                {
                    EventLog.CreateEventSource(Constants.EventSource, Constants.EventLogName);
                }
            }
            catch
            { }
        }
    }
}
