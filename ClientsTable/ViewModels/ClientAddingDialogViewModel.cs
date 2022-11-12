using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SgupsPlanner.Core.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace ClientsTable.ViewModels
{
    public class ClientAddingDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        #region DelegateCommands

        private DelegateCommand<string> _closeDialogCommand;

        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand<string>(CloseDialog);

        private DelegateCommand _addClientCommand;
        public DelegateCommand AddClientCommand =>
            _addClientCommand ??= new DelegateCommand(AddClientToContext, CanAddClient)
                .ObservesProperty(() => ClientViewModel.IsValid)
                .ObservesProperty(() => ClientViewModel.IsUnique);
        
        #endregion

        #region Bindable Properties

        private string _title = "Новый клиент";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private ClientViewModel _clientViewModel;
        public ClientViewModel ClientViewModel
        {
            get => _clientViewModel;
            set => SetProperty(ref _clientViewModel, value);
        }

        #endregion

        private void AddClientToContext()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters { { "AddedClientViewModel", ClientViewModel } }));
        }

        private bool CanAddClient()
        {
            return ClientViewModel == null || ClientViewModel.IsValid && ClientViewModel.IsUnique;
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
            ClientViewModel = parameters.GetValue<ClientViewModel>(nameof(ClientViewModel));
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName] => (ClientViewModel as IDataErrorInfo)[columnName];

        public string Error => (ClientViewModel as IDataErrorInfo).Error;

        #endregion
    }
}
