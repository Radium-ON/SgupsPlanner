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
using BankLoansDataModel;
using BankLoansDataModel.Extensions;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Windows.Navigation;
using SgupsPlanner.Core.Extensions;
using SgupsPlanner.Core.ViewModels;
using OffersTable.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OffersTableViewModel : ModernViewModelBase
    {
        #region Backing Fields

        private ObservableCollection<OfferViewModel> _offerViewModels;

        private IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;

        #endregion

        public OffersTableViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            DeleteOfferCommand = new DelegateCommand<OfferViewModel>(async vm => await DeleteSelectedOfferAsync(vm));
            AddOfferCommand = new DelegateCommand(ShowAddOfferDialog);

            OfferViewModels = new ObservableCollection<OfferViewModel>();

            #region Navigation Commands

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadDataAsync());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);

            #endregion
        }

        private void ShowAddOfferDialog()
        {
            _dialogService.ShowDialog(nameof(OfferAddingDialog), new DialogParameters { { "OfferViewModel", new OfferViewModel(new Offer(), _bankEntities) } },
                async r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        var addedOfferVm = r.Parameters.GetValue<OfferViewModel>("AddedOfferViewModel");

                        _bankEntities.Offers.Add(addedOfferVm.Entity);
                        await _bankEntities.SaveChangesAsync(CancellationToken.None);

                        OfferViewModels.Add(addedOfferVm);
                    }
                });
        }



        public ObservableCollection<OfferViewModel> OfferViewModels
        {
            get => _offerViewModels;
            set => SetProperty(ref _offerViewModels, value);
        }

        public ObjectContext CurrentObjectContext => ((IObjectContextAdapter)_bankEntities).ObjectContext;

        #region DelegateCommands

        public DelegateCommand<OfferViewModel> DeleteOfferCommand { get; private set; }
        public DelegateCommand AddOfferCommand { get; private set; }

        #endregion

        #region NavigationEvents Methods

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("OffersTableViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadDataAsync()
        {
            _bankEntities = new BankEntitiesContext();
            OfferViewModels.Clear();
            OfferViewModels.AddRange(await GetOfferInfoViewModelsAsync(_bankEntities.Offers));
            Debug.WriteLine("OffersTableViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("OffersTableViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("OffersTableViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("OffersTableViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        /// <param name="e">Параметры отмены навигации</param>
        private void NavigatingFrom(NavigatingCancelEventArgs e)
        {
            var dbcontext = _bankEntities as DbContext;
            var hasChanges = dbcontext?.ChangeTracker.HasChanges();

            if (hasChanges == true)
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("some_data_changed_dialog_title") as string,
                    Application.Current.FindResource("some_data_changed_dialog_message") as string,
                    async r => { await NavigatingWithModifiedOffersCallBackAsync(r, e, dbcontext); });
            }
            Debug.WriteLine("OffersTableViewModel - NavigatingFrom");
        }

        #endregion

        private async Task DeleteSelectedOfferAsync(OfferViewModel offerVm)
        {
            if (offerVm == null) return;

            if (offerVm.Entity.Banks.Count == 0)
            {
                await RemoveOfferAsync(offerVm);
            }
            else
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("offer_deleted_dialog_title") as string,
                    Application.Current.FindResource("offer_deleted_dialog_message") as string,
                    async r =>
                    {
                        if (r.Result == ButtonResult.OK)
                        {
                            await RemoveOfferAsync(offerVm);
                        }
                    });
            }
        }

        private async Task RemoveOfferAsync(OfferViewModel offerVm)
        {
            _bankEntities.Offers.Remove(offerVm.Entity);
            OfferViewModels.Remove(offerVm);
            await _bankEntities.SaveChangesAsync(CancellationToken.None);
        }

        private async Task<IEnumerable<OfferViewModel>> GetOfferInfoViewModelsAsync(IDbSet<Offer> offers)
        {
            await offers.LoadAsync();
            return offers.Local.Select(offer => new OfferViewModel(offer, _bankEntities));
        }

        private async Task NavigatingWithModifiedOffersCallBackAsync(IDialogResult r, NavigatingCancelEventArgs e, DbContext dbcontext)
        {
            if (r.Result == ButtonResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (r.Result == ButtonResult.OK)
            {
                var updatedOffers = CurrentObjectContext.GetEntriesByEntityState<Offer>(EntityState.Modified);
                var badOffers = await GetNotValidOfferViewModelsAsync(OfferViewModels);
                if (badOffers.Count == 0)
                {
                    var status = await _bankEntities.SaveChangesWithValidationAsync(CancellationToken.None);
                    if (!status.IsValid)
                    {
                        e.Cancel = true;


                        _dialogService.ShowOkDialog("Ошибка обновления", string.Join("\n", status.EfErrors), m => { });
                        await dbcontext.ReloadAllEntitiesAsync(updatedOffers);
                    }
                }
                else
                {
                    e.Cancel = true;
                    await dbcontext.ReloadAllEntitiesAsync(updatedOffers);
                }
            }
        }

        /// <summary>
        /// Возвращает список оболочек неисправных предложений <see cref="OfferViewModel"/>; асинхронный.
        /// </summary>
        /// <param name="offerInfoViewModels"></param>
        /// <returns>Список неуникальных предложений.</returns>
        private async Task<List<OfferViewModel>> GetNotValidOfferViewModelsAsync(IEnumerable<OfferViewModel> offerInfoViewModels)
        {
            return await offerInfoViewModels.AsAsyncQueryable().Where(vm => vm.IsValid == false).ToListAsync();
        }
    }
}
