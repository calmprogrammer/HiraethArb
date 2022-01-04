using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiraethArb.BusinessLogic.Classes
{
    public class Quote
    {
        public Token? fromToken { get; set; }
        public Token? toToken { get; set;}
        public string? toTokenAmount { get; set; }
        public string? fromTokenAmount { get; set; }
        public List<Object>? protocols { get; set; } 
        public int? estimatedGas { get; set; }
    }
}
