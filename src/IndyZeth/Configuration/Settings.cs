using System;
using System.Collections.Generic;
using System.Text;

namespace Elbanique.IndyZeth.Configuration
{
    public class Settings
    {
        public CompanyMapping[] CompanyMappings { get; set; }
    }

    public class CompanyMapping
    {
        public string CompanyName { get; set; }
        public string NamespaceFirstPart { get; set; }
    }
}
