using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.Sqlite.Ext.EfCore3
{
    public static class ConvertDbGuidsToEfCore3
    {
        /// <summary>
        /// Convert the database as described at https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.0/breaking-changes.
        /// This may not work well if the data does not match the current model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext">The context to migrate.</param>
        public static void ConvertToEfCore3<T>(this T dbContext) where T : DbContext
        {
            var type = typeof(T);
            var enumerableType = typeof(IEnumerable);

            foreach (var prop in type.GetProperties()
                .Where(i => enumerableType.IsAssignableFrom(i.PropertyType)))
            {
                var propType = prop.PropertyType.GetGenericArguments()[0];

                var mapping = dbContext.Model.FindEntityType(propType);
                var schema = mapping.GetSchema();
                var table = mapping.GetTableName();

                foreach (var columnInfo in propType.GetProperties().Where(i => i.PropertyType == typeof(Guid)))
                {
                    var column = columnInfo.Name;

                    dbContext.Database.ExecuteSqlRaw(
$@"PRAGMA foreign_keys = 0;
UPDATE ""{table}""
SET ""{column}"" = hex(substr(""{column}"", 4, 1)) ||
                 hex(substr(""{column}"", 3, 1)) ||
                 hex(substr(""{column}"", 2, 1)) ||
                 hex(substr(""{column}"", 1, 1)) || '-' ||
                 hex(substr(""{column}"", 6, 1)) ||
                 hex(substr(""{column}"", 5, 1)) || '-' ||
                 hex(substr(""{column}"", 8, 1)) ||
                 hex(substr(""{column}"", 7, 1)) || '-' ||
                 hex(substr(""{column}"", 9, 2)) || '-' ||
                 hex(substr(""{column}"", 11, 6))
WHERE typeof(""{column}"") == 'blob';
PRAGMA foreign_keys = 1;");
                }
            }
        }
    }
}
