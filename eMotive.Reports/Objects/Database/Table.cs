using System.Collections.Generic;

namespace eMotive.Reports.Objects.Database
{
    public class Table
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<Column> Columns { get; set; } 
    }
}
