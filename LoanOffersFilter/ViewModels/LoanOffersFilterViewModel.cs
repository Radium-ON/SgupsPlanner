using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Navigation;
using SgupsPlanner.Core.Extensions;
using SgupsPlanner.Core.ViewModels;
using LoanOffersFilter.Views;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace LoanOffersFilter.ViewModels
{
    [SuppressMessage("ReSharper", "AsyncConverter.ConfigureAwaitHighlighting")]
    public class LoanOffersFilterViewModel : ModernViewModelBase
    {
        #region Backing Fields

        private IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;

        private ICollectionView _clientsCollectionView;
        private CollectionViewSource _offersViewSource;
        private ObservableCollection<Offer> _offers;
        private Client _selectedClient;

        private int? _monthsInput;
        private double? _loanAmountInput;
        private float? _interestInput;
        private bool _canRemoveMonthsFilter;
        private bool _canRemoveLoanAmountFilter;
        private bool _canRemoveInterestFilter;
        private bool _canRemoveClientFilter;
        private Offer _selectedOffer;

        #endregion

        public LoanOffersFilterViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            OffersViewSource = new CollectionViewSource
            {
                IsLiveFilteringRequested = true,
                IsLiveGroupingRequested = true,
            };

            CreateLoanAgreementCommand = new DelegateCommand(CreateLoanAgreement, CanCreateLoanAgreement).ObservesProperty(() => AvailableFunds);

            #region Filter Commands

            ResetFiltersCommand = new RelayCommand(ResetFilters);
            RemoveClientFilterCommand = new RelayCommand(RemoveClientFilter, o => CanRemoveClientFilter);
            RemoveMonthsFilterCommand = new RelayCommand(RemoveMonthsFilter, o => CanRemoveMonthsFilter);
            RemoveLoanAmountFilterCommand = new RelayCommand(RemoveLoanAmountFilter, o => CanRemoveLoanAmountFilter);
            RemoveInterestFilterCommand = new RelayCommand(RemoveInterestFilter, o => CanRemoveInterestFilter);

            #endregion

            #region Navigation Commands

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadDataAsync());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);

            #endregion
        }



        private bool CanCreateLoanAgreement()
        {
            return AvailableFunds >= 0 &&
                   SelectedOffer != null &&
                   OffersViewSource.View.CurrentItem != null &&
                   LoanAmountInput.HasValue &&
                   MonthsInput.HasValue;
        }

        private void CreateLoanAgreement()
        {
            var agreement = new LoanAgreement
            {
                Client = SelectedClient,
                Interest = SelectedOffer.Interest,
                Months = (int)MonthsInput,
                LoanAmount = (decimal)LoanAmountInput,
                Payment = (decimal?)Payment,
            };

            ShowAddAgreementDialog(agreement);
            ResetFilters(null);
        }

        private void ShowAddAgreementDialog(LoanAgreement agreement)
        {
            _dialogService.ShowDialog(nameof(AgreementAddingDialog),
                new DialogParameters
                {
                    { "AgreementViewModel", new AgreementViewModel(agreement, _bankEntities) },
                    {"CurrentOffer",SelectedOffer}
                },
                async r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        var addedAgreementVm = r.Parameters.GetValue<AgreementViewModel>("AddedAgreementViewModel");

                        SelectedClient.LoanAgreements.Add(addedAgreementVm.Entity);
                        await _bankEntities.SaveChangesAsync(CancellationToken.None);
                    }
                });
        }

        private void OnCurrentClientChanged(object sender, EventArgs e)
        {
            SelectedClient = (Client)ClientsCollectionView.CurrentItem;
            RaisePropertyChanged(nameof(Payment));
            RaisePropertyChanged(nameof(AvailableFunds));
        }

        private void OnCurrentOfferChanged(object sender, EventArgs e)
        {
            if (SelectedOffer != null)
            {
                InterestInput = SelectedOffer.Interest;
                RemoveInterestFilterCommand.Execute(null);
                RaisePropertyChanged(nameof(Payment));
                RaisePropertyChanged(nameof(AvailableFunds));
            }
        }

        private void InitCollectionViews()
        {
            ClientsCollectionView = CollectionViewSource.GetDefaultView(_bankEntities.Clients.Local);
            ClientsCollectionView.CurrentChanged += OnCurrentClientChanged;

            OffersViewSource.View.CurrentChanged += OnCurrentOfferChanged;
            using (OffersViewSource.DeferRefresh())
            {
                if (OffersViewSource.GroupDescriptions.Count == 0)
                {
                    OffersViewSource.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Offer.Banks)));
                }
            }
        }



        public ObjectContext CurrentObjectContext => ((IObjectContextAdapter)_bankEntities).ObjectContext;

        public ICollectionView ClientsCollectionView
        {
            get => _clientsCollectionView;
            set => SetProperty(ref _clientsCollectionView, value);
        }

        public CollectionViewSource OffersViewSource
        {
            get => _offersViewSource;
            set => SetProperty(ref _offersViewSource, value);
        }

        public Client SelectedClient
        {
            get => _selectedClient;
            set => SetProperty(ref _selectedClient, value,
                () => ApplyFilter(_selectedClient != null ? FilterField.Client : FilterField.None));
        }

        public Offer SelectedOffer
        {
            get => _selectedOffer;
            set => SetProperty(ref _selectedOffer, value);
        }

        public ObservableCollection<Offer> Offers
        {
            get => _offers;
            set => SetProperty(ref _offers, value);
        }

        public double Payment
        {
            get
            {
                if (MonthsInput.HasValue && LoanAmountInput.HasValue && SelectedOffer != null)
                {
                    return CalculatePayment();
                }

                return 0;
            }
        }

        public double AvailableFunds
        {
            get
            {
                if (SelectedClient == null)
                {
                    return 0;
                }
                return (double)SelectedClient.Salary - (double)SelectedClient.LoanAgreements
                    .Where(l => l.IsRepaid == false).Sum(l => l.Payment) - Payment;
            }
        }

        private double CalculatePayment()
        {
            double? result = 0;
            double? interestPerMonth = SelectedOffer.Interest / 12.0;
            if (MonthsInput.HasValue)
            {
                var power = Math.Pow((double)(1 + interestPerMonth), (double)MonthsInput);
                var numerator = interestPerMonth * power;
                var denominator = power - 1;
                var annuityRate = numerator / denominator;

                result = (LoanAmountInput * annuityRate);
            }
            return (double)result;
        }

        private bool CheckConnection(DbContext context)
        {
            try
            {
                context.Database.Connection.Open();
                context.Database.Connection.Close();
            }
            catch (SqlException)
            {
                return false;
            }
            return true;
        }

        #region Filter Sliders Properties

        public int? MonthsInput
        {
            get => _monthsInput;
            set => SetProperty(ref _monthsInput, value,
                () =>
                {
                    ApplyFilter(_monthsInput.HasValue ? FilterField.Months : FilterField.None);
                    RaisePropertyChanged(nameof(Payment));
                    RaisePropertyChanged(nameof(AvailableFunds));
                });
        }

        public double? LoanAmountInput
        {
            get => _loanAmountInput;
            set => SetProperty(ref _loanAmountInput, value, () =>
            {
                ApplyFilter(_loanAmountInput.HasValue ? FilterField.LoanAmount : FilterField.None);
                RaisePropertyChanged(nameof(Payment));
                RaisePropertyChanged(nameof(AvailableFunds));
            });
        }

        public float? InterestInput
        {
            get => _interestInput;
            set => SetProperty(ref _interestInput, value, () => ApplyFilter(_interestInput.HasValue ? FilterField.Interest : FilterField.None));
        }

        /// <summary>
        /// Gets or sets a flag indicating if the Months filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveMonthsFilter
        {
            get => _canRemoveMonthsFilter;
            set => SetProperty(ref _canRemoveMonthsFilter, value);
        }

        /// <summary>
        /// Gets or sets a flag indicating if the LoanAmount filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveLoanAmountFilter
        {
            get => _canRemoveLoanAmountFilter;
            set => SetProperty(ref _canRemoveLoanAmountFilter, value);
        }

        /// <summary>
        /// Gets or sets a flag indicating if the Interest filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveInterestFilter
        {
            get => _canRemoveInterestFilter;
            set => SetProperty(ref _canRemoveInterestFilter, value);
        }

        /// <summary>
        /// Gets or sets a flag indicating if the Client filter, if applied, can be removed.
        /// </summary>
        public bool CanRemoveClientFilter
        {
            get => _canRemoveClientFilter;
            set => SetProperty(ref _canRemoveClientFilter, value);
        }

        #endregion

        #region Commands Properties

        public ICommand RemoveClientFilterCommand
        {
            get; private set;
        }
        public ICommand ResetFiltersCommand
        {
            get;
            private set;
        }
        public ICommand RemoveMonthsFilterCommand
        {
            get;
            private set;
        }
        public ICommand RemoveLoanAmountFilterCommand
        {
            get;
            private set;
        }
        public ICommand RemoveInterestFilterCommand
        {
            get;
            private set;
        }

        public ICommand CreateLoanAgreementCommand
        {
            get; private set;
        }
        public ICommand TakeSelectedItemGroupNameCommand
        {
            get; private set;
        }

        #endregion

        #region Filtering Helpers

        /// <summary>
        /// Очищает фильтры отпиской от событий и обнулением свойств выбора
        /// </summary>
        /// <param name="o"></param>
        public void ResetFilters(object o)
        {
            RemoveClientFilter(o);
            RemoveInterestFilter(o);
            RemoveLoanAmountFilter(o);
            RemoveMonthsFilter(o);
        }

        private void RemoveClientFilter(object o)
        {
            OffersViewSource.Filter -= FilterByClient;
            SelectedClient = null;
            CanRemoveClientFilter = false;
        }

        public void RemoveMonthsFilter(object o)
        {
            OffersViewSource.Filter -= FilterByMonths;
            LoanAmountInput = null;
            CanRemoveMonthsFilter = false;
        }

        public void RemoveLoanAmountFilter(object o)
        {
            OffersViewSource.Filter -= FilterByLoanAmount;
            MonthsInput = null;
            CanRemoveLoanAmountFilter = false;
        }

        public void RemoveInterestFilter(object o)
        {
            OffersViewSource.Filter -= FilterByInterest;
            InterestInput = null;
            CanRemoveInterestFilter = false;
        }

        /* Notes on Adding Filters:
         *   Each filter is added by subscribing a filter method to the Filter event
         *   of the OffersViewSource.  Filters are applied in the order in which they were added. 
         *   To prevent adding filters mulitple times ( because we are using drop down lists
         *   in the view), the CanRemove***Filter flags are used to ensure each filter
         *   is added only once.  If a filter has been added, its corresponding CanRemove***Filter
         *   is set to true.       
         *   
         *   If a filter has been applied already (for example someone selects "Canada" to filter by country
         *   and then they change their selection to another value (say "Mexico") we need to undo the previous
         *   country filter then apply the new one.  This does not completey Reset the filter, just
         *   allows it to be changed to another filter value. This applies to the other filters as well
         */

        public void AddClientFilter()
        {
            if (CanRemoveClientFilter)
            {
                OffersViewSource.Filter -= FilterByClient;
                OffersViewSource.Filter += FilterByClient;
            }
            else
            {
                OffersViewSource.Filter += FilterByClient;
                CanRemoveClientFilter = true;
            }
        }

        public void AddMonthsFilter()
        {
            if (CanRemoveMonthsFilter)
            {
                OffersViewSource.Filter -= FilterByMonths;
                OffersViewSource.Filter += FilterByMonths;
            }
            else
            {
                OffersViewSource.Filter += FilterByMonths;
                CanRemoveMonthsFilter = true;
            }
        }

        public void AddLoanAmountFilter()
        {
            if (CanRemoveLoanAmountFilter)
            {
                OffersViewSource.Filter -= FilterByLoanAmount;
                OffersViewSource.Filter += FilterByLoanAmount;
            }
            else
            {
                OffersViewSource.Filter += FilterByLoanAmount;
                CanRemoveLoanAmountFilter = true;
            }
        }

        public void AddInterestFilter()
        {
            if (CanRemoveInterestFilter)
            {
                OffersViewSource.Filter -= FilterByInterest;
                OffersViewSource.Filter += FilterByInterest;
            }
            else
            {
                OffersViewSource.Filter += FilterByInterest;
                CanRemoveInterestFilter = true;
            }
        }

        #endregion

        #region Filter Handlers

        /* Notes on Filter Methods:
         * When using multiple filters, do not explicitly set anything to true.  Rather,
         * only hide things which do not match the filter criteria
         * by setting e.Accepted = false.  If you set e.Accept = true, if effectively
         * clears out any previous filters applied to it.  
         */

        private void FilterByClient(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Offer src))
                e.Accepted = false;
            else if (SelectedClient.Age < src.MinAge ||
                     SelectedClient.Seniority < src.MinSeniority ||
                     SelectedClient.LoanAgreements.Count(l => l.IsRepaid == false) > src.ActiveLoansNumber)
                e.Accepted = false;
        }

        private void FilterByLoanAmount(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Offer src))
                e.Accepted = false;
            else if (LoanAmountInput > (double?)src.MaxLoanAmount || LoanAmountInput < (double?)src.MinLoanAmount)
                e.Accepted = false;
        }

        private void FilterByInterest(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Offer src))
                e.Accepted = false;
            else if (InterestInput < src.Interest)
                e.Accepted = false;
        }

        private void FilterByMonths(object sender, FilterEventArgs e)
        {
            if (!(e.Item is Offer src))
                e.Accepted = false;
            else if (MonthsInput > src.MaxOfMonths)
                e.Accepted = false;
        }

        private enum FilterField
        {
            Client,
            LoanAmount,
            Months,
            Interest,
            None
        }

        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.Client:
                    AddClientFilter();
                    break;
                case FilterField.LoanAmount:
                    AddLoanAmountFilter();
                    break;
                case FilterField.Months:
                    AddMonthsFilter();
                    break;
                case FilterField.Interest:
                    AddInterestFilter();
                    break;
            }
        }

        #endregion

        #region NavigationEvents Methods

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadDataAsync()
        {
            _bankEntities = new BankEntitiesContext();
            if (CheckConnection(_bankEntities as DbContext))
            {
                await _bankEntities.Clients.LoadAsync();
                await _bankEntities.Offers.LoadAsync();

                Offers = _bankEntities.Offers.Local;
                OffersViewSource.Source = Offers;

                if (ClientsCollectionView == null)
                {
                    InitCollectionViews();
                }
            }
            else
            {
                _dialogService.ShowOkDialog(
                    Application.Current.FindResource("connection_error_dialog_title") as string,
                    Application.Current.FindResource("connection_error_dialog_message") as string,
                    r => { Application.Current.MainWindow?.Close();});
            }
            Debug.WriteLine("LoanOffersFilterViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("LoanOffersFilterViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        /// <param name="e">Параметры отмены навигации</param>
        private void NavigatingFrom(NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("LoanOffersFilterViewModel - NavigatingFrom");
        }

        #endregion
    }
}
