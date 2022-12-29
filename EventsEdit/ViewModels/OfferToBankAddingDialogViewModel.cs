using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using FirstFloor.ModernUI.Presentation;
using SgupsPlanner.Core.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace EventsEdit.ViewModels
{
    public class OfferToBankAddingDialogViewModel : BindableBase, IDialogAware
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private ICommand _addOffersToBankCommand;
        public ICommand AddOffersToBankCommand =>
            _addOffersToBankCommand ??= new RelayCommand(async o => await AddOffersToBankContextAsync(), CanAddOffersToBank);

        private DelegateCommand<SelectionChangedEventArgs> _selectOffersCommand;
        public DelegateCommand<SelectionChangedEventArgs> SelectOffersCommand =>
            _selectOffersCommand ??= new DelegateCommand<SelectionChangedEventArgs>(OnSelectionChanged);

        #endregion

        private void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (Offer item in e.RemovedItems)
            {
                OffersToAdd.Remove(item);
            }

            foreach (Offer item in e.AddedItems)
            {
                OffersToAdd.Add(item);
            }
        }

        private bool CanAddOffersToBank(object o)
        {
            if (OffersToAdd != null)
            {
                return OffersToAdd.Count > 0;
            }

            return false;
        }


        #region Bindable Properties

        private string _title = "Добавление предложений для банка";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private ICollectionView _offersView;
        public ICollectionView OffersView
        {
            get => _offersView;
            set => SetProperty(ref _offersView, value);
        }

        private BankViewModel _bankViewModel;
        public BankViewModel BankViewModel
        {
            get => _bankViewModel;
            set => SetProperty(ref _bankViewModel, value);
        }

        private ObservableCollection<Offer> _offersToAdd;
        public ObservableCollection<Offer> OffersToAdd
        {
            get => _offersToAdd;
            set => SetProperty(ref _offersToAdd, value);
        }

        private IBankEntitiesContext DbContext { get; set; }

        #endregion

        private async Task AddOffersToBankContextAsync()
        {
            foreach (var newOffer in OffersToAdd.Where(newOffer => BankViewModel.Offers.FirstOrDefault(o => o.PK_OfferId == newOffer.PK_OfferId) == null))
            {
                BankViewModel.Offers.Add(newOffer);
            }

            await DbContext.SaveChangesAsync(CancellationToken.None);
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog(string parameter)
        {
            var result = parameter?.ToLower() switch
            {
                "true" => ButtonResult.OK,
                "false" => ButtonResult.Cancel,
                _ => ButtonResult.None
            };

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual async void OnDialogOpened(IDialogParameters parameters)
        {
            BankViewModel = parameters.GetValue<BankViewModel>(nameof(BankViewModel));
            DbContext = parameters.GetValue<IBankEntitiesContext>(nameof(DbContext));
            OffersToAdd = new ObservableCollection<Offer>();
            await DbContext.Offers.LoadAsync();
            OffersView = CollectionViewSource.GetDefaultView(DbContext.Offers.Local);
        }
    }
}