using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SgupsPlanner.Core.DtoMappers;
using SgupsPlanner.Core.ResponseWrapper;
using SgupsPlanner.Core.ViewModels;

namespace EventsEdit.Data
{
    public class EventEditRepository : IEventEditRepository
    {
        private readonly IEventEditDataSource _localDataSource;

        public EventEditRepository()
        {
            _localDataSource = new EventEditLocalDataSource();
        }
        public async Task<IResponseWrapper> GetAllEvents()
        {
            try
            {
                var response = await _localDataSource.GetEvents();
                var list = EventMapper.ConvertFromListDto(response).ToList();
                return new Success<List<EventObservable>>(list);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[EventEditRepository.GetEvents()] Error: " + e.Message);
                return new Error(e.Message);
            }
        }

        public async Task<IResponseWrapper> GetEventById(int eventId)
        {
            try
            {
                var response = await _localDataSource.GetEventById(eventId);
                return new Success<EventObservable>(EventMapper.ConvertFromDto(response));
            }
            catch (Exception e)
            {
                Debug.WriteLine("[EventEditRepository.GetEventById()] Error: " + e.Message);
                return new Error(e.Message);
            }
        }

        public async Task<IResponseWrapper> CreateEvent(EventObservable eventObservable)
        {
            try
            {
                var response = await _localDataSource.CreateEvent(EventMapper.ConvertToDto(eventObservable));
                return new Success<bool>(response);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[EventEditRepository.CreateEvent()] Error: " + e.Message);
                return new Error(e.Message);
            }
        }

        public async Task<IResponseWrapper> DeleteEvent(int eventId)
        {
            try
            {
                var response = await _localDataSource.DeleteEvent(eventId);
                return new Success<bool>(response);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[EventEditRepository.DeleteEvent()] Error: " + e.Message);
                return new Error(e.Message);
            }
        }

        public async Task<IResponseWrapper> UpdateEvent(EventObservable eventObservable)
        {
            try
            {
                var response = await _localDataSource.UpdateEvent(EventMapper.ConvertToDto(eventObservable));
                return new Success<bool>(response);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[EventEditRepository.UpdateEvent()] Error: " + e.Message);
                return new Error(e.Message);
            }
        }
    }
}