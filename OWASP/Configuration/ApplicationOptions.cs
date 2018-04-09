using OWASP.DAL.Security;
using System.Collections.Generic;

namespace OWASP.Configuration
{
    public class ApplicationOptions : ISecurityOptions
    {
        public string Key { get; set; } = string.Empty;
        public Dictionary<string, string> ConnectionStrings { get; set; }
    }
}
