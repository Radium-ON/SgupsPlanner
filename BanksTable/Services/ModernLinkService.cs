using System;
using BanksTable.Views;
using FirstFloor.ModernUI.Presentation;
using SgupsPlanner.Core.Interfaces;

namespace BanksTable.Services
{
    public class ModernLinkService : IModernLinkService
    {
        #region Implementation of IModernLinkService

        public Link GetModernLink()
        {
            return new Link
            {
                DisplayName = "Банки",
                Source = new Uri($"/BanksTable;component/Views/{nameof(BanksTableView)}.xaml", UriKind.Relative)
            };
        }

        #endregion
    }
}
