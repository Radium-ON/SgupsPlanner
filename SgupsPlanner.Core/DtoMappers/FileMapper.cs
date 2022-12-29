using System.Collections.Generic;
using System.Linq;
using SgupsPlanner.Core.ViewModels;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.DtoMappers
{
    public static class FileMapper
    {
        public static FileDto ConvertToDto(FileObservable observable)
        {
            return new FileDto(observable.FileId,observable.EventId,observable.FileName,observable.CreateDate);
        }
        public static FileObservable ConvertFromDto(FileDto dto)
        {
            return new FileObservable(dto.FileName,dto.CreateDate,dto.EventId);
        }
        public static IEnumerable<FileDto> ConvertToListDto(List<FileObservable> observables)
        {
            return observables?.Select(ConvertToDto);
        }
        public static IEnumerable<FileObservable> ConvertFromListDto(List<FileDto> dtos)
        {
            return dtos?.Select(ConvertFromDto);
        }
    }
}