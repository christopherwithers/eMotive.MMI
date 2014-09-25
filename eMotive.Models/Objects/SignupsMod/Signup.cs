using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;

namespace eMotive.Models.Objects.SignupsMod
{
    public class Signup
    {
        private bool? _isSignedUp;

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
            /*if (_isSignedUp.HasValue)
                return _isSignedUp.Value;

            if (string.IsNullOrEmpty(username))
            {
                _isSignedUp = false;
                return false;
            }*/
            _isSignedUp = Slots.Any(n => /*n.UsersSignedUp.HasContent() &&*/ n.SignedUp(username));

            //   _isSignedUp = Slots.Any(n => n.SignedUp(username));

            return _isSignedUp.Value;
        }


        public void GenerateSlotsAvailableString()
        {
            if (!OverrideClose && Closed)
                SlotsAvailableString = "Sign up closed";

            TotalSlotsAvailable = Slots.Sum(n => n.PlacesAvailable);
            TotalReserveAvailable = Slots.Sum(n => n.ReservePlaces);
            TotalInterestedAvaiable = Slots.Sum(n => n.InterestedPlaces);
            TotalNumberSignedUp = Slots.Sum(n => n.NumberSignedUp());

            int placesAvailable;
            if (!MergeReserve)
            {
                if (TotalNumberSignedUp >= TotalSlotsAvailable)
                {//if there are more users signed up than there are main spaces available
                    if (TotalNumberSignedUp >= TotalSlotsAvailable + TotalReserveAvailable)
                    {//if more people signed up than main and reserve combined, no places available
                        SlotsAvailableString = "No Places Available";
                        return;
                    }

                    //THere are only reserve places available, display how many
                    var value = (TotalSlotsAvailable + TotalReserveAvailable) - TotalNumberSignedUp;
                    SlotsAvailableString = string.Format("{1} {0} Available", "RESERVE".SingularOrPlural(value), value);
                    return;
                }


                //There are main spots available, show user how many main and reserve are remaining
                SlotsAvailableString = string.Format("{1} {0} Available ({2} Main, {3} Reserve)",
                                        "PLACE".SingularOrPlural(TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp),
                                        TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp,
                                        TotalSlotsAvailable - TotalNumberSignedUp,
                                        TotalReserveAvailable);
                return;
            }

            if (TotalNumberSignedUp < TotalSlotsAvailable + TotalReserveAvailable)
            {
                placesAvailable = TotalSlotsAvailable + TotalReserveAvailable - TotalNumberSignedUp;
                SlotsAvailableString = string.Format("{1} {0} Available", "PLACE".SingularOrPlural(placesAvailable), placesAvailable);
                return;
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
