using System.Threading.Tasks;
using SgupsPlanner.Core.ResponseWrapper;
using SgupsPlanner.Core.ViewModels;

namespace EventsEdit.Data
{
    internal interface IEventEditRepository
    {
        Task<IResponseWrapper> GetAllEvents();
        Task<IResponseWrapper> GetEventById(int eventId);
        Task<IResponseWrapper> CreateEvent(EventObservable eventObservable);
        Task<IResponseWrapper> DeleteEvent(int eventId);
        Task<IResponseWrapper> UpdateEvent(EventObservable eventObservable);



    }
}