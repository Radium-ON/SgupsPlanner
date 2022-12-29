using System;
using System.Collections.Generic;

namespace SgupsPlanner.Database.Entities
{
    public class EventDto
    {
        public int Id { get; }
        public string EventName { get; }
        public string Description { get; }
        public bool IsActive { get; }
        public bool IsRepeatable { get; }
        public DateTime DeadlineDate { get; }
        public DateTime StartNotifyDate { get; }
        public RepeatInterval RepeatInterval { get; }
        public DateTime CreateDate { get; }

        public List<FileDto> Files { get; private set; }

        public EventDto(string eventName, string description, bool isActive, bool isRepeatable, DateTime deadlineDate, DateTime startNotifyDate, RepeatInterval repeatInterval, DateTime createDate)
        {
            EventName = eventName;
            Description = description;
            IsActive = isActive;
            IsRepeatable = isRepeatable;
            DeadlineDate = deadlineDate;
            StartNotifyDate = startNotifyDate;
            RepeatInterval = repeatInterval;
            CreateDate = createDate;
        }

        public EventDto(int id, string eventName, string description, bool isActive, bool isRepeatable, DateTime deadlineDate, DateTime startNotifyDate, RepeatInterval repeatInterval, DateTime createDate) : this(eventName, description, isActive, isRepeatable, deadlineDate, startNotifyDate, repeatInterval, createDate)
        {
            Id = id;
        }

        public EventDto(int id, string eventName, string description, bool isActive, bool isRepeatable, DateTime deadlineDate, DateTime startNotifyDate, RepeatInterval repeatInterval, DateTime createDate, List<FileDto> files) : this(id, eventName, description, isActive, isRepeatable, deadlineDate, startNotifyDate, repeatInterval, createDate)
        {
            Files = files;
        }

        public EventDto()
        {
        }

        public void SetFiles(List<FileDto> files)
        {
            Files = files;
        }

        public override string ToString()
        {
            return EventName + " " + Description + " " + DeadlineDate.Date;
        }
    }
}
