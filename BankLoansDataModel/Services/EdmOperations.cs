using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLoansDataModel.Services
{
    public static class EdmOperations
    {
        public static async Task InsertNewOfferBankAsync(Offer offer, Bank bank)
        {
            using (var context = new BankLoansEntities())
            {
                //add instances to context
                context.Banks.Add(bank);
                context.Offers.Add(offer);

                // add instance to navigation property
                offer.Banks.Add(bank);

                //call SaveChanges from context to confirm inserts
                await context.SaveChangesAsync();
            }
        }

        public static async Task InsertExistingOfferBankAsync(int offerId, int bankId)
        {
            using (var context = new BankLoansEntities())
            {

                var offer = new Offer { PK_OfferId = offerId };
                context.Offers.Add(offer);
                context.Offers.Attach(offer);

                var bank = new Bank { PK_RegNumber = bankId };
                context.Banks.Add(bank);
                context.Banks.Attach(bank);

                offer.Banks.Add(bank);

                await context.SaveChangesAsync();
            }
        }

        public static List<BankOffersViewModel> GetOffersByBankId(int bankId)
        {
            using (var context = new BankLoansEntities())
            {
                var result = (
                    // состояние из контекста
                    from a in context.Banks
                        // состояние из свойства навигации
                    from b in a.Offers
                        //подключаем полезные данные таблицы
                    join c in context.Offers on b.PK_OfferId equals c.PK_OfferId
                    where a.PK_RegNumber == bankId
                    select new BankOffersViewModel
                    {
                        ID = c.PK_OfferId,
                        BankName = a.Name,
                        InterestRate = c.Interest
                    }).ToList();

                return result;
            }
        }

        public class BankOffersViewModel
        {
            public int ID { get; internal set; }
            public string BankName { get; internal set; }
            public float InterestRate { get; internal set; }
        }
    }
}
