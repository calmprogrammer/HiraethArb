using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json; 
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HiraethArb.BusinessLogic.Classes
{
    public class Token
    {
    
        public string? symbol { get; set; }
        public string? name { get; set; }    
        public int? decimals { get; set; }
        public string? address { get; set; }
        public string? logoURI { get; set; }

    }

}
