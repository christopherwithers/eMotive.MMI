using System;

namespace eMotive.Models.Objects.Signups
{
    public class SignupState
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        //public int SlotsAvailable { get; set; }
        public bool SignedUp { get; set; }
        public int NumberSignedUp { get; set; }
        public int TotalSlotsAvailable { get; set; }
        public int TotalReserveAvailable { get; set; }
        public int TotalInterestedAvaiable { get; set; }
        public SlotType TypeAvailable { get; set; }
        public bool DisabilitySignup { get; set; }
        public bool Closed { get; set; }

        public Group Group { get; set; }

        public string Description { get; set; }

        public bool OverrideClose { get; set; }
        public bool MergeReserve { get; set; }

        public string SlotsAvailableString()
        {
            if (!OverrideClose && Closed)
                return "Sign up closed";

            int placesAvailable;

            if (!MergeReserve)
            {

                if (NumberSignedUp < TotalSlotsAvailable)
                    return string.Format("{1} {0} Available", "Place".SingularOrPlural(TotalSlotsAvailable - NumberSignedUp), TotalSlotsAvailable - NumberSignedUp);

                if (NumberSignedUp < TotalSlotsAvailable + TotalReserveAvailable)
                    return string.Format("{1} {0} Available", "Reserve".SingularOrPlural(TotalSlotsAvailable - NumberSignedUp - TotalReserveAvailable), TotalSlotsAvailable + TotalReserveAvailable - NumberSignedUp);

            }
            else
            {
                if (NumberSignedUp < TotalSlotsAvailable + TotalReserveAvailable)
                {
                    placesAvailable = TotalSlotsAvailable + TotalReserveAvailable - NumberSignedUp;
                    return string.Format("{1} {0} Available", "Place".SingularOrPlural(placesAvailable), placesAvailable);
                }
            }

            if (NumberSignedUp < TotalSlotsAvailable + TotalReserveAvailable + TotalInterestedAvaiable)
            {
                placesAvailable = TotalSlotsAvailable + TotalReserveAvailable + TotalInterestedAvaiable - NumberSignedUp;
                return string.Format("{1} {0} Available", "Interested", placesAvailable);
            }

            return "No Places Available";
        }
    }

    public static class StringExtensions
    {
        public static string SingularOrPlural(this string _term, int _count)
        {
            return _count == 1 ? _term : _term + "s";
        }
    }
}
