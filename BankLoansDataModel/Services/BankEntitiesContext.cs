using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankLoansDataModel.Extensions;

namespace BankLoansDataModel.Services
{
    public class BankEntitiesContext : BankLoansEntities, IBankEntitiesContext
    {
        #region Overrides of IBankEnitiesContext

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var changedEntities = ChangeTracker
                .Entries()
                .Where(_ => _.State == EntityState.Added ||
                            _.State == EntityState.Modified);

            var errors = new List<ValidationResult>();
            foreach (var e in changedEntities)
            {
                var vc = new ValidationContext(e.Entity, null, null);
                Validator.TryValidateObject(
                    e.Entity, vc, errors, validateAllProperties: true);
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var changedEntities = ChangeTracker
                .Entries()
                .Where(_ => _.State == EntityState.Added ||
                            _.State == EntityState.Modified);

            var errors = new List<ValidationResult>();
            foreach (var e in changedEntities)
            {
                var vc = new ValidationContext(e.Entity, null, null);
                Validator.TryValidateObject(
                    e.Entity, vc, errors, validateAllProperties: true);
            }
            return base.SaveChanges();
        }

        public async Task<EfStatus> SaveChangesWithValidationAsync(CancellationToken cancellationToken)
        {
            var status = new EfStatus();
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbEntityValidationException ex)
            {
                return status.SetErrors(ex.EntityValidationErrors);
            }
            catch (DbUpdateException ex)
            {
                var decodedErrors = TryDecodeDbUpdateException(ex);
                if (decodedErrors == null)
                    throw;  //падаем, если что-то неизвестное
                return status.SetErrors(decodedErrors);
            }

            return status;
        }

        #endregion

        private static readonly Dictionary<int, string> _sqlErrorTextDict =
            new Dictionary<int, string>
            {
                [547] = "Операция провалена, так как другая сущность использует эту сущность.",
                [2601] = "Одно из свойств помечено как уникальное и уже есть сущность с таким значением.",
                [2627] = "",
                [3621] = "Операция прервана."
            };

        /// <summary>
        /// This decodes the DbUpdateException. If there are any errors it can
        /// handle then it returns a list of errors. Otherwise it returns null
        /// which means rethrow the error as it has not been handled
        /// </summary>
        /// <param name="exception">0 исключения уровня SQL сервера</param>
        /// <returns>null if cannot handle errors, otherwise a list of errors</returns>
        private IEnumerable<ValidationResult> TryDecodeDbUpdateException(DbUpdateException exception)
        {
            if (!(exception.InnerException is System.Data.Entity.Core.UpdateException) ||
                !(exception.InnerException.InnerException is System.Data.SqlClient.SqlException))
                return null;
            var sqlException =
                (System.Data.SqlClient.SqlException)exception.InnerException.InnerException;
            var result = new List<ValidationResult>();
            for (var i = 0; i < sqlException.Errors.Count; i++)
            {
                var errorNum = sqlException.Errors[i].Number;
                if (_sqlErrorTextDict.TryGetValue(errorNum, out var errorOutText))
                {
                    if (errorNum == 2627)
                    {
                        var errorText = sqlException.Errors[i].Message;

                        var dbObjectName = errorText.Split('\u0027')[3];
                        var dublicateValue = errorText.Split('(', ')')[1];
                        var resultMessage = $"Нельзя вставить дубликат поля в объект '{dbObjectName}'. Значение дубликата равно ({dublicateValue}).";
                        errorOutText = resultMessage;
                    }
                    result.Add(new ValidationResult(errorOutText));

                }
            }
            return result.Any() ? result : null;
        }
    }
}
