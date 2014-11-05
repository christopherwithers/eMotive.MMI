using System.Collections.Generic;

namespace eMotive.Reports.Objects.Database
{
    public class Database
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<Table> Tables { get; set; } 
    }
}
