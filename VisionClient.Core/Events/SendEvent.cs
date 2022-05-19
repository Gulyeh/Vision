using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Models;

namespace VisionClient.Core.Events
{
    public class SendEvent<T> : PubSubEvent<T>
    {
    }
}
