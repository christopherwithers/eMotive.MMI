using System;
using System.Collections.Generic;
//using System.Linq;
//using eMotive.Models.Objects.Users;
using Extensions;

namespace eMotive.Models.Objects.SignupsMod
{
    public class Slot
    {
        private int _numberSignedUp = -1;

        public int id { get; set; }
        public string Description { get; set; }
        public int PlacesAvailable { get; set; }
        public int ReservePlaces { get; set; }
        public int InterestedPlaces { get; set; }
        public bool Enabled { get; set; }
        public int IdSignUp { get; set; }
        public DateTime Time { get; set; }
        public ICollection<UserSignup> UsersSignedUp { get; set; }

        public int NumberSignedUp()
        {
            if (_numberSignedUp > -1)
                return _numberSignedUp;


            return _numberSignedUp = UsersSignedUp.HasContent() ? UsersSignedUp.Count : 0;
        }
    }
}
