using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HiraethArb.BusinessLogic.Classes
{
    public class SearchForArb
    {
        public string? amount { get; set; }
        public List<Token>? tokenList { get; set; }

        public string fromTokenAddress { get; set; }

        private string? finalArbs; 
        public SearchForArb(string? amount, List<Token>? tokenList, string fromTokenAddress)
        {
            this.amount = amount;
            this.tokenList = tokenList;
            this.fromTokenAddress = fromTokenAddress;
        }

        public void LoadQuotes()
        {
            for (int i = 0; i < tokenList!.Count; i++)
            {

                string toTokenAddress = tokenList[i].address!;
                string quoteUrlOne = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={fromTokenAddress}&toTokenAddress={toTokenAddress}&amount={amount}";
                string quoteUrlTwo = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={toTokenAddress}&toTokenAddress={fromTokenAddress}&amount={amount}";

                var quoteAPIFromToken = new QuoteAPI(quoteUrlOne);
                QuoteAPI quoteAPIToToken = new QuoteAPI(quoteUrlTwo);
                quoteAPIFromToken.GetAPI();
                quoteAPIToToken.GetAPI();

                CreateTextFile(quoteAPIFromToken, quoteAPIToToken);
                WriteToConsole(quoteAPIFromToken, quoteAPIToToken);

                try
                {
                    var quoteAPIPriceDifference = BuildAPIURL(fromTokenAddress, toTokenAddress, quoteAPIToToken);
                    if (quoteAPIPriceDifference.quote == null)
                        continue;

                    SearchForPriceDifference(quoteAPIFromToken, quoteAPIPriceDifference); 

                    quoteAPIPriceDifference = BuildAPIURL(toTokenAddress, fromTokenAddress, quoteAPIFromToken);
                    if (quoteAPIPriceDifference.quote == null)
                        continue;

                    SearchForPriceDifference(quoteAPIToToken, quoteAPIPriceDifference);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error : " + e.Message);
                }

            }
            Console.WriteLine("final arb opps : \n" + finalArbs); 
        }

        private static QuoteAPI BuildAPIURL(string fromTokenAddress, string toTokenAddress, QuoteAPI quoteAPIToToken)
        {
            string priceDifferenceUrlFromToken = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={fromTokenAddress}&toTokenAddress={toTokenAddress}&amount={quoteAPIToToken.quote?.fromTokenAmount}";
            QuoteAPI quoteAPIPriceDifference = new QuoteAPI(priceDifferenceUrlFromToken);
            quoteAPIPriceDifference.GetAPI();
            return quoteAPIPriceDifference;
        }

        private void SearchForPriceDifference(QuoteAPI quoteAPIFromToken, QuoteAPI quoteAPIPriceDifference)
        {
            string finalArbs = ""; 
            BigInteger? estimatedGasFeeEth = (quoteAPIFromToken.quote!.estimatedGas + quoteAPIPriceDifference.quote!.estimatedGas);
            Token? eth = tokenList!.Find(x => x.symbol!.Equals("MATIC"));

            var gasFeeUrl = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={eth!.address}&toTokenAddress={quoteAPIFromToken.quote.toToken!.address}&amount={estimatedGasFeeEth}";
            QuoteAPI quoteAPIGasFeeToToken = new QuoteAPI(gasFeeUrl);

            if (!(quoteAPIPriceDifference.quote!.toTokenAmount!.Equals(quoteAPIFromToken.quote!.toTokenAmount)))
            {

                Console.WriteLine($"Arb oppurtunity : {quoteAPIPriceDifference.quote!.toTokenAmount} {quoteAPIFromToken.quote!.toTokenAmount} ");

                bool isSuccessfulConversionPriceDifferenceAmount = BigInteger.TryParse(quoteAPIPriceDifference.quote!.toTokenAmount, out BigInteger priceDiferenceAmount);
                bool isSuccessfulConversionToTokenAmount = BigInteger.TryParse(quoteAPIFromToken.quote!.toTokenAmount, out BigInteger toTokenAmount);
                bool isSucessfulConversionGasFee = BigInteger.TryParse(quoteAPIGasFeeToToken.quote?.toTokenAmount, out BigInteger gasFeeToToken);

                if (gasFeeToToken == 0)
                    gasFeeToToken = (BigInteger)(quoteAPIFromToken.quote!.estimatedGas + quoteAPIPriceDifference.quote!.estimatedGas)!;

                BigInteger difference = priceDiferenceAmount - toTokenAmount - gasFeeToToken;//negative eth-> btc   // btc-> eth 

                int isBigger = BigInteger.Compare(difference, toTokenAmount / 10);

                if (isSuccessfulConversionPriceDifferenceAmount && isSuccessfulConversionToTokenAmount && difference > 0 && isBigger >= 0)
                {
                    finalArbs += $"\nPrice Difference : {difference} {quoteAPIFromToken.quote.toToken!.symbol} Gas Fee : {gasFeeToToken} {quoteAPIFromToken.quote.toToken!.symbol}";
                    this.finalArbs = finalArbs; 
                    Console.Beep();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Price Difference : {difference} {quoteAPIFromToken.quote.toToken!.symbol} Gas Fee : {gasFeeToToken} {quoteAPIFromToken.quote.toToken!.symbol}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        
        }

        private static void WriteToConsole(QuoteAPI quoteAPIFromToken, QuoteAPI quoteAPIToToken)
        {
            bool isSuccessfulConversionFromToken = BigInteger.TryParse(quoteAPIFromToken.quote?.toTokenAmount, out BigInteger fromTokenAmount);
            bool isSuccessfulConversionToToken = BigInteger.TryParse(quoteAPIToToken.quote?.toTokenAmount, out BigInteger toTokenAmount);

            if (!isSuccessfulConversionFromToken && !isSuccessfulConversionToToken) return;

            Console.WriteLine("\nQuote");
            Console.WriteLine($"{quoteAPIFromToken.quote?.fromToken?.symbol} - {quoteAPIFromToken.quote?.toToken?.symbol}" +
                $" Price : {fromTokenAmount} {quoteAPIFromToken.quote?.fromToken?.symbol} Gas Fees {quoteAPIFromToken.quote?.estimatedGas}");
            Console.WriteLine($"{quoteAPIToToken.quote?.fromToken?.symbol} - {quoteAPIToToken.quote?.toToken?.symbol}" +
               $" Price : {toTokenAmount} {quoteAPIToToken.quote?.fromToken?.symbol} Gas Fees {quoteAPIToToken.quote?.estimatedGas}");
        }


        /// <summary>
        /// Method that creates or appends to a text file the quotes that are genereated from the 1Inch API
        /// </summary>
        /// <param name="quoteAPIFromToken">The from token API</param>
        /// <param name="quoteAPIToToken">The to token API</param>
        private static void CreateTextFile(QuoteAPI quoteAPIFromToken, QuoteAPI quoteAPIToToken)
        {
            using StreamWriter outputFile = new StreamWriter("HiraethArb.txt", true);

            outputFile.WriteLine($"{quoteAPIFromToken.quote?.fromToken?.symbol} - {quoteAPIFromToken.quote?.toToken?.symbol}" +
            $",{quoteAPIFromToken.quote?.toTokenAmount} {quoteAPIFromToken.quote?.toToken?.symbol},{quoteAPIFromToken.quote?.estimatedGas}");

            outputFile.WriteLine($"{quoteAPIToToken.quote?.fromToken?.symbol} - {quoteAPIToToken.quote?.toToken?.symbol}" +
            $",{quoteAPIToToken.quote?.toTokenAmount} {quoteAPIToToken.quote?.toToken?.symbol},{quoteAPIToToken.quote?.estimatedGas}");
        }
    }
}


//What mphaso wants in the docs ----------->>>>>>>>>>>
//

//add tether quote for to token
