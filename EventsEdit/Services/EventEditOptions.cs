using System.Threading.Tasks;
using EventsEdit.Data;
using SgupsPlanner.Core.ResponseWrapper;
using SgupsPlanner.Core.ViewModels;

namespace EventsEdit.Services
{
    public class EventEditOptions
    {
        private readonly IEventEditRepository _repository;

        public EventEditOptions()
        {
            _repository = new EventEditRepository();
        }
        public async Task<IResponseWrapper> GetEventById(int eventId)
        {
            return await Task.Run(() => _repository.GetEventById(eventId));
        }
        public async Task<IResponseWrapper> GetAllEvents()
        {
            return await Task.Run(() => _repository.GetAllEvents());
        }
        public async Task<IResponseWrapper> CreateEvent(EventObservable eo)
        {
            return await Task.Run(() => _repository.CreateEvent(eo));
        }
        public async Task<IResponseWrapper> UpdateEvent(EventObservable eo)
        {
            return await Task.Run(() => _repository.UpdateEvent(eo));
        }
        public async Task<IResponseWrapper> DeleteEvent(int eventId)
        {
            return await Task.Run(() => _repository.DeleteEvent(eventId));
        }
    }
}