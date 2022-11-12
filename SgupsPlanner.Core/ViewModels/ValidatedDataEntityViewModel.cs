using System;
using System.ComponentModel;
using System.Linq;
using Prism.Mvvm;

namespace SgupsPlanner.Core.ViewModels
{
    public abstract class ValidatedDataEntityViewModel<TEntity> : BindableBase, IDataErrorInfo
    {
        #region Backing Fields

        private protected TEntity entity;

        #endregion

        protected ValidatedDataEntityViewModel(TEntity entity)
        {
            this.entity = entity;
        }

        public TEntity Entity => entity;

        public bool IsUnique => !CheckIsEntityContainsInContext(entity);

        protected abstract bool CheckIsEntityContainsInContext(TEntity entity);



        #region Implementation of IDataErrorInfo

        public string this[string columnName] => GetValidationError(columnName);

        public string Error => null;

        #endregion

        #region Validation

        public bool IsValid
        {
            get
            {
                return ValidatedProperties != null && ValidatedProperties.All(property => GetValidationError(property) == null);
            }
        }

        public string[] ValidatedProperties { get; protected set; }



        protected abstract string GetValidationError(string propertyName);


        protected static bool IsStringMissing(string value)
        {
            return
                string.IsNullOrEmpty(value) ||
                value.Trim() == string.Empty;
        }

        protected static bool IsStringAllDigits(string value)
        {
            return value.All(char.IsDigit);
        }

        protected static bool IsStringAllLetters(string value)
        {
            return value.All(char.IsLetter);
        }
        #endregion
    }
}
