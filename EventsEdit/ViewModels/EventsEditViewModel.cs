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
using System.Windows.Input;
using EventsEdit.Services;
using EventsEdit.Views;
using FirstFloor.ModernUI.Windows.Navigation;
using SgupsPlanner.Core.Extensions;
using SgupsPlanner.Core.ViewModels;
using Prism.Commands;
using Prism.Services.Dialogs;
using SgupsPlanner.Core.DtoMappers;
using SgupsPlanner.Core.ResponseWrapper;
using SgupsPlanner.Database.Database;
using SgupsPlanner.Database.Entities;

namespace EventsEdit.ViewModels
{
    public class EventsEditViewModel : ModernViewModelBase
    {
        #region Backing Fields

        private AsyncObservableCollection<EventEditViewModel> _bankViewModels;

        private EventEditOptions _options;
        private readonly IDialogService _dialogService;
        private EventEditViewModel _selectedBankViewModel;

        #endregion

        public EventsEditViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            DeleteBankCommand = new DelegateCommand<EventEditViewModel>(async vm => await DeleteSelectedBankAsync(vm));
            AddBankCommand = new DelegateCommand(ShowAddBankDialog);
            //DeleteOfferFromBankCommand = new DelegateCommand<Offer>(async offer => await DeleteSelectedOfferAsync(offer));
            AddOfferToBankCommand = new DelegateCommand(ShowAddOfferToBankDialog, CanShowAddOfferToBankDialog).ObservesProperty(() => SelectedBankViewModel);

            BankViewModels = new AsyncObservableCollection<EventEditViewModel>();

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
                    { "EventViewModel", SelectedBankViewModel },
                    {"DbContext", _bankEntities}
                },
                r => { });
        }

        private async Task DeleteSelectedOfferAsync(EventDto offer)
        {
            if (SelectedBankViewModel != null)
            {
                //SelectedBankViewModel.Offers.Remove(offer);
                //await _bankEntities.SaveChangesAsync(CancellationToken.None);
            }
        }

        private void ShowAddBankDialog()
        {
            _dialogService.ShowDialog(nameof(BankAddingDialog), new DialogParameters { { "EventViewModel", new EventEditViewModel(new EventDto()) } },
                async r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        var addedBankVm = r.Parameters.GetValue<EventEditViewModel>("AddedBankViewModel");

                        //_bankEntities.Banks.Add(addedBankVm.Entity);
                        //await _bankEntities.SaveChangesAsync(CancellationToken.None);

                        BankViewModels.Add(addedBankVm);
                    }
                });
        }



        public AsyncObservableCollection<EventEditViewModel> BankViewModels
        {
            get => _bankViewModels;
            set => SetProperty(ref _bankViewModels, value);
        }

        public EventEditViewModel SelectedBankViewModel
        {
            get => _selectedBankViewModel;
            set => SetProperty<EventEditViewModel>(ref _selectedBankViewModel, value);
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
            Debug.WriteLine("EventsEditViewModel - VisibilityChanged");
        }

        /// <summary>
        /// Вызывается после события Loaded связанного view.
        /// </summary>
        private async Task LoadDataAsync()
        {
            _bankEntities = new DataBaseEvent();
            BankViewModels.Clear();
            var listDto = await _bankEntities.SelectAllAsync(false);
            BankViewModels.AddRange(EventMapper.ConvertFromListDto(listDto));
            Debug.WriteLine("EventsEditViewModel - LoadData");
        }

        /// <summary>
        /// Вызывается после перехода к другому view.
        /// </summary>
        private void NavigatedFrom()
        {
            Debug.WriteLine("EventsEditViewModel - NavigatedFrom");
        }

        /// <summary>
        /// Вызывается, когда переходим к представлению, связанному с этой viewmodel.
        /// </summary>
        private void NavigatedTo()
        {
            Debug.WriteLine("EventsEditViewModel - NavigatedTo");
        }

        /// <summary>
        /// Навигация фрагментов.
        /// </summary>
        private void FragmentNavigation()
        {
            Debug.WriteLine("EventsEditViewModel - FragmentNavigation");
        }

        /// <summary>
        /// Вызывается, когда переходим на новое view.
        /// </summary>
        private void NavigatingFrom(NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            var dbcontext = _bankEntities;
            var hasChanges = false;

            //if (hasChanges == true)
            //{
            //    _dialogService.ShowOkCancelDialog(
            //        Application.Current.FindResource("some_data_changed_dialog_title") as string,
            //        Application.Current.FindResource("some_data_changed_dialog_message") as string,
            //        async r => { await NavigatingWithModifiedBanksCallBackAsync(r, navigatingCancelEventArgs, dbcontext); });
            //}
            Debug.WriteLine("EventsEditViewModel - NavigatingFrom");
        }

        #endregion

        private async Task DeleteSelectedBankAsync(EventEditViewModel bankVm)
        {
            if (bankVm == null) return;

            //if (bankVm.Entity.LoanAgreements.Count == 0 && bankVm.Entity.Offers.Count == 0)
            //{
            //    await RemoveBankAsync(bankVm);
            //}
            //else
            //{
            //    _dialogService.ShowOkCancelDialog(
            //        Application.Current.FindResource("bank_deleted_dialog_title") as string,
            //        Application.Current.FindResource("bank_deleted_dialog_message") as string,
            //        async r =>
            //        {
            //            if (r.Result == ButtonResult.OK)
            //            {
            //                await RemoveBankAsync(bankVm);
            //            }
            //        });
            //}
        }

        private async Task RemoveBankAsync(EventEditViewModel bankVm)
        {
            //_bankEntities.Banks.Remove(bankVm.Entity);
            BankViewModels.Remove(bankVm);
            //await _bankEntities.SaveChangesAsync(CancellationToken.None);
        }

        private async Task GetEvents()
        {
            try
            {
                var response = await _historyOptions.GetTrainings(Session.StudentId);
                if (response is Success<List<TrainingObservable>> responseWrapper)
                {
                    Trainings = new ObservableCollection<TrainingObservable>(responseWrapper.Data);
                }
                else
                {
                    var errorMessage = (response as Error)?.Message;
                    Debug.WriteLine("[HistoryViewModel.GetTrainings()] Error: " + errorMessage);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }

        }

        private async Task NavigatingWithModifiedBanksCallBackAsync(IDialogResult r, NavigatingCancelEventArgs e, DbContext dbcontext)
        {
            if (r.Result == ButtonResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (r.Result == ButtonResult.OK)
            {
                //var updatedBanks = CurrentObjectContext.GetEntriesByEntityState<Bank>(EntityState.Modified);
                var badBanks = await GetNotValidBankViewModelsAsync(BankViewModels);
                if (badBanks.Count == 0)
                {
                    //var status = await _bankEntities.SaveChangesWithValidationAsync(CancellationToken.None);
                    //if (!status.IsValid)
                    //{
                    //    e.Cancel = true;

                    //    //_dialogService.ShowOkDialog("Ошибка обновления", string.Join("\n", status.EfErrors), m => { });
                    //    //await dbcontext.ReloadAllEntitiesAsync(updatedBanks);
                    //}
                }
                else
                {
                    e.Cancel = true;
                    //await dbcontext.ReloadAllEntitiesAsync(updatedBanks);
                }
            }
        }

        /// <summary>
        /// Возвращает список оболочек неисправных клиентов <see cref="EventEditViewModel"/>; асинхронный.
        /// </summary>
        /// <param name="bankInfoViewModels"></param>
        /// <returns>Список неуникальных клиентов.</returns>
        private async Task<List<EventEditViewModel>> GetNotValidBankViewModelsAsync(IEnumerable<EventEditViewModel> bankInfoViewModels)
        {
            //return await bankInfoViewModels.AsAsyncQueryable().Where(vm => vm.IsValid == false).ToListAsync();
            return new List<EventEditViewModel>();
        }
    }
}
