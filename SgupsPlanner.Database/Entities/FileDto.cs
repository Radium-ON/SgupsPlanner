using System;

namespace SgupsPlanner.Database.Entities
{
    public class FileDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string FileName { get; set; }
        public DateTime CreateDate { get; set; }

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
