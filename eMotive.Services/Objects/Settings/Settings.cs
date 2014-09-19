namespace eMotive.Services.Objects.Settings
{
    public class Settings
    {
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public bool DisableEmails { get; set; }
        public int MaxLoginAttempts { get; set; }
        public int LockoutTimeMinutes { get; set; }
        public string MailFromAddress { get; set; }
        public string GoogleAnalytics { get; set; }
        public string MetaTags { get; set; }
    }

}
