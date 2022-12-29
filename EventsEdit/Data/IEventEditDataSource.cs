using System.Collections.Generic;
using System.Threading.Tasks;
using SgupsPlanner.Core.ResponseWrapper;
using SgupsPlanner.Core.ViewModels;
using SgupsPlanner.Database.Entities;

namespace EventsEdit.Data
{
    internal interface IEventEditDataSource
    {
        Task<EventDto> GetEventById(int eventId);
        Task<List<EventDto>> GetEvents();
        Task<bool> UpdateEvent(EventDto eventDto);
        Task<bool> CreateEvent(EventDto eventDto);
        Task<bool> DeleteEvent(int eventId);
    }
}