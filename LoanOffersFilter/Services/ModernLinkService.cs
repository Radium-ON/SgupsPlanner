using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoanOffersFilter.Views;
using FirstFloor.ModernUI.Presentation;
using SgupsPlanner.Core.Interfaces;

namespace LoanOffersFilter.Services
{
    public class ModernLinkService : IModernLinkService
    {
        #region Implementation of IModernLinkService

        public Link GetModernLink()
        {
            return new Link
            {
                DisplayName = "Актуальное",
                Source = new Uri($"/LoanOffersFilter;component/Views/{nameof(LoanOffersFilterView)}.xaml", UriKind.Relative)
            };
        }

        #endregion
    }
}
