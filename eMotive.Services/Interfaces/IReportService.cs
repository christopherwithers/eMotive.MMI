using System.Collections.Generic;
using eMotive.Models.Objects.Reports.Users;

namespace eMotive.Services.Interfaces
{
    public interface IReportService
    {
        IEnumerable<SCEReportItem> FetchAllSCEs();
        IEnumerable<SCEReportItem> FetchUsersNotSignedUp();
        IEnumerable<SCEReportItem> FetchSCEData(IEnumerable<int> _userIds);
    }
}
