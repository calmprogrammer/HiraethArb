using System.Numerics;

namespace HiraethArb.BusinessLogic.Classes
{
    public class Logging
    {


        /// <summary>
        /// Method that creates or appends to a text file the quotes that are genereated from the 1Inch API
        /// </summary>
        /// <param name="transaction">The from token API</param>
        /// <param name="quoteAPIToToken">The to token API</param>
        public static void CreateTextFile(QuoteAPI transaction)
        {
            using StreamWriter outputFile = new StreamWriter("HiraethArb.txt", true);

            outputFile.WriteLine($"{transaction.quote?.fromToken?.symbol} - {transaction.quote?.toToken?.symbol}" +
            $",{transaction.quote?.toTokenAmount} {transaction.quote?.toToken?.symbol},{transaction.quote?.estimatedGas}");

        }

        public static void WriteToConsole(QuoteAPI transaction)
        {

            try
            {
                Console.WriteLine("\nQuote");
                Console.WriteLine($"{transaction.quote?.fromToken?.symbol} - {transaction.quote?.toToken?.symbol}" +
                    $" Price : {transaction.quote?.toTokenAmount} {transaction.quote?.toToken?.symbol} Gas Fees {transaction.quote?.estimatedGas}");

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception" + ex.Message); 
            }
        }
    }
}

