using System;
using System.ComponentModel;
using System.Diagnostics;
using BankLoansDataModel;
using BankLoansDataModel.Services;
using SgupsPlanner.Core.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace OffersTable.ViewModels
{
    public class OfferAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private DelegateCommand _addOfferCommand;
        public DelegateCommand AddOfferCommand =>
            _addOfferCommand ??= new DelegateCommand(AddOfferToContext, CanAddOffer)
                .ObservesProperty(() => OfferViewModel.IsValid)
                .ObservesProperty(() => OfferViewModel.IsUnique);

        private bool CanAddOffer()
        {
            return OfferViewModel == null || OfferViewModel.IsValid && OfferViewModel.IsUnique;
        }

        #endregion

        #region Bindable Properties

        private string _title = "Новое предложение";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private OfferViewModel _offerViewModel;
        public OfferViewModel OfferViewModel
        {
            get => _offerViewModel;
            set => SetProperty(ref _offerViewModel, value);
        }

        #endregion

        private void AddOfferToContext()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters { { "AddedOfferViewModel", OfferViewModel } }));
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

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            OfferViewModel = parameters.GetValue<OfferViewModel>(nameof(OfferViewModel));
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => (OfferViewModel as IDataErrorInfo)[columnName];

        public string Error => (OfferViewModel as IDataErrorInfo).Error;

        #endregion
    }
}