namespace eMotive.Models.Objects.SignupsMod
{
    public class UserSlotView
    {
        public string LoggedInUser { get; set; }

        public string HeaderText { get; set; }
        public string FooterText { get; set; }

        public Signup Signup { get; set; }

        public void Initialise()
        {
            foreach (var slot in Signup.Slots ?? new Slot[] { })
            {
                slot.GeneratePlacesAvailableString();
            }
        }
    }
}
