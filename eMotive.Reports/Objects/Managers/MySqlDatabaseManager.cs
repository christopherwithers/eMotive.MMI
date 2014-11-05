using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Dapper;
using eMotive.Reports.Interfaces;
using eMotive.Reports.Objects.Database;
using Extensions;
using MySql.Data.MySqlClient;

namespace eMotive.Reports.Objects.Managers
{
    public class MySqlDatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public MySqlDatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal IDbConnection Connection
        {
            get { return _connection ?? (_connection = new MySqlConnection(_connectionString)); }
        }

        private class InformationSchema
        {
            public string Table_Schema { get; set; } 
            public string Table_Name { get; set; }
            public string Column_Name { get; set; }
            public string Data_Type { get; set; }
        }

        public Database.Database GetDatabase(string name)
        {
            using (var conn = Connection)
            {
                    Database.Database database = null;
                    var sql = "SELECT `Table_Schema`, `Table_Name`, `Column_Name`, `Data_Type` FROM `information_schema`.`columns` WHERE `table_schema` = 'mminew' ORDER BY `table_name`,`ordinal_position`;";

                    var results = conn.Query<InformationSchema>(sql);//, new { name = "mminew" });

                    if (results.HasContent())
                    {
                        database = new Database.Database
                        {
                            ID = 0,
                            Name = "mminew"
                        };

                        var tables = new Collection<Table>();

                        foreach (var result in results)
                        {
                            if (tables.All(n => n.Name != result.Table_Name))
                            {
                                tables.Add(new Table { Name = result.Table_Name});
                            }
                        }

                      //  var columnDict = results.ToDictionary(k => k.Table_Name, v => new Column {Field = v.Column_Name, Type = v.Data_Type});
                        var columnDict = results.GroupBy(n => n.Table_Name).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var table in tables)
                        {
                            table.Columns = columnDict[table.Name].Select(n => new Column {Field = n.Column_Name, Type = n.Data_Type});
                        }

                        database.Tables = tables;
                    }

                    return database;
                }
            }

        
    }
}
