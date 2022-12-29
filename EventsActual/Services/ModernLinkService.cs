using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsActual.Views;
using FirstFloor.ModernUI.Presentation;
using SgupsPlanner.Core.Interfaces;

namespace EventsActual.Services
{
    public class ModernLinkService : IModernLinkService
    {
        #region Implementation of IModernLinkService

        public Link GetModernLink()
        {
            return new Link
            {
                DisplayName = "Актуальное",
                Source = new Uri($"/EventsActual;component/Views/{nameof(EventsActualView)}.xaml", UriKind.Relative)
            };
        }

        #endregion
    }
}
