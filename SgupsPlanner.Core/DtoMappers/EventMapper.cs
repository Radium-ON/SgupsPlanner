using System.Collections.Generic;
using System.Linq;
using SgupsPlanner.Core.ViewModels;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.DtoMappers
{
    public static class EventMapper
    {
        public static EventDto ConvertToDto(EventViewModel vm)
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
                vm.CreateDate,vm.Files.ToList()
            );
        }

        public static EventViewModel ConvertFromDto(EventDto dto)
        {
            return new EventViewModel(dto);
        }

        public static IEnumerable<EventDto> ConvertToListDto(List<EventViewModel> observables)
        {
            return observables?.Select(ConvertToDto);
        }

        public static IEnumerable<EventViewModel> ConvertFromListDto(List<EventDto> dtos)
        {
            return dtos?.Select(ConvertFromDto);
        }
    }
}
