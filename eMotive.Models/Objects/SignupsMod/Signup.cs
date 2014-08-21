using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Extensions;

namespace eMotive.Models.Objects.SignupsMod
{
    public class Signup
    {
        private bool _isSignedUp;
        
        public Signup()
        {
            _isSignedUp = false;
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime CloseDate { get; set; }
        public Group Group { get; set; }
        public string AcademicYear { get; set; }
        public bool Closed { get; set; }
        public bool OverrideClose { get; set; }
        public bool MergeReserve { get; set; }
        public bool AllowMultipleSignups { get; set; }
        public bool IsTraining { get; set; }

        public string Description { get; set; }

        public IEnumerable<Slot> Slots { get; set; }

        public int TotalSlotsAvailable { get; set; }
        public int TotalReserveAvailable { get; set; }
        public int TotalInterestedAvaiable { get; set; }
        public int TotalNumberSignedUp { get; set; }

        public string SlotsAvailableString { get; set; }

        public bool SignedUp(string username)
        {//TODO: do we need this n.UsersSignedUp.HasContent() ??

            if (string.IsNullOrEmpty(username))
                return false;

            _isSignedUp = Slots.Any(n => /*n.UsersSignedUp.HasContent() &&*/ n.UsersSignedUp.Any(m => m.User.Username == username));

            return _isSignedUp;
        }


        public void GenerateSlotsAvailableString()
        {
            if (!OverrideClose && Closed)
                SlotsAvailableString = "Sign up closed";

            TotalSlotsAvailable = Slots.Select(n => n.PlacesAvailable).Count();
            TotalReserveAvailable = Slots.Select(n => n.ReservePlaces).Count();
            TotalInterestedAvaiable = Slots.Select(n => n.InterestedPlaces).Count();
            TotalNumberSignedUp = Slots.Select(n => n.NumberSignedUp()).Count();

            int placesAvailable;

            if (!MergeReserve)
            {
             //   var inMain = false;

                var totalMainRemaining = 0;
                var totalReserveRemaining = 0;
                
                foreach (var slot in Slots)
                {
                    totalMainRemaining += slot.PlacesAvailable - slot.NumberSignedUp() < 0 ? 0 : slot.PlacesAvailable - slot.NumberSignedUp();

                    if (totalMainRemaining >= slot.NumberSignedUp())
                    {
                        totalReserveRemaining += slot.ReservePlaces;
                    }
                    else
                    {
                        totalReserveRemaining += (slot.PlacesAvailable + slot.ReservePlaces) - slot.NumberSignedUp();
                    }
                }

                if (totalMainRemaining > 0)
                {
                    SlotsAvailableString = string.Format("{1} {0} Available ({2} Main, {3} Reserve)",
                        "PLACE".SingularOrPlural(TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp),
                        TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp,
                        totalMainRemaining,
                        totalReserveRemaining);
                    return;
                }

                totalReserveRemaining = Slots.Sum(n => n.ReservePlaces + n.PlacesAvailable - n.NumberSignedUp());

                SlotsAvailableString = string.Format("{1} {0} Available",
                    "RESERVE".SingularOrPlural(totalReserveRemaining - TotalNumberSignedUp),
                    totalReserveRemaining);

                SlotsAvailableString = string.Format("{1} {0} Available", "Place".SingularOrPlural(TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp), TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp);
                return;
            }
            else
            {
                if (TotalNumberSignedUp < TotalSlotsAvailable + TotalReserveAvailable)
                {
                    placesAvailable = TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp;
                    SlotsAvailableString = string.Format("{1} {0} Available", "PLACE".SingularOrPlural(placesAvailable), placesAvailable);
                    return;
                }
            }

            if (TotalNumberSignedUp < TotalSlotsAvailable + TotalReserveAvailable + TotalInterestedAvaiable)
            {
                placesAvailable = TotalSlotsAvailable + TotalReserveAvailable + TotalInterestedAvaiable - TotalNumberSignedUp;
                SlotsAvailableString = string.Format("{1} {0} Available", "INTERESTED", placesAvailable);
                return;
            }

            SlotsAvailableString = "No Places Available";
        }



    }

    public static class StringExtensions
    {
        public static string SingularOrPlural(this string _term, int _count)
        {
            return _count == 1 ? _term : _term + "S";
        }

        public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> finder)
        {
            int index = 0;
            foreach (var item in list)
            {

                if (finder(item))
                {
                    return index;
                }
                index++;
                //  index++;
            }

            return -1;
        }
    }



}
