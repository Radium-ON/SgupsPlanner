using ClientsTable.ViewModels;
using ClientsTable.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ClientsTable
{
    public class ClientsTableModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<ClientAddingDialog, ClientAddingDialogViewModel>();
        }
    }
}