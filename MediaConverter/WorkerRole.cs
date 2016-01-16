using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MediaConverter.Converters;
using MyTrip.MyTripLogic.Models;

namespace MediaConverter
{    
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private CloudQueue queue;

        public override void Run()
        {
            Trace.TraceInformation("MediaConverter is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            
            var storageAccount = CloudStorageAccount.Parse
                (RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            Trace.TraceInformation("Creating queue");
            var queueClient = storageAccount.CreateCloudQueueClient();
            this.queue = queueClient.GetQueueReference("converter");
            this.queue.CreateIfNotExists();

            bool result = base.OnStart();

            Trace.TraceInformation("MediaConverter has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("MediaConverter is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CloudQueueMessage msg = null;

            while (!cancellationToken.IsCancellationRequested) 
            {
                Trace.TraceInformation("Working");
                try
                {
                    msg = this.queue.GetMessage();

                    if (msg != null)
                    {
                        this.ProcessQueueMessage(msg); 
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    if (msg != null && msg.DequeueCount > 5)
                    {
                        this.queue.DeleteMessage(msg);
                        Trace.TraceError("Deleting MediaConverter queue item: '{0}'", msg.AsString);
                    }

                    Trace.TraceError("Exception in MediaConverter: '{0}'", e.Message);
                    System.Threading.Thread.Sleep(5000);
                }
                await Task.Delay(1000);
            }
        }


        private void ProcessQueueMessage(CloudQueueMessage msg)
        {
            Trace.TraceInformation("Processing queue message {0}", msg);
            QueueMessage parsedMsg = QueueMessage.DeserializeMessage(msg);

            IConverter converter = null;

            switch (parsedMsg.taskType)
            {
                case QueueTaskType.ConvertMovie:
                    converter = new MovieConverter();
                    break;
                case QueueTaskType.ConvertPhoto:
                    converter = new PhotoConverter();
                    break;
                case QueueTaskType.ConvertRoute:
                    converter = new RouteConverter(); 
                    break;
                default:
                    break;
            }

            if(converter != null)
            {
                converter.ConvertData(parsedMsg);
                this.queue.DeleteMessage(msg);
            }            
        }
    }
}
