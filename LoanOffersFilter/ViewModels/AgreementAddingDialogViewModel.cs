using System;
using System.ComponentModel;
using BankLoansDataModel;
using SgupsPlanner.Core.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace LoanOffersFilter.ViewModels
{
    public class AgreementAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private DelegateCommand _addAgreementCommand;
        public DelegateCommand AddAgreementCommand =>
            _addAgreementCommand ??= new DelegateCommand(AddAgreementToContext, CanAddAgreement)
                .ObservesProperty(() => AgreementViewModel.IsValid)
                .ObservesProperty(() => AgreementViewModel.IsUnique);

        #endregion

        #region Bindable Properties

        private string _title = "Новый кредитный договор";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private AgreementViewModel _agreementViewModel;
        public AgreementViewModel AgreementViewModel
        {
            get => _agreementViewModel;
            set => SetProperty(ref _agreementViewModel, value);
        }

        private Offer _currentOffer;
        public Offer CurrentOffer
        {
            get => _currentOffer;
            set => SetProperty(ref _currentOffer, value);
        }

        private Bank _selectedBank;
        public Bank SelectedBank
        {
            get => _selectedBank;
            set => SetProperty(ref _selectedBank, value);
        }


        #endregion

        private void AddAgreementToContext()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters { { "AddedAgreementViewModel", AgreementViewModel } }));
        }

        private bool CanAddAgreement()
        {
            return AgreementViewModel == null || AgreementViewModel.IsValid && AgreementViewModel.IsUnique;
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
            AgreementViewModel = parameters.GetValue<AgreementViewModel>(nameof(SgupsPlanner.Core.ViewModels.AgreementViewModel));
            CurrentOffer = parameters.GetValue<Offer>(nameof(CurrentOffer));
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => (AgreementViewModel as IDataErrorInfo)[columnName];

        public string Error => (AgreementViewModel as IDataErrorInfo).Error;

        #endregion
    }
}
