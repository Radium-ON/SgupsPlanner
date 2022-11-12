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
    public class OfferViewModel : ValidatedDataEntityViewModel<Offer>
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntitiesContext;
        private bool _isSelected;

        #endregion

        public OfferViewModel(Offer offer, IBankEntitiesContext bankEntitiesContext) : base(offer)
        {
            _bankEntitiesContext = bankEntitiesContext;
            ValidatedProperties = new[]
            {
                "Interest",
                "MinLoanAmount",
                "MaxLoanAmount",
                "MinSeniority",
                "MinAge",
                "ActiveLoansNumber"
            };
        }

        protected override bool CheckIsEntityContainsInContext(Offer offer)
        {
            var findedOffer = _bankEntitiesContext.Offers.FirstOrDefault(existedOffer =>
                existedOffer.Interest == offer.Interest &&
                existedOffer.MinLoanAmount == offer.MinLoanAmount &&
                existedOffer.MaxLoanAmount == offer.MaxLoanAmount &&
                existedOffer.MaxOfMonths == offer.MaxOfMonths &&
                existedOffer.ActiveLoansNumber == offer.ActiveLoansNumber &&
                existedOffer.MinSeniority == offer.MinSeniority &&
                existedOffer.MinAge == offer.MinAge);

            return findedOffer != null;
        }

        #region Entity Properties

        public int OfferId => entity.PK_OfferId;

        public float Interest
        {
            get => entity.Interest;
            set
            {
                if (value == entity.Interest)
                {
                    return;
                }

                entity.Interest = (float)(value * 0.01);

                RaisePropertyChanged(nameof(Interest));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal MinLoanAmount
        {
            get => entity.MinLoanAmount;
            set
            {
                if (value == entity.MinLoanAmount)
                {
                    return;
                }

                entity.MinLoanAmount = value;

                RaisePropertyChanged(nameof(MinLoanAmount));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public decimal MaxLoanAmount
        {
            get => entity.MaxLoanAmount;
            set
            {
                if (value == entity.MaxLoanAmount)
                {
                    return;
                }

                entity.MaxLoanAmount = value;

                RaisePropertyChanged(nameof(MaxLoanAmount));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int MaxOfMonths
        {
            get => entity.MaxOfMonths;
            set
            {
                if (value == entity.MaxOfMonths)
                {
                    return;
                }

                entity.MaxOfMonths = value;

                RaisePropertyChanged(nameof(MaxOfMonths));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int? ActiveLoansNumber
        {
            get => entity.ActiveLoansNumber;
            set
            {
                if (value == entity.ActiveLoansNumber)
                {
                    return;
                }

                entity.ActiveLoansNumber = value;

                RaisePropertyChanged(nameof(ActiveLoansNumber));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int? MinSeniority
        {
            get => entity.MinSeniority;
            set
            {
                if (value == entity.MinSeniority)
                {
                    return;
                }

                entity.MinSeniority = value;

                RaisePropertyChanged(nameof(MinSeniority));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public int? MinAge
        {
            get => entity.MinAge;
            set
            {
                if (value == entity.MinAge)
                {
                    return;
                }

                entity.MinAge = value;

                RaisePropertyChanged(nameof(MinAge));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public ObservableCollection<Bank> Banks => entity.Banks;

        #endregion

        #region UI Properties

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        #endregion

        #region Validation

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "Interest" => ValidateInterest(),
                "MinLoanAmount" => ValidateMinLoanAmount(),
                "MaxLoanAmount" => ValidateMaxLoanAmount(),
                "MinSeniority" => ValidateMinSeniority(),
                "MinAge" => ValidateMinAge(),
                "ActiveLoansNumber" => ValidateActiveLoansNumber(),
                _ => null
            };
            return error;
        }

        private string ValidateActiveLoansNumber()
        {
            if (ActiveLoansNumber < 0)
            {
                return Application.Current.FindResource("offer_error_active_loans_number_out_of_range") as string;
            }
            return null;
        }


        private string ValidateMinSeniority()
        {
            if (MinSeniority < 0 || MinSeniority > 100 || MinSeniority >= MinAge)
            {
                return Application.Current.FindResource("offer_error_seniority_out_of_range") as string;
            }
            return null;
        }

        private string ValidateMinAge()
        {
            if (MinAge <= 0 || MinAge <= MinSeniority)
            {
                return Application.Current.FindResource("offer_error_age_out_of_range") as string;
            }
            return null;
        }

        private string ValidateMaxLoanAmount()
        {
            if (MaxLoanAmount <= MinLoanAmount || MaxLoanAmount <= 0)
            {
                return Application.Current.FindResource("offer_error_max_loan_amount_out_of_range") as string;
            }
            return null;
        }

        private string ValidateInterest()
        {
            if (Interest <= 0f)
            {
                return Application.Current.FindResource("offer_error_interest_negate") as string;
            }
            return null;
        }

        private string ValidateMinLoanAmount()
        {

            if (MinLoanAmount >= MaxLoanAmount || MinLoanAmount <= 0)
            {
                return Application.Current.FindResource("offer_error_min_loan_amount_out_of_range") as string;
            }
            return null;
        }

        #endregion
    }
}
