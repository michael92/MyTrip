using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MyTrip.MyTripLogic.Models
{
    public class QueueMessage
    {

        public string routeId { get; set; }
        public QueueTaskType taskType { get; set; }
        public string tripId { get; set; }
        public string url { get; set; }


        public static CloudQueueMessage SerializeMessage(QueueMessage msg)
        {
            String s =new JavaScriptSerializer().Serialize(msg);
            CloudQueueMessage m = new CloudQueueMessage(s);
            return m;
        }

        public static QueueMessage DeserializeMessage(CloudQueueMessage msg)
        {
            return new JavaScriptSerializer().Deserialize<QueueMessage>(msg.AsString);
        }

    }

    public enum QueueTaskType
    {
        ConvertPhoto,
        ConvertMovie,
        ConvertRoute,
        GeneratePoster,
        GenerateMovie
    }

}
