using System;
using System.IO;

namespace Threax.Sqlite.Ext
{
    public class SqliteFileExtensions
    {
        private static String DataSourceStart = "Data Source=";
        private static String DataSourceEnd = ";";
        private static char DataSourceEndChar = ';';

        /// <summary>
        /// This function will try to create a file from connection strings in the format:
        /// Data Source=Path;
        /// </summary>
        /// <param name="connectionString"></param>
        public static void TryCreateFile(string connectionString)
        {
            if (connectionString.StartsWith(DataSourceStart) && connectionString.EndsWith(DataSourceEnd))
            {
                var file = connectionString.Substring(DataSourceStart.Length);
                file = file.TrimEnd(DataSourceEndChar);
                file = Path.GetFullPath(file);
                if (!File.Exists(file))
                {
                    var dir = Path.GetDirectoryName(file);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    using (var stream = File.Create(file)) { }
                }
            }
        }
    }
}
