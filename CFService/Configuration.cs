using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Faore.CFService
{
    public class Configuration
    {

        private const string Regex_APIKey = @"^([a-z0-9]){37}$";
        private const string Regex_Email = @"^([\w\_\.\-]+)@(?:([\w\-]+)\.)*([\w\-]+\.\w+)$";

        public string APIKey { get; private set; }
        public string Email { get; private set; }
        public FullyQualifiedDomainName[] Domains { get; private set; }

        public Configuration(RegistryKey rootKey)
        {
            Regex regex_APIKey = new Regex(Regex_APIKey);
            Regex regex_Email = new Regex(Regex_Email);

            if(rootKey.GetValue("cf_api_key") == null || rootKey.GetValue("cf_account_email") == null || rootKey.GetValue("domains") == null)
            {
                throw new NotConfiguredException("Failed to load configuration from registry because there are missing required values.");
            }

            //Verify each value kind.
            if(rootKey.GetValueKind("cf_api_key") != RegistryValueKind.String)
            {
                throw new BadConfigurationException($"Invalid registry key type.");
            }

            if (rootKey.GetValueKind("cf_account_email") != RegistryValueKind.String)
            {
                throw new BadConfigurationException($"Invalid registry key type.");
            }

            if (rootKey.GetValueKind("domains") != RegistryValueKind.MultiString)
            {
                throw new BadConfigurationException($"Invalid registry key type.");
            }

            //Validate API key format.
            APIKey = (string)rootKey.GetValue("cf_api_key");
            if(!regex_APIKey.IsMatch(APIKey))
            {
                throw new BadConfigurationException("API key format is incorrect.");
            }
            Email = (string)rootKey.GetValue("cf_account_email");
            if (!regex_Email.IsMatch(Email))
            {
                throw new BadConfigurationException("Account email format is incorrect.");
            }
            var domains = (string[]) rootKey.GetValue("domains");
            Domains = new FullyQualifiedDomainName[domains.Length];
            for(int i = 0; i < domains.Length; i++)
            {
                Domains[i] = new FullyQualifiedDomainName(domains[i]);
            }
        }
    }
}
