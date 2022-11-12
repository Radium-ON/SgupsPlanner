using FirstFloor.ModernUI.Windows.Navigation;
using Prism.Commands;
using Prism.Mvvm;

namespace SgupsPlanner.Core.ViewModels
{
    public abstract class ModernViewModelBase : BindableBase
    {
        /// <summary>
        /// Gets or sets the navigating from command.
        /// </summary>
        /// <value>The navigating from command.</value>
        public DelegateCommand<NavigatingCancelEventArgs> NavigatingFromCommand { get; set; }

        /// <summary>
        /// Gets or sets the navigated from command.
        /// </summary>
        /// <value>The navigated from command.</value>
        public DelegateCommand NavigatedFromCommand { get; set; }

        /// <summary>
        /// Gets or sets the navigated to command.
        /// </summary>
        /// <value>The navigated to command.</value>
        public DelegateCommand NavigatedToCommand { get; set; }

        /// <summary>
        /// Gets or sets the fragment navigation command.
        /// </summary>
        /// <value>The fragment navigation command.</value>
        public DelegateCommand FragmentNavigationCommand { get; set; }

        /// <summary>
        /// Gets or sets the loaded command.
        /// </summary>
        /// <value>The loaded command.</value>
        public DelegateCommand LoadedCommand { get; set; }

        /// <summary>
        /// Gets or sets the is visible changed command.
        /// </summary>
        /// <value>The is visible changed command.</value>
        public DelegateCommand IsVisibleChangedCommand { get; set; }
    }
}
