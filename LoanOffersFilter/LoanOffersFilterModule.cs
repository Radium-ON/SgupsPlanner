using LoanOffersFilter.ViewModels;
using LoanOffersFilter.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace LoanOffersFilter
{
    public class LoanOffersFilterModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<AgreementAddingDialog,AgreementAddingDialogViewModel>();
        }
    }
}