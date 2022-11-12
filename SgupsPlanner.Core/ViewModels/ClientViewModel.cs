using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI;
using Prism.Mvvm;

namespace SgupsPlanner.Core.ViewModels
{
    public class ClientViewModel : ValidatedDataEntityViewModel<Client>
    {
        #region Backing Fields

        private readonly IBankEntitiesContext _bankEntities;

        #endregion

        public ClientViewModel(Client client, IBankEntitiesContext bankEntities) : base(client)
        {
            _bankEntities = bankEntities;
            ValidatedProperties = new[]
            {
                "FirstName",
                "LastName",
                "Passport",
                "Tin",
                "Age",
                "Seniority",
                "Salary"
            };
        }

        protected override bool CheckIsEntityContainsInContext(Client client)
        {
            return _bankEntities.Clients.Any(c => c.Passport == client.Passport || c.TIN == client.TIN);
        }

        #region Entity Properties

        public int ClientId => entity.PK_ClientId;

        public string FirstName
        {
            get => entity.FirstName;
            set
            {
                if (value == entity.FirstName)
                {
                    return;
                }

                entity.FirstName = value;

                RaisePropertyChanged(nameof(FirstName));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string LastName
        {
            get => entity.LastName;
            set
            {
                if (value == entity.LastName)
                {
                    return;
                }

                entity.LastName = value;

                RaisePropertyChanged(nameof(LastName));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string Passport
        {
            get => entity.Passport;
            set
            {
                if (value == entity.Passport)
                {
                    return;
                }

                entity.Passport = value;

                RaisePropertyChanged(nameof(Passport));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public string Tin
        {
            get => entity.TIN;
            set
            {
                if (value == entity.TIN)
                {
                    return;
                }

                entity.TIN = value;

                RaisePropertyChanged(nameof(Tin));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public int Age
        {
            get => entity.Age;
            set
            {
                if (value == entity.Age)
                {
                    return;
                }

                entity.Age = value;

                RaisePropertyChanged(nameof(Age));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public int Seniority
        {
            get => entity.Seniority;
            set
            {
                if (value == entity.Seniority)
                {
                    return;
                }

                entity.Seniority = value;

                RaisePropertyChanged(nameof(Seniority));
                RaisePropertyChanged(nameof(IsValid));
            }
        }
        public decimal Salary
        {
            get => entity.Salary;
            set
            {
                if (value == entity.Salary)
                {
                    return;
                }

                entity.Salary = value;

                RaisePropertyChanged(nameof(Salary));
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public ObservableCollection<LoanAgreement> LoanAgreements => entity.LoanAgreements;
        #endregion
        
        #region Validation
        
        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            var error = propertyName switch
            {
                "FirstName" => ValidateFirstName(),
                "LastName" => ValidateLastName(),
                "Passport" => ValidatePassport(),
                "Tin" => ValidateTin(),
                "Age" => ValidateAge(),
                "Seniority" => ValidateSeniority(),
                "Salary" => ValidateSalary(),
                _ => null
            };

            return error;
        }

        private string ValidateSalary()
        {
            if (Salary <= 0)
            {
                return Application.Current.FindResource("client_error_missing_salary") as string;
            }
            return null;
        }

        private string ValidateSeniority()
        {
            if (Seniority < 0 || Seniority > 100 || Seniority >= Age)
            {
                return Application.Current.FindResource("client_error_missing_seniority") as string;
            }
            return null;
        }

        private string ValidateAge()
        {
            if (Age <= 0 || Age <= Seniority)
            {
                return Application.Current.FindResource("client_error_missing_age") as string;
            }
            return null;
        }

        private string ValidateTin()
        {
            if (IsStringMissing(Tin) || !IsStringAllDigits(Tin) || Tin.Length != 12 || Tin.StartsWith("0"))
            {
                return Application.Current.FindResource("client_error_missing_tin") as string;
            }
            return null;
        }

        private string ValidatePassport()
        {
            if (IsStringMissing(Passport) || !IsStringAllDigits(Passport) || Passport.Length != 10 || Passport.StartsWith("0"))
            {
                return Application.Current.FindResource("client_error_missing_passport") as string;
            }
            return null;
        }

        private string ValidateFirstName()
        {
            if (IsStringMissing(FirstName) || !IsStringAllLetters(FirstName))
            {
                return Application.Current.FindResource("client_error_missing_first_name") as string;
            }
            return null;
        }

        private string ValidateLastName()
        {

            if (IsStringMissing(LastName) || !IsStringAllLetters(LastName))
                return Application.Current.FindResource("client_error_missing_lastname") as string;

            return null;
        }

        #endregion
    }
}
