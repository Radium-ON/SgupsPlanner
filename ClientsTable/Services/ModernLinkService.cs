using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientsTable.Views;
using FirstFloor.ModernUI.Presentation;
using SgupsPlanner.Core.Interfaces;

namespace ClientsTable.Services
{
    public class ModernLinkService : IModernLinkService
    {
        #region Implementation of IModernLinkService

        public Link GetModernLink()
        {
            return new Link
            {
                DisplayName = "Клиенты",
                Source = new Uri($"/ClientsTable;component/Views/{nameof(ClientsTableView)}.xaml", UriKind.Relative)
            };
        }

        #endregion
    }
}
