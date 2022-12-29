using System;
using EventsEdit.Views;
using FirstFloor.ModernUI.Presentation;
using SgupsPlanner.Core.Interfaces;

namespace EventsEdit.Services
{
    public class ModernLinkService : IModernLinkService
    {
        #region Implementation of IModernLinkService

        public Link GetModernLink()
        {
            return new Link
            {
                DisplayName = "Ваши события",
                Source = new Uri($"/EventsEdit;component/Views/{nameof(EventsEditView)}.xaml", UriKind.Relative)
            };
        }

        #endregion
    }
}
