using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebPipelineJobQueue.Common.CommonConstants;
using WebPipelineJobQueue.Common.Extensions;
using WebPipelineJobQueue.Common.Generic;
using WebPipelineJobQueue.Common.Items;

namespace WebPipelineJobQueue.Common.Helpers
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class PipelineHelper : Singleton<PipelineHelper>
    {
        public CommunicationItem SendSync(CommunicationItem message, string pipeName, bool isPingRequest = false)
        {
            try
            {
                using (var namedPipeClient = new NamedPipeClientStream(pipeName))
                {
                    namedPipeClient.Connect(Constants.PipeConnectTimeout);

                    var messageCrypted = message.Serialize();

                    var buffer = Encoding.UTF8.GetBytes(messageCrypted);

                    var cToken = new CancellationTokenSource();

                    cToken.CancelAfter(TimeSpan.FromMilliseconds(Constants.PipeWriteTimeout));

                    Task.Run(async () =>
                    {
                        await namedPipeClient.WriteAsync(buffer, 0, buffer.Length);
                    }).Wait(cToken.Token);

                    cToken = new CancellationTokenSource();

                    cToken.CancelAfter(TimeSpan.FromMilliseconds(Constants.PipeWriteTimeout));

                    var readBuffer = new byte[2048];

                    if (isPingRequest)
                    {
                        cToken = new CancellationTokenSource();

                        cToken.CancelAfter(TimeSpan.FromMilliseconds(Constants.PipeWriteTimeout));

                        Task.Run(async () =>
                        {
                            await namedPipeClient.ReadAsync(readBuffer, 0, readBuffer.Length);
                        }).Wait(cToken.Token);
                    }
                    else
                    {
                        namedPipeClient.Read(readBuffer, 0, readBuffer.Length);
                    }

                    namedPipeClient.WaitForPipeDrain();
                    namedPipeClient.Close();

                    var cObject = Encoding.UTF8.GetString(readBuffer).TrimEnd((char)0).Trim();

                    return cObject.Deserialize<CommunicationItem>();
                }
            }
            catch (TimeoutException) { }
            catch (SocketException) { }
            catch (IOException) { }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                LogHelper.Instance.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, exceptionId: 0);
            }

            return null;
        }
    }
}
