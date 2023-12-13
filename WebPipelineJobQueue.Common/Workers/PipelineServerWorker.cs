using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WebPipelineJobQueue.Common.Extensions;
using WebPipelineJobQueue.Common.Helpers;
using WebPipelineJobQueue.Common.Items;
using WebPipelineJobQueue.Common.Workers.Base;

namespace WebPipelineJobQueue.Common.Workers
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class PipelineServerWorker : WorkerBase
    {
        private readonly string pipeName;
        private readonly PipeSecurity pipeSecurity;

        public PipelineServerWorker(WorkerParameterItem myParams, string pipeName) : base(0, myParams)
        {
            this.pipeName = pipeName;

            pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite | PipeAccessRights.Synchronize, System.Security.AccessControl.AccessControlType.Allow));
        }

        protected override Task RunProcess()
        {
            return Task.Run(() =>
            {
                while (!continuationToken.IsCancellationRequested)
                {
                    GetMessageAndResponse();
                }
            }, continuationToken.Token);
        }

        private void GetMessageAndResponse()
        {
            try
            {
                using (var pipeServer = new NamedPipeServerStream(pipeName,
                   PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, pipeSecurity))
                {
                    pipeServer.WaitForConnection();

                    var readBuffer = new byte[2048];
                    pipeServer.Read(readBuffer, 0, readBuffer.Length);

                    var message = Encoding.UTF8.GetString(readBuffer).TrimEnd((char)0).Trim();

                    var messageObject = message.Deserialize<CommunicationItem>();

                    var response = QueueTaskWithReturnTypeSync(message).Serialize();

                    var writeBuffer = Encoding.UTF8.GetBytes(response);

                    pipeServer.Write(writeBuffer, 0, writeBuffer.Length);

                    pipeServer.Flush();
                    pipeServer.Close();
                }
            }
            catch (SocketException) { }
            catch (IOException) { }
            catch (Exception ex)
            {
                LogHelper.Instance.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, exceptionId: 7005);
            }
        }
    }
}
