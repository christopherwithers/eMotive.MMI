﻿using System.Collections.Generic;
using eMotive.Models.Objects.Reports.Users;

namespace eMotive.Services.Interfaces
{
    public interface IReportService
    {
        IEnumerable<SCEReportItem> FetchAllSCEs();
        IEnumerable<SCEReportItem> FetchUsersNotSignedUp();
        IEnumerable<SCEReportItem> FetchSCEData(IEnumerable<int> _userIds);
        IEnumerable<SCEReportItem> FetchApplicantData(IEnumerable<int> _userIds);

        IEnumerable<InterviewerReportItem> FetchAllInterviewers();
        IEnumerable<InterviewerReportItem> FetchInterviewersNotSignedUp();
        IEnumerable<InterviewerReportItem> FetchAllObservers();
        IEnumerable<InterviewerReportItem> FetchObserversNotSignedUp();
        IEnumerable<InterviewerReportItem> FetchAllInterviewersAndObservers();
        IEnumerable<InterviewerReportItem> FetchInterviewersAndObserversNotSignedUp();
    }
}
