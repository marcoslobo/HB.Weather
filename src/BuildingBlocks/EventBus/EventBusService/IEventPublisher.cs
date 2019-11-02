using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HB.Weather.BuildingBlocks.EventBus
{
    public interface IEventPublisher
    {
        Task PublishMessage<T>(T msg);
    }
}
