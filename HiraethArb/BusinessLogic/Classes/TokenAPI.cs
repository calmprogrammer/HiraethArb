using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HiraethArb.BusinessLogic.Classes
{
    public class TokenAPI : API
    {
        public TokenAPI(string url) : base(url)
        {
            
        }
        public Dictionary<string,Token>? tokensDictionary { get; set; }
        protected override void Deserialize()
        {
            var tokens = JsonSerializer.Deserialize<Dictionary<string, Token>>(jsonObject!["tokens"]);
            this.tokensDictionary = tokens; 
        }
        
    }
}
