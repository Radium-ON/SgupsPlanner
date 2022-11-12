using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using Prism.Mvvm;

namespace SgupsPlanner.Core.ViewModels
{
    public class AgreementViewModel : ValidatedDataEntityViewModel<LoanAgreement>
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntitiesContext;

        #endregion

        public AgreementViewModel(LoanAgreement agreement, IBankEntitiesContext bankEntitiesContext) : base(agreement)
        {
            _bankEntitiesContext = bankEntitiesContext;
            ValidatedProperties = new[]
            {
                "AgreementNumber",
                "ContractDate",
                "Bank"
            };
        }

        protected override bool CheckIsEntityContainsInContext(LoanAgreement agreement)
        {
            return _bankEntitiesContext.LoanAgreements.Any(l => l.AgreementNumber == agreement.AgreementNumber);
        }

        #region Entity Properties

        public int AgreementId => entity.PK_AgreementId;

        public bool? IsRepaid => entity.IsRepaid;


        public Client Client => entity.Client;

        public string AgreementNumber
        {
            get => entity.AgreementNumber;
            set
            {
                if (value == entity.AgreementNumber)
                {
                    return;
                }

                entity.AgreementNumber = value;

                RaisePropertyChanged(nameof(AgreementNumber));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public DateTime ContractDate
        {
            get => entity.ContractDate;
            set
            {
                if (value == entity.ContractDate)
                {
                    return;
                }

                entity.ContractDate = value;

                RaisePropertyChanged(nameof(ContractDate));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal? Payment
        {
            get => entity.Payment;
            set
            {
                if (value == entity.Payment)
                {
                    return;
                }

                entity.Payment = value;

                RaisePropertyChanged(nameof(Payment));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal LoanAmount
        {
            get => entity.LoanAmount;
            set
            {
                if (value == entity.LoanAmount)
                {
                    return;
                }

                entity.LoanAmount = value;

                RaisePropertyChanged(nameof(LoanAmount));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int Months
        {
            get => entity.Months;
            set
            {
                if (value == entity.Months)
                {
                    return;
                }

                entity.Months = value;

                RaisePropertyChanged(nameof(Months));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public float Interest
        {
            get => entity.Interest;
            set
            {
                if (value == entity.Interest)
                {
                    return;
                }

                entity.Interest = value;

                RaisePropertyChanged(nameof(Interest));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public Bank Bank
        {
            get => entity.Bank;
            set
            {
                if (value == entity.Bank)
                {
                    return;
                }

                entity.Bank = value;

                RaisePropertyChanged(nameof(Bank));
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
                "AgreementNumber" => ValidateAgreementNumber(),
                "ContractDate" => ValidateContractDate(),
                "Bank" => ValidateBank(),
                _ => null
            };

            return error;
        }

        private string ValidateBank()
        {
            return Bank == null ? Application.Current.FindResource("agreement_error_bank_is_null") as string : null;
        }

        private string ValidateAgreementNumber()
        {
            if (IsStringMissing(AgreementNumber) || !IsStringAllDigits(AgreementNumber))
            {
                return Application.Current.FindResource("agreement_error_agreement_number") as string;
            }
            return null;
        }

        private string ValidateContractDate()
        {
            if (ContractDate.Date == DateTime.MinValue)
            {
                ContractDate = DateTime.Now;
            }
            if (ContractDate.Date > DateTime.Today)
            {
                return Application.Current.FindResource("agreement_error_contract_date") as string;
            }
            return null;
        }

        #endregion
    }
}
