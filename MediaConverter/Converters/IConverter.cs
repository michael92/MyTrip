using MediaConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaConverter.Converters
{
    interface IConverter
    {
        void ConvertData(QueueMessage msg);
    }
}
