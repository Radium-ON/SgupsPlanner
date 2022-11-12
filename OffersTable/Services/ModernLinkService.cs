using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OffersTable.Views;
using FirstFloor.ModernUI.Presentation;
using SgupsPlanner.Core.Interfaces;

namespace OffersTable.Services
{
    public class ModernLinkService : IModernLinkService
    {
        #region Implementation of IModernLinkService

        public Link GetModernLink()
        {
            return new Link
            {
                DisplayName = "Предложения",
                Source = new Uri($"/OffersTable;component/Views/{nameof(OffersTableView)}.xaml", UriKind.Relative)
            };
        }

        #endregion
    }
}
