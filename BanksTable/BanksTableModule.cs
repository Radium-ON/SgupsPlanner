using BanksTable.ViewModels;
using BanksTable.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace BanksTable
{
    public class BanksTableModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<BankAddingDialog, BankAddingDialogViewModel>();
            containerRegistry.RegisterDialog<OfferToBankAddingDialog, OfferToBankAddingDialogViewModel>();
        }
    }
}