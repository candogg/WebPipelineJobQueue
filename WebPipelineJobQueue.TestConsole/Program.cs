using System;
using System.Threading;
using WebPipelineJobQueue.Common.Enumerations;
using WebPipelineJobQueue.Common.Extensions;
using WebPipelineJobQueue.Common.Items;
using WebPipelineJobQueue.Common.Workers;

namespace WebPipelineJobQueue.TestConsole
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    internal class Program
    {
        private readonly static ManualResetEvent oExitEvent;
        private readonly static PipelineServerWorker oMessageServer;

        static Program()
        {
            oExitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (ss, ee) =>
            {
                ee.Cancel = true;
                oExitEvent.Set();
            };

            oMessageServer = new PipelineServerWorker(new WorkerParameterItem(callbackWithReturnType: MessageReceivedCallback), "CanoTestPipe2");
            oMessageServer.StartOperation();
        }

        static object MessageReceivedCallback(object message)
        {
            var responseObject = new CommunicationItem() { MessageType = MessageTypesEnum.None };

            var communicationObject = message.Deserialize<CommunicationItem>();

            if (communicationObject == null)
            {
                return responseObject;
            }

            switch (communicationObject.MessageType)
            {
                case MessageTypesEnum.Test:
                    {
                        responseObject.AdditionalMessage = "Hello Can from D2MService";

                        ///START
                        ///
                        /// Burada gerekli işlemler yapılacak
                        /// 
                        ///END

                        break;
                    }
            }

            Console.WriteLine($"{DateTime.Now}: {communicationObject.AdditionalMessage}");

            return responseObject;
        }

        static void Main()
        {
            Console.WriteLine("Started...");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Waiting for Incoming Messages...");
            Console.ResetColor();



            oExitEvent.WaitOne();
            Console.Write("Finished... Press any key to exit");
            Console.ReadKey();
        }
    }
}
