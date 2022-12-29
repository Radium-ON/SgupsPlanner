using System;
using System.ComponentModel;
using SgupsPlanner.Core.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace EventsEdit.ViewModels
{
    public class BankAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private DelegateCommand _addBankCommand;

        public DelegateCommand AddBankCommand =>
            _addBankCommand ??= new DelegateCommand(AddClientToContext, CanAddClient);
        
        #endregion

        #region Bindable Properties

        private string _title = "Новый банк";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private BankViewModel _bankViewModel;
        public BankViewModel BankViewModel
        {
            get => _bankViewModel;
            set => SetProperty(ref _bankViewModel, value);
        }

        #endregion

        private void AddClientToContext()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters { { "AddedBankViewModel", BankViewModel } }));
        }

        private bool CanAddClient()
        {
            return BankViewModel == null;
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
            BankViewModel = parameters.GetValue<BankViewModel>(nameof(BankViewModel));
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => (BankViewModel as IDataErrorInfo)[columnName];

        public string Error => (BankViewModel as IDataErrorInfo).Error;

        #endregion
    }
}
