using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using SgupsPlanner.Database;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.ViewModels
{
    public class EventObservable : BindableBase
    {
        #region Backing Fields

        private string _eventName;
        private string _description;
        private bool _isActive;
        private bool _isRepeatable;
        private DateTime _deadlineDate;
        private DateTime _createDate;
        private DateTime _startNotifyDate;
        private RepeatInterval _repeatInterval;
        private ObservableCollection<FileObservable> _files;

        #endregion

        public EventObservable()
        {
            
        }
        public EventObservable(IEnumerable<FileObservable> files)
        {
            _files = new ObservableCollection<FileObservable>(files);
        }

        public EventObservable(string eventName, string description, bool isActive, bool isRepeatable, DateTime deadlineDate, DateTime date, DateTime startNotifyDate, RepeatInterval repeatInterval, ObservableCollection<FileObservable> files) : this(files)
        {
            _eventName = eventName;
            _description = description;
            _isActive = isActive;
            _isRepeatable = isRepeatable;
            _deadlineDate = deadlineDate;
            _createDate = date;
            _startNotifyDate = startNotifyDate;
            _repeatInterval = repeatInterval;
            _files = files;
        }

        #region Entity Properties

        public int EventId { get; }

        public string EventName
        {
            get => _eventName;
            set => SetProperty(ref _eventName, value);

        }
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);

        }
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);

        }
        public bool IsRepeatable
        {
            get => _isRepeatable;
            set => SetProperty(ref _isRepeatable, value);

        }
        public DateTime DeadlineDate
        {
            get => _deadlineDate;
            set => SetProperty(ref _deadlineDate, value);

        }
        public DateTime CreateDate
        {
            get => _createDate;
            set => SetProperty(ref _createDate, value);

        }
        public DateTime StartNotifyDate
        {
            get => _startNotifyDate;
            set => SetProperty(ref _startNotifyDate, value);

        }
        public RepeatInterval RepeatInterval
        {
            get => _repeatInterval;
            set => SetProperty(ref _repeatInterval, value);

        }
        public ObservableCollection<FileObservable> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }
        #endregion
    }
}
