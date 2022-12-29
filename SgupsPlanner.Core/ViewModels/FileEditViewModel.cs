using System;
using System.Windows;
using System.Collections.ObjectModel;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Core.ViewModels
{
    public class FileEditViewModel : ValidatedViewModel<FileObservable>
    {
        #region Backing Fields

        //private readonly IBankEntitiesContext _bankEntitiesContext;

        #endregion

        public FileEditViewModel(FileObservable observable) : base(observable)
        {
            ValidatedProperties = new[]
            {
                "FileName"
            };
        }

        #region Entity Properties

        public int FileId => entity.FileId;

        public string FileName
        {
            get => entity.FileName;
            set
            {
                if (value == entity.FileName)
                {
                    return;
                }

                entity.FileName = value;

                RaisePropertyChanged(nameof(FileName));
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

        public int EventId
        {
            get => entity.EventId;
            set
            {
                if (value == entity.EventId)
                {
                    return;
                }

                entity.EventId = value;

                RaisePropertyChanged(nameof(EventId));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        #endregion

        #region Validation

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "BankName" => ValidateFileName(),
                _ => null
            };

            return error;
        }
        private string ValidateFileName()
        {

            if (IsStringMissing(FileName))
                return Application.Current.FindResource("bank_error_missing_bank_name") as string;

            return null;
        }

        #endregion
    }
}
