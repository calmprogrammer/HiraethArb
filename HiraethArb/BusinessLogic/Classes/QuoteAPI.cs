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

        private static readonly string tetherAddress = "0xc2132d05d31c914a87c6611c10748aeb04b58e8f";
        public QuoteAPI(string url) : base(url)
        {

        }

        protected override void Deserialize()
        {
            var quote = JsonSerializer.Deserialize<Quote>(jsonObject!);
            this.quote = quote;
        }
        public static QuoteAPI BuildAPIURL(string fromTokenAddress, string toTokenAddress, string amount)
        {
            string quoteUrl = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={fromTokenAddress}&toTokenAddress={toTokenAddress}&amount={amount}";
            QuoteAPI transaction = new QuoteAPI(quoteUrl);
            transaction.GetAPI();
            return transaction;
        }
        public static void ConvertToTether(string fromTokenAddress, string amount)
        {
            var tetherPrice = BuildAPIURL(fromTokenAddress, tetherAddress, amount);
            Console.WriteLine("Tether Price : " + tetherPrice.quote?.toTokenAmount?.ToString());
        }
        public static void AddDecimals(string amount, int? numberOfDecimals)
        {
            Console.WriteLine("Amount with decimals start: " + amount);
            if (amount.Length - 1 < numberOfDecimals)
            {
                int? zerosToAdd = numberOfDecimals - amount.Length - 1;
                string prefixOfZeros = ""; 
                for (int i = 0; i < zerosToAdd; i++)
                {
                    prefixOfZeros += "0"; 
                }
                amount = amount.Insert(0, prefixOfZeros);
                
            }
            for (int i = amount.Length; i > 0; i--)
            {
                if(i == numberOfDecimals)
                {
                   // string tempAmount = amount; 
                    string startSubArray = amount.Substring(i,amount.Length - 1);//[123] --> [32,1]
                     
                    startSubArray += ",";
                    amount = startSubArray;  

                }
            }
            Console.WriteLine("Amount with decimals end: " + amount);
        }
    }
}
