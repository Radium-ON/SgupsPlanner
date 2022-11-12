using System;
using System.Windows;
using System.Collections.ObjectModel;
using SgupsPlanner.Database;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.ViewModels
{
    public class EventViewModel : ValidatedViewModel<EventDto>
    {

        public EventViewModel(EventDto eventDto) : base(eventDto)
        {
            ValidatedProperties = new[]
            {
                "EventName",
                "Description",
                "IsActive",
                "IsRepeatable",
                "DeadlineDate",
                "StartNotifyDate",
                "RepeatInterval",
                "CreateDate"
            };
        }

        #region Entity Properties

        public int EventId => entity.Id;

        public string EventName
        {
            get => entity.EventName;
            set
            {
                if (value == entity.EventName)
                {
                    return;
                }

                entity.EventName = value;

                RaisePropertyChanged(nameof(EventName));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string Description
        {
            get => entity.Description;
            set
            {
                if (value == entity.Description)
                {
                    return;
                }

                entity.Description = value;

                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public bool IsActive
        {
            get => entity.IsActive;
            set
            {
                if (value == entity.IsActive)
                {
                    return;
                }

                entity.IsActive = value;

                RaisePropertyChanged(nameof(IsActive));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public bool IsRepeatable
        {
            get => entity.IsRepeatable;
            set
            {
                if (value == entity.IsRepeatable)
                {
                    return;
                }

                entity.IsRepeatable = value;

                RaisePropertyChanged(nameof(IsRepeatable));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public DateTime DeadlineDate
        {
            get => entity.DeadlineDate;
            set
            {
                if (value == entity.DeadlineDate)
                {
                    return;
                }

                entity.DeadlineDate = value;

                RaisePropertyChanged(nameof(DeadlineDate));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public DateTime CreateDate
        {
            get => entity.CreateDate;
            set
            {
                if (value == entity.CreateDate)
                {
                    return;
                }

                entity.CreateDate = value;

                RaisePropertyChanged(nameof(CreateDate));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public DateTime StartNotifyDate
        {
            get => entity.StartNotifyDate;
            set
            {
                if (value == entity.StartNotifyDate)
                {
                    return;
                }

                entity.StartNotifyDate = value;

                RaisePropertyChanged(nameof(StartNotifyDate));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public RepeatInterval RepeatInterval
        {
            get => entity.RepeatInterval;
            set
            {
                if (value == entity.RepeatInterval)
                {
                    return;
                }

                entity.RepeatInterval = value;

                RaisePropertyChanged(nameof(RepeatInterval));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public ObservableCollection<FileDto> Files => new ObservableCollection<FileDto>(entity.Files);
        #endregion
        
        #region Validation

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "EventName" => ValidateEventName(),
                "Description" => ValidateDescription(),
                "DeadlineDate" => ValidateDeadline(),
                "StartNotifyDate" => ValidateStartNotifyDate(),
                "RepeatInterval" => ValidateRepeatInterval(),
                _ => null
            };

            return error;
        }

        private string ValidateRepeatInterval()
        {
            if (!IsRepeatable)
            {
                return Application.Current.FindResource("client_error_missing_age") as string;
            }
            return null;
        }

        private string ValidateStartNotifyDate()
        {
            if (DateTime.Compare(StartNotifyDate, DeadlineDate) > 0)
            {
                return Application.Current.FindResource("client_error_missing_tin") as string;
            }
            return null;
        }

        private string ValidateDeadline()
        {
            if (DateTime.Compare(DeadlineDate,DateTime.Now)<0)
            {
                return Application.Current.FindResource("client_error_missing_passport") as string;
            }
            return null;
        }

        private string ValidateEventName()
        {
            if (IsStringMissing(EventName))
            {
                return Application.Current.FindResource("client_error_missing_first_name") as string;
            }
            return null;
        }

        private string ValidateDescription()
        {

            if (IsStringMissing(Description))
                return Application.Current.FindResource("client_error_missing_lastname") as string;

            return null;
        }

        #endregion
    }
}
