using System.Windows;
using OffersTable.ViewModels;
using OffersTable.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using QuickConverter;

namespace OffersTable
{
    public class OffersTableModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //Инициализация QuickConverter
            EquationTokenizer.AddNamespace(typeof(object));
            EquationTokenizer.AddNamespace(typeof(Visibility));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<OfferAddingDialog, OfferAddingDialogViewModel>();
        }
    }
}