using System;
using System.Windows;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.ViewModels
{
    public class FileObservable : BindableBase
    {
        #region Backing Fields

        private string _fileName;
        private DateTime _createDate;
        private int _eventId;

        #endregion

        public FileObservable()
        {
            
        }

        public FileObservable(string fileName, DateTime date, int eventId)
        {
            _fileName = fileName;
            _createDate = date;
            _eventId = eventId;
        }

        public FileObservable(string fileName, DateTime date, int eventId, int fileId) : this(fileName, date, eventId)
        {
            _fileName = fileName;
            _createDate = date;
            _eventId = eventId;
            FileId = fileId;
        }

        #region Entity Properties

        public int FileId { get; }

        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);

        }

        public DateTime CreateDate
        {
            get => _createDate;
            set => SetProperty(ref _createDate, value);

        }

        public int EventId
        {
            get => _eventId;
            set => SetProperty(ref _eventId, value);
        }

        #endregion

    }
}
