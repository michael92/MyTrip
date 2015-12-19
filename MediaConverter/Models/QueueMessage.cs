using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MediaConverter.Models
{
    public class QueueMessage
    {
        public int routeId { get; set; }
        public QueueTaskType taskType { get; set; }
        public int tripId { get; set; }
        public string url { get; set; }


        // Tutaj trzeba ustalic z innymi jaki format będzie miał string w CloudQueueMessage
        internal static QueueMessage ParseMessage(CloudQueueMessage msg)
        {
            throw new NotImplementedException();
            string unproccessedMsg = msg.AsString;
            //TODO: zamiana stringa w obiekt QueueMessage
            return new QueueMessage { };
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
