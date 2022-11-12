using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BankLoansDataModel;
using BankLoansDataModel.Extensions;
using BankLoansDataModel.Services;
using BanksTable.Views;
using FirstFloor.ModernUI.Windows.Navigation;
using SgupsPlanner.Core.Extensions;
using SgupsPlanner.Core.ViewModels;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace BanksTable.ViewModels
{
    public class BanksTableViewModel : ModernViewModelBase
    {
        #region Backing Fields

        private AsyncObservableCollection<BankViewModel> _bankViewModels;

        private IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;
        private BankViewModel _selectedBankViewModel;

        #endregion

        public BanksTableViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            DeleteBankCommand = new DelegateCommand<BankViewModel>(async vm => await DeleteSelectedBankAsync(vm));
            AddBankCommand = new DelegateCommand(ShowAddBankDialog);
            DeleteOfferFromBankCommand = new DelegateCommand<Offer>(async offer => await DeleteSelectedOfferAsync(offer));
            AddOfferToBankCommand = new DelegateCommand(ShowAddOfferToBankDialog, CanShowAddOfferToBankDialog).ObservesProperty(() => SelectedBankViewModel);

            BankViewModels = new AsyncObservableCollection<BankViewModel>();

            #region Navigation Commands

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadDataAsync());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);

            #endregion
        }

        private bool CanShowAddOfferToBankDialog()
        {
            return SelectedBankViewModel != null;
        }

        private void ShowAddOfferToBankDialog()
        {
            _dialogService.ShowDialog(nameof(OfferToBankAddingDialog),
                new DialogParameters
                {
                    { "BankViewModel", SelectedBankViewModel },
                    {"DbContext", _bankEntities}
                },
                r => { });
        }

        private async Task DeleteSelectedOfferAsync(Offer offer)
        {
            if (SelectedBankViewModel != null)
            {
                SelectedBankViewModel.Offers.Remove(offer);
                await _bankEntities.SaveChangesAsync(CancellationToken.None);
            }
        }

        private void ShowAddBankDialog()
        {
            _dialogService.ShowDialog(nameof(BankAddingDialog), new DialogParameters { { "BankViewModel", new BankViewModel(new Bank(), _bankEntities) } },
                async r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        var addedBankVm = r.Parameters.GetValue<BankViewModel>("AddedBankViewModel");

                        _bankEntities.Banks.Add(addedBankVm.Entity);
                        await _bankEntities.SaveChangesAsync(CancellationToken.None);

                        BankViewModels.Add(addedBankVm);
                    }
                });
        }



        public AsyncObservableCollection<BankViewModel> BankViewModels
        {
            get => _bankViewModels;
            set => SetProperty(ref _bankViewModels, value);
        }

        public BankViewModel SelectedBankViewModel
        {
            get => _selectedBankViewModel;
            set => SetProperty<BankViewModel>(ref _selectedBankViewModel, value);
        }

        public ObjectContext CurrentObjectContext => ((IObjectContextAdapter)_bankEntities).ObjectContext;

        #region Commands

        public ICommand DeleteBankCommand { get; private set; }
        public ICommand AddBankCommand { get; private set; }
        public ICommand AddOfferToBankCommand { get; private set; }
        public ICommand DeleteOfferFromBankCommand { get; private set; }

        #endregion

        #region NavigationEvents Methods

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("BanksTableViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadDataAsync()
        {
            _bankEntities = new BankEntitiesContext();
            BankViewModels.Clear();
            BankViewModels.AddRange(await GetBankInfoViewModelsAsync(_bankEntities.Banks));
            Debug.WriteLine("BanksTableViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("BanksTableViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("BanksTableViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("BanksTableViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        private void NavigatingFrom(NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            var dbcontext = _bankEntities as DbContext;
            var hasChanges = dbcontext?.ChangeTracker.HasChanges();

            if (hasChanges == true)
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("some_data_changed_dialog_title") as string,
                    Application.Current.FindResource("some_data_changed_dialog_message") as string,
                    async r => { await NavigatingWithModifiedBanksCallBackAsync(r, navigatingCancelEventArgs, dbcontext); });
            }
            Debug.WriteLine("BanksTableViewModel - NavigatingFrom");
        }

        #endregion

        private async Task DeleteSelectedBankAsync(BankViewModel bankVm)
        {
            if (bankVm == null) return;

            if (bankVm.Entity.LoanAgreements.Count == 0 && bankVm.Entity.Offers.Count == 0)
            {
                await RemoveBankAsync(bankVm);
            }
            else
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("bank_deleted_dialog_title") as string,
                    Application.Current.FindResource("bank_deleted_dialog_message") as string,
                    async r =>
                    {
                        if (r.Result == ButtonResult.OK)
                        {
                            await RemoveBankAsync(bankVm);
                        }
                    });
            }
        }

        private async Task RemoveBankAsync(BankViewModel bankVm)
        {
            _bankEntities.Banks.Remove(bankVm.Entity);
            BankViewModels.Remove(bankVm);
            await _bankEntities.SaveChangesAsync(CancellationToken.None);
        }

        private async Task<IEnumerable<BankViewModel>> GetBankInfoViewModelsAsync(IDbSet<Bank> banks)
        {
            await banks.LoadAsync();
            return banks.Local.Select(bank => new BankViewModel(bank, _bankEntities));
        }

        private async Task NavigatingWithModifiedBanksCallBackAsync(IDialogResult r, NavigatingCancelEventArgs e, DbContext dbcontext)
        {
            if (r.Result == ButtonResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (r.Result == ButtonResult.OK)
            {
                var updatedBanks = CurrentObjectContext.GetEntriesByEntityState<Bank>(EntityState.Modified);
                var badBanks = await GetNotValidBankViewModelsAsync(BankViewModels);
                if (badBanks.Count == 0)
                {
                    var status = await _bankEntities.SaveChangesWithValidationAsync(CancellationToken.None);
                    if (!status.IsValid)
                    {
                        e.Cancel = true;

                        _dialogService.ShowOkDialog("Ошибка обновления", string.Join("\n", status.EfErrors), m => { });
                        await dbcontext.ReloadAllEntitiesAsync(updatedBanks);
                    }
                }
                else
                {
                    e.Cancel = true;
                    await dbcontext.ReloadAllEntitiesAsync(updatedBanks);
                }
            }
        }

        /// <summary>
        /// Возвращает список оболочек неисправных клиентов <see cref="BankViewModel"/>; асинхронный.
        /// </summary>
        /// <param name="bankInfoViewModels"></param>
        /// <returns>Список неуникальных клиентов.</returns>
        private async Task<List<BankViewModel>> GetNotValidBankViewModelsAsync(IEnumerable<BankViewModel> bankInfoViewModels)
        {
            return await bankInfoViewModels.AsAsyncQueryable().Where(vm => vm.IsValid == false).ToListAsync();
        }
    }
}
