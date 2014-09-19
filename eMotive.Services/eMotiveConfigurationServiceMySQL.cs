using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using eMotive.Services.Interfaces;
using eMotive.Services.Objects.Settings;
using MySql.Data.MySqlClient;


namespace eMotive.Services
{
    public class eMotiveConfigurationServiceMySQL : IeMotiveConfigurationService
    {
        private readonly string _connectionString;
        private IDbConnection _connection;
        private Settings _settings;


        public eMotiveConfigurationServiceMySQL(string connectionString)
        {
            _connectionString = connectionString;

            if(!GetSettings())
                throw new Exception("Could not fetch site settings.");
        }

        
        private bool GetSettings()
        {
            using (var cn = Connection)
            {

                var results = cn.Query<Settings>("SELECT * FROM `settings`;").SingleOrDefault();

                if (results == null)
                    return false;
                
                _settings = results;

                return true;
            }
        }

        internal IDbConnection Connection
        {
            get
            {
                return _connection ?? new MySqlConnection(_connectionString);
            }
        }

        internal Settings Settings()
        {
            if (_settings != null)
                return _settings;

            if (GetSettings())
                return _settings;

            throw new Exception("Could not fetch site settings.");
        }

        public string EmailFromAddress()
        {
            return Settings().MailFromAddress;
        }

        public bool EmailsEnabled()
        {
            return !Settings().DisableEmails;
        }

        public int MaxLoginAttempts()
        {
            return Settings().MaxLoginAttempts;
        }

        public int LockoutTimeInMinutes()
        {
            return Settings().LockoutTimeMinutes;
        }

        public string SiteName()
        {
            return Settings().SiteName;
        }

        public string SiteURL()
        {
            return Settings().SiteURL;
        }

        public string GoogleAnalytics()
        {
            return Settings().GoogleAnalytics;
        }

        public string MetaTags()
        {
            return Settings().MetaTags;
        }

        public bool SaveSettings(Settings settings)
        {
            using (var cn = Connection)
            {

                var success = cn.Execute("INSERT INTO `settings` VALUES @settings;", settings) > 0;

                if (success)
                {//TODO: need to lock this?
                    _settings = settings;
                }

                return success;
            }
        }

        public string GetClientIpAddress()
        {
            var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }
    }
}
