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
        public long? amount { get; set; }
        public List<Token>? tokenList { get; set; }

        public string fromTokenAddress { get; set; }

        public SearchForArb(long? amount, List<Token>? tokenList, string fromTokenAddress)
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

                QuoteAPI quoteAPIFromToken = new QuoteAPI(quoteUrlOne);
                QuoteAPI quoteAPIToToken = new QuoteAPI(quoteUrlTwo);
                quoteAPIFromToken.GetAPI();
                quoteAPIToToken.GetAPI();

                CreateTextFile(quoteAPIFromToken, quoteAPIToToken);
                WriteToConsole(quoteAPIFromToken, quoteAPIToToken);

                try
                {
                    string priceDifferenceUrlFromToken = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={fromTokenAddress}&toTokenAddress={toTokenAddress}&amount={quoteAPIToToken.quote?.fromTokenAmount}";
                    QuoteAPI quoteAPIPriceDifference = new QuoteAPI(priceDifferenceUrlFromToken);
                    quoteAPIPriceDifference.GetAPI();

                    if (quoteAPIPriceDifference.quote == null)
                        continue;

                    SearchForPriceDifference(quoteAPIFromToken, quoteAPIPriceDifference);

                    string priceDifferenceUrlToToken = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={toTokenAddress}&toTokenAddress={fromTokenAddress}&amount={quoteAPIFromToken.quote?.fromTokenAmount}";
                    quoteAPIPriceDifference = new QuoteAPI(priceDifferenceUrlToToken);
                    quoteAPIPriceDifference.GetAPI();

                    if (quoteAPIPriceDifference.quote == null)
                        continue;

                    SearchForPriceDifference(quoteAPIToToken, quoteAPIPriceDifference);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error : " + e.Message);
                }

            }
        }

        private static void SearchForPriceDifference(QuoteAPI quoteAPIFromToken, QuoteAPI quoteAPIPriceDifference)
        {
            if (!(quoteAPIPriceDifference.quote!.toTokenAmount!.Equals(quoteAPIFromToken.quote!.toTokenAmount)))
            {
                Console.WriteLine($"Arb oppurtunity : {quoteAPIPriceDifference.quote!.toTokenAmount} {quoteAPIFromToken.quote!.toTokenAmount} ");

                bool isSuccessfulConversionPriceDifferenceAmount = BigInteger.TryParse(quoteAPIPriceDifference.quote!.toTokenAmount, out BigInteger priceDiferenceAmount);
                bool isSuccessfulConversionToTokenAmount = BigInteger.TryParse(quoteAPIFromToken.quote!.toTokenAmount, out BigInteger toTokenAmount);

                BigInteger difference = priceDiferenceAmount - toTokenAmount;
                if (isSuccessfulConversionPriceDifferenceAmount && isSuccessfulConversionToTokenAmount)
                    Console.WriteLine($"Price Difference : {BigInteger.Abs(difference)} {quoteAPIFromToken.quote.toToken!.symbol}");

            }
        }

        private static void WriteToConsole(QuoteAPI quoteAPIFromToken, QuoteAPI quoteAPIToToken)
        {
            Console.WriteLine("\nQuote");
            Console.WriteLine($"{quoteAPIFromToken.quote?.fromToken?.symbol} - {quoteAPIFromToken.quote?.toToken?.symbol}" +
                $" Price : {quoteAPIFromToken.quote?.toTokenAmount} {quoteAPIFromToken.quote?.toToken?.symbol} Gas Fees {quoteAPIFromToken.quote?.estimatedGas}");
            Console.WriteLine($"{quoteAPIToToken.quote?.fromToken?.symbol} - {quoteAPIToToken.quote?.toToken?.symbol}" +
               $" Price : {quoteAPIToToken.quote?.toTokenAmount} {quoteAPIToToken.quote?.toToken?.symbol} Gas Fees {quoteAPIToToken.quote?.estimatedGas}");
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
