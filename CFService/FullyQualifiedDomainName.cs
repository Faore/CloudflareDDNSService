using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Faore.CFService
{
    public class FullyQualifiedDomainName
    {
        private const string Regex_FQDN = @"^(?:([\w\_]+)\.)*([\w]+)\.(\w+)\.?$";

        private List<string> Hierarchy;

        public FullyQualifiedDomainName(string domain)
        {
            Regex regex_FQDN = new Regex(Regex_FQDN);
            if (!regex_FQDN.IsMatch(domain))
            {
                throw new ArgumentException("Bad fully or partially qualified domain name.");
            }
            Match match = regex_FQDN.Match(domain);
            //Add the DNS Root
            Hierarchy.Add(string.Empty);
            //Add the TLD
            Hierarchy.Add(match.Groups[3].Captures[0].Value);
            //Add the domain name.
            Hierarchy.Add(match.Groups[2].Captures[0].Value);
            //Add each subdomain name.
            for (var x = match.Groups[1].Captures.Count - 1; x >= 0; x--)
            {
                Hierarchy.Add(match.Groups[1].Captures[x].Value);
            }
        }

        public string ZoneName { get {return $"{Hierarchy[2]}.{Hierarchy[1]}";}}
        public string PartiallyQualifiedDomainName {
            get {
                var output = "";
                for(var i = Hierarchy.Count - 1;  i > 0; i--)
                {
                    if (i < Hierarchy.Count - 1)
                    {
                        output += Hierarchy[i];
                    } else
                    {
                        output += $".{Hierarchy[i]}";
                    }
                }
                return output;
            }
        }
    }
}
