using System.Collections.Generic;
using System.Linq;
using SgupsPlanner.Core.ViewModels;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.DtoMappers
{
    public static class FileMapper
    {
        public static FileDto ConvertToDto(FileViewModel observable)
        {
            return new FileDto(observable.FileId,observable.EventId,observable.FileName,observable.CreateDate);
        }
        public static FileViewModel ConvertFromDto(FileDto dto)
        {
            return new FileViewModel(dto);
        }
        public static IEnumerable<FileDto> ConvertToListDto(List<FileViewModel> observables)
        {
            return observables?.Select(ConvertToDto);
        }
        public static IEnumerable<FileViewModel> ConvertFromListDto(List<FileDto> dtos)
        {
            return dtos?.Select(ConvertFromDto);
        }
    }
}