using System;

namespace SgupsPlanner.Database.Entities
{
    public class FileDto
    {
        public int Id { get; }
        public int EventId { get; }
        public string FileName { get; }
        public DateTime CreateDate { get; }

        public FileDto(int id, int eventId, string fileName, DateTime date)
        {
            Id = id;
            EventId = eventId;
            FileName = fileName;
            CreateDate = date;
        }

        public FileDto()
        {
        }

        #region Overrides of Object

        public override string ToString()
        {
            return FileName;
        }

        #endregion
    }
}
