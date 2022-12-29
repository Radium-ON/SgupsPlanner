using System.Collections.Generic;
using System.Threading.Tasks;
using SgupsPlanner.Database.Database;
using SgupsPlanner.Database.Entities;
using SgupsPlanner.Database.Interfaces;

namespace EventsEdit.Data
{
    public class EventEditLocalDataSource : IEventEditDataSource
    {
        private readonly IDataBaseEvent<EventDto, bool> dataBaseEvent;
        private readonly IDataBaseFile<FileDto, bool> dataBaseFile;

        public EventEditLocalDataSource()
        {
            dataBaseEvent = new DataBaseEvent();
            dataBaseFile = new DataBaseFile();
        }
        public Task<EventDto> GetEventById(int eventId)
        {
            return dataBaseEvent.SelectByIdAsync(eventId,true);
        }

        public Task<List<EventDto>> GetEvents()
        {
            return dataBaseEvent.SelectAllAsync(true);
        }

        public Task<bool> UpdateEvent(EventDto eventDto)
        {
            return dataBaseEvent.UpdateAsync(eventDto);
        }

        public Task<bool> CreateEvent(EventDto eventDto)
        {
            return dataBaseEvent.CreateAsync(eventDto);
        }

        public Task<bool> DeleteEvent(int eventId)
        {
            return dataBaseEvent.DeleteAsync(eventId);
        }
    }
}