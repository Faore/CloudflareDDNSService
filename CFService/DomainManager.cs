using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudFlare.NET;

namespace Faore.CFService
{
    public class DomainManager
    {
        private Configuration Configuration;
        private CloudFlareClient CloudflareClient;
        private EventLog eventLog;
        public DomainManager(Configuration configuration, EventLog eventLog)
        {
            Configuration = configuration;
            CloudflareClient = new CloudFlareClient(new CloudFlareAuth(Configuration.Email, Configuration.APIKey));
        }

        public void BuildIdentifiers()
        {
            var zoneTask = CloudflareClient.GetAllZonesAsync();
            zoneTask.RunSynchronously();
            zoneTask.Wait();
            var presentZones = zoneTask.Result;
            Dictionary<string, string> zoneIdentifiers = new Dictionary<string, string>();
            HashSet<string> neededZoneIdentifiers = new HashSet<string>();
            foreach(var zone in Configuration.Domains)
            {
                neededZoneIdentifiers.Add(zone.ZoneName);
            }
            foreach(var zone in presentZones)
            {
                if(neededZoneIdentifiers.Contains(zone.Name))
                {
                    zoneIdentifiers.Add(zone.Name, zone.Id);
                    neededZoneIdentifiers.Remove(zone.Name);
                }
            }
            if(neededZoneIdentifiers.Count > 0)
            {
                eventLog.WriteEntry($"The following DNS zones could not be found on CloudFlare. Any records under these zones will not be updated:\n{neededZoneIdentifiers.ToString()}", EventLogEntryType.Warning);
            }

            HashSet<FullyQualifiedDomainName> neededDomains = new HashSet<FullyQualifiedDomainName>(Configuration.Domains);

            foreach(var zone in zoneIdentifiers)
            {
                var domainListTask = CloudflareClient.GetAllDnsRecordsAsync(zone.Value);

            }
        }
    }
}
