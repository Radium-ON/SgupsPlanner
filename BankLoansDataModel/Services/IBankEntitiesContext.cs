using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using BankLoansDataModel.Extensions;

namespace BankLoansDataModel.Services
{
    public interface IBankEntitiesContext
    {
        DbSet<Bank> Banks { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<LoanAgreement> LoanAgreements { get; set; }
        DbSet<Offer> Offers { get; set; }
        
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<EfStatus> SaveChangesWithValidationAsync(CancellationToken cancellationToken);
    }
}