using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLoansDataModel.Extensions
{
    public static class ObjectContextExtensions
    {
        public static IEnumerable<TEntity> GetEntriesByEntityState<TEntity>(this ObjectContext objectContext, EntityState state) where TEntity : class
        {
            var updatedObjects =
                from entry in objectContext.ObjectStateManager.GetObjectStateEntries(state)
                where entry.EntityKey != null
                select entry.Entity as TEntity;
            return updatedObjects;
        }
    }
}
