using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace MediaConverter.Models
{
    public class QueueMessage
    {
        public string routeId { get; set; }
        public QueueTaskType taskType { get; set; }
        public string tripId { get; set; }
        public string url { get; set; }


        public static QueueMessage DeserializeMessage(CloudQueueMessage msg)
        {
            return new JavaScriptSerializer().Deserialize<QueueMessage>(msg.AsString);
        }

        public static string SerializeMessage(QueueMessage qmsg)
        {
            return new JavaScriptSerializer().Serialize(qmsg);
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
