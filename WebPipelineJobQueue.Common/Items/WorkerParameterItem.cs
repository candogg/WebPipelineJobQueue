using System;

namespace WebPipelineJobQueue.Common.Items
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class WorkerParameterItem
    {
        public Action<object> Callback { get; set; }
        public Func<object, object> CallbackWithReturnType { get; set; }

        public WorkerParameterItem(Action<object> callBack = null, Func<object, object> callbackWithReturnType = null)
        {
            Callback = callBack;
            CallbackWithReturnType = callbackWithReturnType;
        }
    }
}
