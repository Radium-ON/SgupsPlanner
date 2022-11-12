using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SgupsPlanner.Core.ViewModels;
using SgupsPlanner.Core.Views;
using Prism.Ioc;
using Prism.Modularity;
using QuickConverter;

namespace SgupsPlanner.Core
{
    public class CoreModule : IModule
    {
        #region Implementation of IModule

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<OkCancelDialog,OkCancelDialogViewModel>();
            containerRegistry.RegisterDialog<OkDialog,OkDialogViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            //Инициализация QuickConverter
            EquationTokenizer.AddNamespace(typeof(object));
            EquationTokenizer.AddNamespace(typeof(Visibility));
        }

        #endregion
    }
}
