using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SgupsPlanner.Core.ViewModels;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.DtoMappers
{
    public static class EventMapper
    {
        public static EventDto ConvertToDto(EventObservable vm)
        {
            return new EventDto(
                vm.EventId,
                vm.EventName,
                vm.Description,
                vm.IsActive,
                vm.IsRepeatable,
                vm.DeadlineDate,
                vm.StartNotifyDate,
                vm.RepeatInterval,
                vm.CreateDate,
                FileMapper.ConvertToListDto(vm.Files.ToList()).ToList()
            );
        }

        public static EventObservable ConvertFromDto(EventDto dto)
        {
            return new EventObservable(dto.EventName, dto.Description, dto.IsActive, dto.IsRepeatable, dto.DeadlineDate,
                dto.CreateDate, dto.StartNotifyDate, dto.RepeatInterval,
                new ObservableCollection<FileObservable>(FileMapper.ConvertFromListDto(dto.Files)));
        }

        public static IEnumerable<EventDto> ConvertToListDto(List<EventObservable> observables)
        {
            return observables?.Select(ConvertToDto);
        }

        public static IEnumerable<EventObservable> ConvertFromListDto(List<EventDto> dtos)
        {
            return dtos?.Select(ConvertFromDto);
        }
    }
}
