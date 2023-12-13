using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebPipelineJobQueue.Common.Items;

namespace WebPipelineJobQueue.Common.Workers.Base
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public abstract class WorkerBase
    {
        protected Task myTask;
        protected CancellationTokenSource continuationToken;
        protected int interval;
        protected WorkerParameterItem myParams;
        protected bool reStarted;

        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                if (!interval.Equals(value))
                {
                    continuationToken?.Cancel();

                    Task.WhenAll(new Task[] { myTask }.Where(x => x != null)).ContinueWith(_ =>
                    {
                        continuationToken = new CancellationTokenSource();

                        myTask = null;

                        interval = value;
                    }).GetAwaiter().GetResult();

                    StartOperation(true);
                }
            }
        }

        protected WorkerBase(int interval, WorkerParameterItem myParams)
        {
            continuationToken = new CancellationTokenSource();

            this.interval = interval;
            this.myParams = myParams;
            reStarted = false;
        }

        public virtual void StopOperation()
        {
            if (!continuationToken.IsCancellationRequested)
                continuationToken?.Cancel();

            Task.WhenAll(new Task[] { myTask }.Where(x => x != null)).ContinueWith(_ =>
            {
                continuationToken = new CancellationTokenSource();

                myTask = null;
            }).GetAwaiter().GetResult();
        }

        public virtual void StartOperation(bool reStarted = false)
        {
            if (myTask != null) return;

            this.reStarted = reStarted;

            myTask = RunProcess();
        }

        protected virtual Task RunProcess()
        {
            return Task.FromResult(0);
        }

        protected void QueueTaskAsync(object data = null)
        {
            Task.Run(() =>
            {
                myParams.Callback?.Invoke(data);
            });
        }

        protected void QueueTaskSync(object data = null)
        {
            myParams.Callback?.Invoke(data);
        }

        protected object QueueTaskWithReturnTypeSync(object data = null)
        {
            return myParams.CallbackWithReturnType?.Invoke(data);
        }

        protected Task<object> QueueTaskWithReturnTypeAsync(object data = null)
        {
            return Task.Run(() =>
            {
                return myParams.CallbackWithReturnType?.Invoke(data);
            });
        }
    }
}
