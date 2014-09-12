using System.Collections.Generic;
using eMotive.Models.Objects.Search;

namespace eMotive.Models.Objects.Users
{
    public class UserSearch : BasicSearch
    {
        public UserSearch()
        {
            ItemType = "Users";

            RoleFilter = new[] {"All", "Admin", "Interviewer", "Applicant"};
        }

        public IEnumerable<User> Users { get; set; }

        public string[] RoleFilter { get; set; }
        public string SelectedRoleFilter { get; set; }
       // public bool CanCreate { get; set; }
    }
}
