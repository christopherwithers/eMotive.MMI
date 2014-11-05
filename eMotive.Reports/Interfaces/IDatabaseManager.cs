using eMotive.Reports.Objects.Database;

namespace eMotive.Reports.Interfaces
{
    public interface IDatabaseManager
    {
        Database GetDatabase(string name);
    }
}
