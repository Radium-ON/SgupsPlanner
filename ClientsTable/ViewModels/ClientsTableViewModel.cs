using System;
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
using ClientsTable.Views;
using SgupsPlanner.Core.Extensions;
using SgupsPlanner.Core.ViewModels;
using FirstFloor.ModernUI.Windows.Navigation;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

namespace ClientsTable.ViewModels
{
    public class ClientsTableViewModel : ModernViewModelBase
    {
        #region Backing Fields

        private AsyncObservableCollection<ClientViewModel> _clientViewModels;

        private IBankEntitiesContext _bankEntities;
        private readonly IDialogService _dialogService;

        #endregion

        public ClientsTableViewModel(IBankEntitiesContext bankEntities, IDialogService dialogService)
        {
            _bankEntities = bankEntities;
            _dialogService = dialogService;

            DeleteClientCommand = new DelegateCommand<ClientViewModel>(async vm => await DeleteSelectedClientAsync(vm));
            AddClientCommand = new DelegateCommand(ShowAddClientDialog);

            ClientViewModels = new AsyncObservableCollection<ClientViewModel>();

            NavigatingFromCommand = new DelegateCommand<NavigatingCancelEventArgs>(NavigatingFrom);
            NavigatedFromCommand = new DelegateCommand(NavigatedFrom);
            NavigatedToCommand = new DelegateCommand(NavigatedTo);
            FragmentNavigationCommand = new DelegateCommand(FragmentNavigation);
            LoadedCommand = new DelegateCommand(async () => await LoadDataAsync());
            IsVisibleChangedCommand = new DelegateCommand(VisibilityChanged);
        }

        private void ShowAddClientDialog()
        {
            _dialogService.ShowDialog(nameof(ClientAddingDialog), new DialogParameters { { "ClientViewModel", new ClientViewModel(new Client(), _bankEntities) } },
                async r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        var addedClientVm = r.Parameters.GetValue<ClientViewModel>("AddedClientViewModel");

                        _bankEntities.Clients.Add(addedClientVm.Entity);
                        await _bankEntities.SaveChangesAsync(CancellationToken.None);

                        ClientViewModels.Add(addedClientVm);
                    }
                });
        }



        public AsyncObservableCollection<ClientViewModel> ClientViewModels
        {
            get => _clientViewModels;
            set => SetProperty(ref _clientViewModels, value);
        }

        public ObjectContext CurrentObjectContext => ((IObjectContextAdapter)_bankEntities).ObjectContext;

        #region DelegateCommands

        public DelegateCommand<ClientViewModel> DeleteClientCommand { get; private set; }
        public DelegateCommand AddClientCommand { get; private set; }

        #endregion

        #region NavigationEvents Methods

        /// <summary>
        /// Вызывается после события IsVisibleChanged связанного view.
        /// </summary>
        private void VisibilityChanged()
        {
            Debug.WriteLine("ClientsTableViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadDataAsync()
        {
            _bankEntities = new BankEntitiesContext();
            ClientViewModels.Clear();
            ClientViewModels.AddRange(await GetOfferInfoViewModelsAsync(_bankEntities.Clients));
            Debug.WriteLine("ClientsTableViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("ClientsTableViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("ClientsTableViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("ClientsTableViewModel - FragmentNavigation");
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
                    async r => { await NavigatingWithModifiedClientsCallBackAsync(r, navigatingCancelEventArgs, dbcontext); });
            }
            Debug.WriteLine("ClientsTableViewModel - NavigatingFrom");
        }

        #endregion

        private async Task DeleteSelectedClientAsync(ClientViewModel clientVm)
        {
            if (clientVm == null) return;

            if (clientVm.Entity.LoanAgreements.Count == 0)
            {
                await RemoveClientWithViewModelAsync(clientVm);
            }
            else
            {
                _dialogService.ShowOkCancelDialog(
                    Application.Current.FindResource("client_deleted_dialog_title") as string,
                    Application.Current.FindResource("client_deleted_dialog_message") as string,
                    async r =>
                    {
                        if (r.Result == ButtonResult.OK)
                        {
                            await RemoveClientWithViewModelAsync(clientVm);
                        }
                    });
            }
        }

        private async Task RemoveClientWithViewModelAsync(ClientViewModel clientVm)
        {
            _bankEntities.Clients.Remove(clientVm.Entity);
            ClientViewModels.Remove(clientVm);
            await _bankEntities.SaveChangesAsync(CancellationToken.None);
        }

        private async Task<IEnumerable<ClientViewModel>> GetOfferInfoViewModelsAsync(IDbSet<Client> clients)
        {
            await clients.LoadAsync();
            return clients.Local.Select(client => new ClientViewModel(client, _bankEntities));
        }

        private async Task NavigatingWithModifiedClientsCallBackAsync(IDialogResult r, NavigatingCancelEventArgs e, DbContext dbcontext)
        {
            if (r.Result == ButtonResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (r.Result == ButtonResult.OK)
            {
                var updatedClients = CurrentObjectContext.GetEntriesByEntityState<Client>(EntityState.Modified);
                var badClients = await GetNotValidClientViewModelsAsync(ClientViewModels);
                if (badClients.Count == 0)
                {
                    var status = await _bankEntities.SaveChangesWithValidationAsync(CancellationToken.None);
                    if (!status.IsValid)
                    {
                        e.Cancel = true;

                        _dialogService.ShowOkDialog("Ошибка обновления", string.Join("\n", status.EfErrors), m => { });
                        await dbcontext.ReloadAllEntitiesAsync(updatedClients);
                    }
                }
                else
                {
                    e.Cancel = true;
                    await dbcontext.ReloadAllEntitiesAsync(updatedClients);
                }
            }
        }

        /// <summary>
        /// Возвращает список оболочек неисправных клиентов <see cref="ClientViewModel"/>; асинхронный.
        /// </summary>
        /// <param name="clientInfoViewModels"></param>
        /// <returns>Список неуникальных клиентов.</returns>
        private async Task<List<ClientViewModel>> GetNotValidClientViewModelsAsync(IEnumerable<ClientViewModel> clientInfoViewModels)
        {
            return await clientInfoViewModels.AsAsyncQueryable().Where(vm => vm.IsValid == false).ToListAsync();
        }
    }
}
