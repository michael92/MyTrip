using MyTrip.MyTripLogic.Models;

namespace MediaGenerator.Generators
{
    interface IGenerate
    {
        void GenerateData(QueueMessage msg);
    }
}
