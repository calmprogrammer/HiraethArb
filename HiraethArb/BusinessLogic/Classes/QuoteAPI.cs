using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HiraethArb.BusinessLogic.Classes
{
    public class QuoteAPI : API
    {
        public string? fromTokenAddress { get; set; }
        public string? toTokenAddress { get; set; }
        public double? amount { get; set; }
        public Quote? quote { get; set; }    
        public QuoteAPI(string url) : base(url)
        {
            
        }

        protected override void Deserialize()
        {
            var quote = JsonSerializer.Deserialize<Quote>(jsonObject!) ;
            this.quote = quote ;    
        }
    }
}
