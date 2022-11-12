using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using Prism.Mvvm;

namespace SgupsPlanner.Core.ViewModels
{
    public class BankViewModel : ValidatedDataEntityViewModel<Bank>
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntitiesContext;

        #endregion

        public BankViewModel(Bank bank, IBankEntitiesContext dbContext) : base(bank)
        {
            _bankEntitiesContext = dbContext;
            ValidatedProperties = new[]
            {
                "BankName",
                "Ogrn"
            };
        }

        protected override bool CheckIsEntityContainsInContext(Bank bank)
        {
            return _bankEntitiesContext.Banks.Any(c => c.OGRN == bank.OGRN);
        }

        #region Entity Properties

        public int BankId => entity.PK_RegNumber;

        public string BankName
        {
            get => entity.Name;
            set
            {
                if (value == entity.Name)
                {
                    return;
                }

                entity.Name = value;

                RaisePropertyChanged(nameof(BankName));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public string Ogrn
        {
            get => entity.OGRN;
            set
            {
                if (value == entity.OGRN)
                {
                    return;
                }

                entity.OGRN = value;

                RaisePropertyChanged(nameof(Ogrn));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public string BankLicense
        {
            get => entity.License;
            set
            {
                if (value == entity.License)
                {
                    return;
                }

                entity.License = value;

                RaisePropertyChanged(nameof(BankLicense));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public ObservableCollection<LoanAgreement> LoanAgreements => entity.LoanAgreements;

        public ObservableCollection<Offer> Offers => entity.Offers;
        #endregion

        #region Validation

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "BankName" => ValidateBankName(),
                "Ogrn" => ValidateOgrn(),
                _ => null
            };

            return error;
        }

        private string ValidateOgrn()
        {
            if (IsStringMissing(Ogrn) || !IsStringAllDigits(Ogrn) || Ogrn.Length != 13 || Ogrn.StartsWith("0"))
            {
                return Application.Current.FindResource("bank_error_missing_ogrn") as string;
            }
            return null;
        }

        private string ValidateLicense()
        {
            if (IsStringMissing(BankLicense))
            {
                return Application.Current.FindResource("bank_error_missing_license") as string;
            }
            return null;
        }

        private string ValidateBankName()
        {

            if (IsStringMissing(BankName))
                return Application.Current.FindResource("bank_error_missing_bank_name") as string;

            return null;
        }

        #endregion
    }
}
