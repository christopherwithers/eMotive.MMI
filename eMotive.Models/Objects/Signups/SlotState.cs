namespace eMotive.Models.Objects.Signups
{
    public class SlotState
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
        public int TotalPlacesAvailable { get; set; }
        public int TotalReserveAvailable { get; set; }
        public int TotalInterestedAvaiable { get; set; }
        public int NumberSignedUp { get; set; }
        public bool Enabled { get; set; }

        public bool OverrideClose { get; set; }
        public bool MergeReserve { get; set; }
        public bool Closed { get; set; }

        public SlotStatus Status { get; set; }

        public int PlacesAvailable()
        {
            return TotalPlacesAvailable - NumberSignedUp;
        }

        public string PlacesAvailableString()
        {
            if (!OverrideClose && Closed)
                return "Sign up closed";

            int placesAvailable;

            if (!MergeReserve)
            {

                if (NumberSignedUp < TotalPlacesAvailable)
                    return string.Format("{1} {0} Available", "Place".SingularOrPlural(TotalPlacesAvailable - NumberSignedUp), TotalPlacesAvailable - NumberSignedUp);

                if (NumberSignedUp < TotalPlacesAvailable + TotalReserveAvailable)
                    return string.Format("{1} {0} Available", "Reserve".SingularOrPlural(TotalPlacesAvailable - NumberSignedUp - TotalReserveAvailable), TotalPlacesAvailable + TotalReserveAvailable - NumberSignedUp);

            }
            else
            {
                if (NumberSignedUp < TotalPlacesAvailable + TotalReserveAvailable)
                {
                    placesAvailable = TotalPlacesAvailable + TotalReserveAvailable - NumberSignedUp;
                    return string.Format("{1} {0} Available", "Place".SingularOrPlural(placesAvailable), placesAvailable);
                }
            }

            if (NumberSignedUp < TotalPlacesAvailable + TotalReserveAvailable + TotalInterestedAvaiable)
            {
                placesAvailable = TotalPlacesAvailable + TotalReserveAvailable + TotalInterestedAvaiable - NumberSignedUp;
                return string.Format("{1} {0} Available", "Interested", placesAvailable);
            }

            return "No Places Available";
        }

        public bool IsSignedUpToSlot()
        {
            return Status == SlotStatus.AlreadySignedUp;
        }
    }
}
