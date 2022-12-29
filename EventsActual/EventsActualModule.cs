using EventsActual.ViewModels;
using EventsActual.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace EventsActual
{
    public class EventsActualModule : IModule
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