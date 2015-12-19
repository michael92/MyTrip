using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace MediaConverter.Models
{
    public class QueueMessage
    {
        public int routeId { get; set; }
        public QueueTaskType taskType { get; set; }
        public int tripId { get; set; }
        public string url { get; set; }


        public static QueueMessage DeserializeMessage(CloudQueueMessage msg)
        {
            int indexOf = msg.AsString.IndexOf(':');

            if (indexOf <= 0)
                throw new Exception(string.Format("Cannot deserialize into object of type {0}",
                    typeof(QueueMessage).FullName));

            string typeName = msg.AsString.Substring(0, indexOf);
            string json = msg.AsString.Substring(indexOf + 1);

            if (typeName != typeof(QueueMessage).FullName)
            {
                throw new Exception(string.Format("Cannot deserialize object of type {0} into one of type {1}",
                    typeName,
                    typeof(QueueMessage).FullName));
            }

            return JsonConvert.DeserializeObject<QueueMessage>(json);
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
