using MyTrip.MyTripLogic.Models;

namespace MediaConverter.Converters
{
    interface IConverter
    {
        void ConvertData(QueueMessage msg);
    }
}
