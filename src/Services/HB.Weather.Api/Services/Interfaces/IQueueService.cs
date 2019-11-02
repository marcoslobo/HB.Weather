

using Infra.Dto;
using System.Threading.Tasks;

namespace HB.Weather.Api.Services
{
    public interface IQueueService
    {
        Task PublishMessage(string cityAndCountry);

    }
}
