using EventsEdit.ViewModels;
using EventsEdit.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace EventsEdit
{
    public class EventsEditModule : IModule
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