using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using Cache.Interfaces;
using eMotive.Managers.Objects.Signups;
using eMotive.Models.Objects;
using eMotive.Models.Objects.Users;
using Extensions;
using eMotive.Managers.Interfaces;
using eMotive.Models.Objects.Signups;
using eMotive.Repository.Interfaces;
using eMotive.Services.Interfaces;
using Ninject;
using rep = eMotive.Repository.Objects.Signups;

namespace eMotive.Managers.Objects
{
    public class SessionManager : ISessionManager
    {
        private readonly ISessionRepository signupRepository;
        private readonly IUserManager userManager;

        public SessionManager(ISessionRepository _signupRepository, IUserManager _userManager)
        {
            signupRepository = _signupRepository;
            userManager = _userManager;

            AutoMapperManagerConfiguration.Configure();
        }

        [Inject]
        public IeMotiveConfigurationService configurationService { get; set; }
        [Inject]
        public INotificationService notificationService { get; set; }
        [Inject]
        public IEmailService emailService { get; set; }
        [Inject]
        public ICache cache { get; set; }

        readonly Dictionary<int, object> dictionary = new Dictionary<int, object>();
        readonly object dictionaryLock = new object();

        private IEnumerable<Repository.Objects.Signups.Signup> FetchSignupsByGroup(IEnumerable<int> _groups)
        {
            var cacheId = string.Format("RepSignups", string.Join("_", _groups));

            var signups = cache.FetchCollection<Repository.Objects.Signups.Signup>(cacheId, n => n.id, null);

            if (signups.HasContent())
                return signups;

            signups = signupRepository.FetchSignupsByGroup(_groups);

            cache.PutCollection(signups, n => n.id, cacheId);

            return signups;
        }

        //todo: closed date logic here
        public Signup Fetch(int _id)
        {
            var cacheId = string.Format("ModelSignup_{0}", _id);

            var signup = cache.FetchItem<Signup>(cacheId);

            if (signup != null)
                return signup;

            var repSignup = signupRepository.FetchSignup(_id);


            if (repSignup == null)
            {
                notificationService.AddError("The requested signup could not be found.");
                return null;
            }

            signup = Mapper.Map<rep.Signup, Signup>(repSignup);

            IDictionary<int, User> usersDict = null;

            if (repSignup.Slots.Any(n => n.UsersSignedUp.HasContent()))
            {
                //  usersDict = new Dictionary<int, User>();
                var UsersSignedUp = repSignup.Slots.Where(n => n.UsersSignedUp != null).SelectMany(m => m.UsersSignedUp);//.Select(u => u.IdUser);
                var userIds = UsersSignedUp.Select(u => u.IdUser);
                var users = userManager.Fetch(userIds);
                usersDict = users.ToDictionary(k => k.ID, v => v);
            }

            foreach (var repSlot in repSignup.Slots)
            {
                foreach (var slot in signup.Slots)
                {
                    if (repSlot.id != slot.ID) continue;

                    if (!repSlot.UsersSignedUp.HasContent()) continue;

                    slot.ApplicantsSignedUp = new Collection<UserSignup>();
                    foreach (var user in repSlot.UsersSignedUp)
                    {
                        slot.ApplicantsSignedUp.Add(new UserSignup { User = usersDict[user.IdUser], SignupDate = user.SignUpDate, ID = user.ID });
                    }
                }
            }


            return signup;
        }

        //TODO: need a new signup admin obj which contains full user + signup date etc! Then map to it!
        public IEnumerable<Signup> FetchAll()
        {
            var cacheId = "ModelSignupCollection";

            var signupModels = cache.FetchCollection<Signup>(cacheId, i => i.ID, null);//TODO: need a fetch (ids) func to push in here?

            if (signupModels.HasContent())
                return signupModels;

            var signups = signupRepository.FetchAll();

            if (!signups.HasContent())
                return null;

            signupModels = Mapper.Map<IEnumerable<rep.Signup>, IEnumerable<Signup>>(signups);

            var usersDict = userManager.Fetch(signups.SelectMany(u => u.Slots.Where(n => n.UsersSignedUp.HasContent()).SelectMany(n => n.UsersSignedUp).Select(m => m.IdUser))).ToDictionary(k => k.ID, v => v);

            foreach (var repSignup in signups)
            {
                foreach (var modSignup in signupModels)
                {
                    foreach (var repSlot in repSignup.Slots)
                    {
                        foreach (var slot in modSignup.Slots)
                        {
                            if (repSlot.id != slot.ID) continue;

                            if (!repSlot.UsersSignedUp.HasContent()) continue;

                            slot.ApplicantsSignedUp = new Collection<UserSignup>();
                            foreach (var user in repSlot.UsersSignedUp)
                            {
                                slot.ApplicantsSignedUp.Add(new UserSignup { User = usersDict[user.IdUser], SignupDate = user.SignUpDate, ID = user.ID });
                            }
                        }
                    }
                }
            }

            cache.PutCollection(signupModels, i => i.ID, cacheId);

            return signupModels;
        }

        public IEnumerable<Signup> FetchAllTraining()
        {
            var cacheId = "ModelSignupCollectionTraining";

            var signupModels = cache.FetchCollection<Signup>(cacheId, i => i.ID, null);//TODO: need a fetch (ids) func to push in here?

            if (signupModels.HasContent())
                return signupModels;

            var signups = signupRepository.FetchAllTraining();

            if (!signups.HasContent())
                return null;

            signupModels = Mapper.Map<IEnumerable<rep.Signup>, IEnumerable<Signup>>(signups);

            var usersDict = userManager.Fetch(signups.SelectMany(u => u.Slots.Where(n => n.UsersSignedUp.HasContent()).SelectMany(n => n.UsersSignedUp).Select(m => m.IdUser))).ToDictionary(k => k.ID, v => v);

            foreach (var repSignup in signups)
            {
                foreach (var modSignup in signupModels)
                {
                    foreach (var repSlot in repSignup.Slots)
                    {
                        foreach (var slot in modSignup.Slots)
                        {
                            if (repSlot.id != slot.ID) continue;

                            if (!repSlot.UsersSignedUp.HasContent()) continue;

                            slot.ApplicantsSignedUp = new Collection<UserSignup>();
                            foreach (var user in repSlot.UsersSignedUp)
                            {
                                slot.ApplicantsSignedUp.Add(new UserSignup { User = usersDict[user.IdUser], SignupDate = user.SignUpDate, ID = user.ID });
                            }
                        }
                    }
                }
            }

            cache.PutCollection(signupModels, i => i.ID, cacheId);

            return signupModels;
        }

        public IEnumerable<SessionDay> FetchAllBrief()
        {
            var groups = signupRepository.FetchGroups();

            if (!groups.HasContent())
            {
                notificationService.AddError("An error occurred. Groups could not be found.");
                return null;
            }

            var signups = FetchAll();

            return Mapper.Map<IEnumerable<Signup>, IEnumerable<SessionDay>>(signups);
        }
        
        public UserHomeView FetchHomeView(string _username)
        {
            //todo: fetch user and group
            var user = userManager.Fetch(_username);

            if (user == null)
            {
                //TODo: ERROR MESSAGE HERE!
                return null;
            }

            var profile = userManager.FetchProfile(_username);
            var signups = FetchSignupsByGroup(profile.Groups.Select(n => n.ID));

            if (signups == null)
                return null;

            var homeView = new UserHomeView
            {
                User = user
            };

            foreach (var signup in signups)
            {
                if (!signup.Slots.HasContent())
                    continue;

                foreach (var slot in signup.Slots)
                {
                    if (!slot.UsersSignedUp.HasContent())
                        continue;

                    foreach (var userSignup in slot.UsersSignedUp)
                    {
                        if (userSignup.IdUser != user.ID) continue;

                        var signupDetails = new UserSignupDetails
                        {
                            SignUpDate = signup.Date.AddHours(slot.Time.Hour).AddMinutes(slot.Time.Minute),
                            SignUpDetails = slot.Description,
                            SignedUpSlotID = slot.id,
                            SignupID = signup.id,
                            SignupGroup = new Group { Description = signup.Group.Description, AllowMultipleSignups = signup.Group.AllowMultipleSignups, ID = signup.Group.ID, Name = signup.Group.Name },
                            SignupDescription = signup.Description,
                            Type = GenerateHomeViewSlotStatus(slot, user.ID)// slot.UsersSignedUp.Where(n => n.IdUser == user.ID).Single(t => t.Type)
                        };

                        homeView.SignupDetails.Add(signupDetails);
                    }
                }
            }

            return homeView;
        }

        public bool RegisterAttendanceToSession(SessionAttendance _session)
        {
            return signupRepository.RegisterAttendanceToSession(Mapper.Map<SessionAttendance, rep.SessionAttendance>(_session));
        }

        public UserSignupView FetchSignupInformation(string _username)
        {
            var signupCollection = new Collection<SignupState>();
            var user = userManager.Fetch(_username);

            if (user == null)
            {//TODO: ERROR MESSAGE HERE!!
                return null;
            }

            var profile = userManager.FetchProfile(_username);
            var userSignUp = FetchuserSignups(user.ID, profile.Groups.Select(n => n.ID));
            var signups = FetchSignupsByGroup(profile.Groups.Select(n => n.ID));

            bool signedup = false;
            int signupId = 0;

            if (signups.HasContent())
            {
                //signupCollection
                foreach (var item in signups)
                {//TODO: NEED TO CHECK IF SLOT IS NULL! I>E ANY SLOTS ASSIGNED TO SIGNUP
                    //Logic to deal with applicants and closed signups
                    //if a signup is closed, we hide it from applicants UNLESS they are signed up to a slot in that signup
                    if (!item.Closed || userSignUp != null && userSignUp.Any(n => n.IdSignUp == item.id))
                    {
                        var signup = new SignupState
                        {
                            ID = item.id,
                            Date = item.Date,
                            SignedUp =
                                item.Slots.Any(
                                    n =>
                                    n.UsersSignedUp.HasContent() &&
                                    n.UsersSignedUp.Any(m => m != null && m.IdUser == user.ID)),
                            TotalSlotsAvailable = item.Slots.Sum(n => n.PlacesAvailable),
                            TotalReserveAvailable = item.Slots.Sum(n => n.ReservePlaces),
                            TotalInterestedAvaiable = item.Slots.Sum(n => n.InterestedPlaces),
                            NumberSignedUp = item.Slots.Sum(n => n.UsersSignedUp.HasContent() ? n.UsersSignedUp.Count() : 0),
                            MergeReserve = item.MergeReserve,
                            OverrideClose = item.OverrideClose,
                            DisabilitySignup = item.Group.DisabilitySignups,
                            Closed = item.Closed || item.CloseDate < DateTime.Now,
                            Description = item.Description,
                            Group = new Group { AllowMultipleSignups = item.Group.AllowMultipleSignups, Description = item.Group.Description, ID = item.Group.ID, Name = item.Group.Name}
                        };

                        if (signup.SignedUp)
                        {
                            signedup = true;
                            signupId = signup.ID;
                        }

                        signupCollection.Add(signup);
                    }
                }
            }

            var signupView = new UserSignupView
            {
                SignupInformation = signupCollection,
                SignupID = signupId,
                SignedUp = signedup
            };

            return signupView;

        }

        public UserSignupView FetchSignupInformation(string _username, int _idGroup)
        {
            var signupCollection = new Collection<SignupState>();
            var user = userManager.Fetch(_username);

            if (user == null)
            {//TODO: ERROR MESSAGE HERE!!
                return null;
            }

            var profile = userManager.FetchProfile(_username);


            if (profile.Groups.All(n => n.ID != _idGroup))
            {
                notificationService.AddError("You do not have access to the requested session signup.");
                return null;
            }

            var userSignUp = FetchuserSignups(user.ID, profile.Groups.Select(n => n.ID));
            var signups = FetchSignupsByGroup(profile.Groups.Select(n => n.ID));

            bool signedup = false;
            int signupId = 0;

            if (signups.HasContent())
            {
                //signupCollection
                foreach (var item in signups)
                {
                    //Logic to deal with applicants and closed signups
                    //if a signup is closed, we hide it from applicants UNLESS they are signed up to a slot in that signup
                    if (!item.Closed || userSignUp != null && userSignUp.Any(n => n.IdSignUp == item.id))
                    {
                        var signup = new SignupState
                        {
                            ID = item.id,
                            Date = item.Date,
                            SignedUp =
                                item.Slots.Any(
                                    n =>
                                    n.UsersSignedUp.HasContent() &&
                                    n.UsersSignedUp.Any(m => m != null && m.IdUser == user.ID)),
                            TotalSlotsAvailable = item.Slots.Sum(n => n.PlacesAvailable),
                            TotalReserveAvailable = item.Slots.Sum(n => n.ReservePlaces),
                            TotalInterestedAvaiable = item.Slots.Sum(n => n.InterestedPlaces),
                            NumberSignedUp = item.Slots.Sum(n => n.UsersSignedUp.HasContent() ? n.UsersSignedUp.Count() : 0),
                            MergeReserve = item.MergeReserve,
                            OverrideClose = item.OverrideClose,
                            DisabilitySignup = item.Group.DisabilitySignups,
                            Closed = item.Closed || item.CloseDate < DateTime.Now,
                            Description = item.Description,
                            Group = new Group { AllowMultipleSignups = item.Group.AllowMultipleSignups, Description = item.Group.Description, ID = item.Group.ID, Name = item.Group.Name }
                        };

                        if (signup.SignedUp)
                        {
                            signedup = true;
                            signupId = signup.ID;
                        }

                        signupCollection.Add(signup);
                    }
                }
            }

            var signupView = new UserSignupView
            {
                SignupInformation = signupCollection,
                SignupID = signupId,
                SignedUp = signedup
            };

            return signupView;

        }


        public IEnumerable<SignupState> FetchSignupStates(string _username)
        {
            var signupCollection = new Collection<SignupState>();
            var user = userManager.Fetch(_username);

            if (user == null)
            {//TODO: ERROR MESSAGE HERE!!
                return null;
            }

            var profile = userManager.FetchProfile(_username);
            var userSignUp = FetchuserSignup(user.ID, profile.Groups.Select(n => n.ID));
            var signups = FetchSignupsByGroup(profile.Groups.Select(n => n.ID));

            bool signedup = false;
            int signupId = 0;

            if (signups.HasContent())
            {
                //signupCollection
                foreach (var item in signups)
                {
                    //Logic to deal with applicants and closed signups
                    //if a signup is closed, we hide it from applicants UNLESS they are signed up to a slot in that signup
                    if (!item.Closed || userSignUp != null && userSignUp.IdSignUp == item.id)
                    {
                        var signup = new SignupState
                        {
                            ID = item.id,
                            Date = item.Date,
                            SignedUp =
                                item.Slots.Any(
                                    n =>
                                    n.UsersSignedUp.HasContent() &&
                                    n.UsersSignedUp.Any(m => m != null && m.IdUser == user.ID)),
                            TotalSlotsAvailable = item.Slots.Sum(n => n.PlacesAvailable),
                            TotalReserveAvailable = item.Slots.Sum(n => n.ReservePlaces),
                            TotalInterestedAvaiable = item.Slots.Sum(n => n.InterestedPlaces),
                            NumberSignedUp = item.Slots.Sum(n => n.UsersSignedUp.HasContent() ? n.UsersSignedUp.Count() : 0),
                            MergeReserve = item.MergeReserve,
                            OverrideClose = item.OverrideClose,
                            DisabilitySignup = item.Group.DisabilitySignups,
                            Closed = item.Closed || item.CloseDate < DateTime.Now
                        };

                        if (signup.SignedUp)
                        {
                            signedup = true;
                            signupId = signup.ID;
                        }

                        signupCollection.Add(signup);
                    }
                }
            }

            return signupCollection;
        }

        public UserSlotView FetchSlotInformation(int _signup, string _username)
        {
            var user = userManager.Fetch(_username);

            //TODO need id of slot!
            var signup = Fetch(_signup);

            var signupGroup = signupRepository.FetchSignupGroup(_signup);
            
            var userProfile = userManager.FetchProfile(_username);

            if (signupGroup == null || userProfile == null || !userProfile.Groups.HasContent())
            {
                notificationService.AddError("An error occurred. The selected interview date could not be loaded.");

                if (signupGroup == null)
                    notificationService.Log(
                        string.Format("SignupManager: FetchSlotInformation: The signupGroup was null for signup: {0}.",
                            _signup));

                if (userProfile == null)
                    notificationService.Log(
                        string.Format(
                            "SignupManager: FetchSlotInformation: The userProfile was null for username: {0}", _username));

                return null;
            } //why isn't this working now?!?

            var hasAccess = userProfile.Groups.Any(@group => signupGroup.ID == @group.ID);

            if (!hasAccess) //signupGroup.ID != 
            {
                notificationService.AddError("You do not have permission to view the requested interview.");
                return null;
            }

            if (signup == null)
            {
                notificationService.AddError("The requested interview date could not be found.");
                return null;
            }

            var userSignup = FetchuserSignups(user.ID, userProfile.Groups.Select(n => n.ID));

            if (signup.Closed && (userSignup == null || userSignup.Any(n => n.IdSignUp == signup.ID)))
            {
                notificationService.AddError("The requested interview date is clsoed.");
                return null;
            }

            var slotCollection = new Collection<SlotState>();

            //TODO: COULD HAVE A BOOL HERE TO CHECK FOR 1 SIGNUP AGAINST ALL GROUPS OR A SIGNUP PER GROUP? #########################################
            var userSignUp = signupRepository.FetchUserSignups(user.ID, userProfile.Groups.Select(n => n.ID));

            var slotView = new UserSlotView(signup);

            foreach (var item in signup.Slots)
            {
                var slot = new SlotState
                {
                    ID = item.ID,
                    Description = item.Description,
                    Enabled = item.Enabled,
                    NumberSignedUp = item.ApplicantsSignedUp.HasContent() ? item.ApplicantsSignedUp.Count() : 0,
                    TotalPlacesAvailable = item.TotalPlacesAvailable,
                    Status = GenerateSlotStatus(item, new GenerateSlotStatusDTO
                    {
                        Closed = signup.Closed || signup.CloseDate < DateTime.Now,
                        MergeReserve = signup.MergeReserve,
                        MultipleSignupsPerSignup = signup.AllowMultipleSignups,
                        MultipleSignupsPerGroup = signupGroup.AllowMultipleSignups,
                        Group = signup.Group.Name,
                        OverrideClose = signup.OverrideClose,
                        Username = _username,
                        UsersSignups = userSignUp,
                        SignupID = signup.ID,
                        SignupDate = signup.Date
                    }),
                    Closed = signup.Closed || signup.CloseDate < DateTime.Now,
                    OverrideClose = signup.OverrideClose,
                    MergeReserve = signup.MergeReserve,
                    TotalInterestedAvaiable = item.InterestedPlaces,
                    TotalReserveAvailable = item.ReservePlaces
                };
               

                slotCollection.Add(slot);
            }

            slotView.Description = signup.Description;
            slotView.SlotState = slotCollection;
            slotView.HasSignedUp = signup.Slots.FirstOrDefault(n => n.ApplicantsSignedUp != null && n.ApplicantsSignedUp.Any(u => String.Equals(u.User.Username, _username, StringComparison.CurrentCultureIgnoreCase))) != null;


            return slotView;
        }


        public bool SignupToSlot(int _signupID, int _slotId, string _username)
        {
            var signup = Fetch(_signupID);

            var slot = signup.Slots.SingleOrDefault(n => n.ID == _slotId);

            if (slot == null)
            {
                notificationService.AddError(string.Format("The requested slot ({0}) could not be found for signup {1}.", _slotId, _signupID));
                return false;
            }

            var user = userManager.Fetch(_username);
            object bodyLock;
            lock (dictionaryLock)
            {

                if (!dictionary.TryGetValue(_slotId, out bodyLock))
                {
                    bodyLock = new object();
                    dictionary[_slotId] = bodyLock;
                }
            }

            if (signup.Closed)
            {
                notificationService.AddIssue("You cannot sign up to this slot. The sign up is closed.");

                return false;
            }

            if (DateTime.Now > signup.CloseDate)
            {
                notificationService.AddIssue(string.Format("You cannot sign up to this slot. The sign up closed on {0}.", signup.CloseDate.ToString("dddd d MMMM yyyy")));

                return false;
            }

            lock (bodyLock)
            {
                var signupDate = DateTime.Now;

                string error;

                if (slot.ApplicantsSignedUp.HasContent())
                {
                    if (slot.ApplicantsSignedUp.Any(n => String.Equals(n.User.Username, user.Username, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        notificationService.AddIssue("You have already signed up to this slot.");
                        return false;
                    }

                    if (slot.ApplicantsSignedUp.Count() >= slot.TotalPlacesAvailable + slot.ReservePlaces + slot.InterestedPlaces) //TODO: look into this logic, what happens if no interested places have been generated??
                    {
                        notificationService.AddIssue("The selected slot is now full.");
                        return false;
                    }
                }

                int id;
                bool interestedSlot = false;

                if (slot.ApplicantsSignedUp.HasContent())
                    interestedSlot = slot.ApplicantsSignedUp.Count() >= slot.TotalPlacesAvailable + slot.ReservePlaces;

                if (signupRepository.SignupToSlot(_slotId, user.ID, signupDate, out id))
                {
                    // slot.ApplicantsSignedUp.Single(n => n.ID == 0).ID = id;
                    var replacements = new Dictionary<string, string>(4)
                    {
                        {"#forename#", user.Forename},
                        {"#surname#", user.Surname},
                        {"#SignupDate#", signup.Date.ToString("dddd d MMMM yyyy")},
                        {"#SlotDescription#", slot.Description},
                        {"#SignupDescription#", signup.Description},
                        {"#GroupDescription#", signup.Group.Name},
                        {"#username#", user.Username},
                        {"#sitename#", configurationService.SiteName()},
                        {"#siteurl#", configurationService.SiteURL()}
                    };

                    string key = interestedSlot ? "InterestedSignup" : "UserSessionSignup";

                    if (emailService.SendMail(key, user.Email, replacements))
                    {
                        emailService.SendEmailLog(key, user.Username);
                        return true;
                    }
                    return true;
                }

                notificationService.AddError("An error occured. ");
                return false;

            }


        }

        public bool CancelSignupToSlot(int _signupID, int _slotId, string _username)
        {
            var signup = Fetch(_signupID);

            var slot = signup.Slots.SingleOrDefault(n => n.ID == _slotId);

            if (slot == null)
            {
                notificationService.AddError(string.Format("The requested slot ({0}) could not be found for signup {1}.", _slotId, _signupID));
                return false;
            }

            var user = userManager.Fetch(_username);
            object bodyLock;
            lock (dictionaryLock)
            {
                if (!dictionary.TryGetValue(_slotId, out bodyLock))
                {
                    bodyLock = new object();
                    dictionary[_slotId] = bodyLock;
                }
            }

            lock (bodyLock)
            {
                var BumpUser = slot.ApplicantsSignedUp.Count() > slot.TotalPlacesAvailable + slot.ReservePlaces;

                if (signupRepository.CancelSignupToSlot(_slotId, user.ID))
                {
                    /*                    var userSignup = signup.Slots.SingleOrDefault(n => n.ID == _slotId).ApplicantsSignedUp.SingleOrDefault(n => n.Applicant.Username == _username);

                    signup.Slots.SingleOrDefault(n => n.ID == _slotId).ApplicantsSignedUp.Remove(userSignup);
*/
                    /*if (slot.ApplicantsSignedUp().Count() >= slot.ApplicantsSignedUp + slot.ReservePlaces)
                    {
                        
                    }*/
                    var replacements = new Dictionary<string, string>(4)
                    {
                        {"#forename#", user.Forename},
                        {"#surname#", user.Surname},
                        {"#SignupDate#", signup.Date.ToString("dddd d MMMM yyyy")},
                        {"#SlotDescription#", slot.Description},
                        {"#username#", user.Username},
                        {"#SignupDescription#", signup.Description},
                        {"#GroupDescription#", signup.Group.Name},
                        {"#sitename#", configurationService.SiteName()},
                        {"#siteurl#", configurationService.SiteURL()}
                    };

                    string key = "UserSessionCancel";

                    if (emailService.SendMail(key, user.Email, replacements))
                    {
                        emailService.SendEmailLog(key, user.Username);
                        return true;
                    }

                    if (BumpUser)
                    {
                        var users = slot.ApplicantsSignedUp.Select(n => n).OrderBy(n => n.SignupDate).ToArray();

                        var UserToBump = users[slot.TotalPlacesAvailable + slot.ReservePlaces + 1].User;


                        key = "SlotUpgrade";

                        if (emailService.SendMail(key, UserToBump.Email, replacements))
                        {
                            emailService.SendEmailLog(key, UserToBump.Username);
                            return true;
                        }
                    }

                    notificationService.AddError("An error occured. ");
                    return true;
                }

                return false;
            }
        }

        public int FetchRCPActivityCode(int _signupID)
        {
            return signupRepository.FetchRCPActivityCode(_signupID);
        }

        //cache this!
        private rep.UserSignup FetchuserSignup(int _iduser, IEnumerable<int> _groupIds)
        {
            return signupRepository.FetchUserSignup(_iduser, _groupIds);
        }

        private IEnumerable<rep.UserSignup> FetchuserSignups(int _iduser, IEnumerable<int> _groupIds)
        {
            return signupRepository.FetchUserSignups(_iduser, _groupIds);
        }

        virtual public SlotType GenerateHomeViewSlotStatus(rep.Slot _slot, int _userId)
        {
         //   var userPosition = _slot.UsersSignedUp.ToList().FindIndex(n => n.Type ==)
           // throw new NotImplementedException();

        //    if(_slot)

            var userSignup = _slot.UsersSignedUp.SingleOrDefault(n => n.IdUser == _userId);

            //todo: error check incase userSignup is null??

            return (SlotType)userSignup.Type;
        }

        virtual public SlotStatus GenerateSlotStatus(Slot _slot, GenerateSlotStatusDTO _params)
        {

            if (!_slot.Enabled)
                return SlotStatus.Closed;

            if (_params.Closed && !_params.OverrideClose)//checked for overide? - centralise closed logic??
                return SlotStatus.Closed;

            var userIsSignnedUpToCurrentSignup = false;
            var applicantsSignedUp = 0;

            if (_slot.ApplicantsSignedUp.HasContent())
            {
                userIsSignnedUpToCurrentSignup = _slot.ApplicantsSignedUp.Any(n => String.Equals(n.User.Username, _params.Username, StringComparison.CurrentCultureIgnoreCase));
                applicantsSignedUp = _slot.ApplicantsSignedUp.Count();
            }

            var currentSignup = _params.UsersSignups.Select(n => n.IdSlot == _slot.ID);

            if (!_params.MultipleSignupsPerGroup)
            {
                if (userIsSignnedUpToCurrentSignup)
                    return SlotStatus.AlreadySignedUp;

                if (_params.UserHasSignup && !_params.MultipleSignupsPerSignup)
                    return SlotStatus.Clash;

                return SlotStatus.Signup;
            }


            if (!_params.MultipleSignupsPerSignup)
            {
                if (userIsSignnedUpToCurrentSignup)
                    return SlotStatus.AlreadySignedUp;

                if (_params.UserHasSignup)
                    return SlotStatus.Clash;

                return SlotStatus.Signup;
            }


            if (applicantsSignedUp < _slot.TotalPlacesAvailable)
            {
                return SlotStatus.Signup;
            }

            if (applicantsSignedUp < _slot.TotalPlacesAvailable + _slot.ReservePlaces)
            {
                return _params.MergeReserve ? SlotStatus.Signup : SlotStatus.Reserve;
            }

            if (applicantsSignedUp < _slot.TotalPlacesAvailable + _slot.ReservePlaces + _slot.InterestedPlaces)
                return SlotStatus.Interested;

            if (applicantsSignedUp >= _slot.TotalPlacesAvailable + _slot.ReservePlaces + _slot.InterestedPlaces)
                return SlotStatus.Full;

            /*if (_userSignup != null)
                return SlotStatus.Clash;*/

            if (!_slot.ApplicantsSignedUp.HasContent())
                return SlotStatus.Signup;



            return SlotStatus.Signup;//todo: need ERROR here?

        }
    }
}
