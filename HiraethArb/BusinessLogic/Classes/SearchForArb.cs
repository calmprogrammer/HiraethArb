using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using HiraethArb.BusinessLogic.Classes;

namespace HiraethArb.BusinessLogic.Classes
{
    /// <summary>
    /// A class that allows for the searching of arbitrage oppurtunities
    /// </summary>
    public class SearchForArb
    {
        //the from token amount which is passed to the object
        public string? amount { get; set; } 

         // the list of tokens from the specific network we are interacting with 
        public List<Token>? tokenList { get; set; }

        // the from token address that we are currently using for comparisons
        public string fromTokenAddress { get; set; } 

        //constructor to initialise the searchforarb object
        public SearchForArb(string? amount, List<Token>? tokenList, string fromTokenAddress) 
        {
            //assinging arguments to class properties
            this.amount = amount;
            this.tokenList = tokenList;
            this.fromTokenAddress = fromTokenAddress;
        }

        /// <summary>
        /// Method responsoble for loading all the quotes of the tokens
        /// </summary>
        public void LoadQuotes()
        {
            //loop for the list of tokens
            for (int i = 0; i < tokenList!.Count; i++) 
            {

                // address of the toToken we are looking at currently
                string toTokenAddress = tokenList[i].address!;

                //Initial transaction quote returned. E.g. 1 eth -->> 0.05 btc 
                QuoteAPI transaction = QuoteAPI.BuildAPIURL(fromTokenAddress, toTokenAddress, amount!); 
                transaction.GetAPI();

                //continue to next iteration if there is no transaction quote
                if (transaction.quote == null)
                    continue;

                //error handling in the case of an unseen error occurs
                try
                {
                    //inverse transaction quote returned. E.g. 0.05 btc -->> 1.002 eth 
                    QuoteAPI inverseTransaction = QuoteAPI.BuildAPIURL(toTokenAddress, fromTokenAddress, transaction.quote!.toTokenAmount!);
                    if (inverseTransaction.quote == null)
                        continue;
                    
                    //calling method responsible for verifying if there is a positive price difference between the initial transaction amount and the inverse transaction amount
                    SearchForPriceDifference(transaction, inverseTransaction);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error : " + e.Message);
                }

            }

        }


        private void SearchForPriceDifference(QuoteAPI transaction, QuoteAPI inverseTransaction)
        {
            //conversion of amount and inverseAmount to BigInteger types to interact with as a number rather than a string
            bool isInverseTransactionAmountConversionSuccessful = BigInteger.TryParse(inverseTransaction.quote!.toTokenAmount, out BigInteger inverseAmount);
            bool isTransactionAmountConversionSuccessful = BigInteger.TryParse(this.amount, out BigInteger amount);
           
            //BigInteger type to store the difference between the amount and inverseAmount
            BigInteger difference = 0;

            //checking if the converions were successful and if successful calculating the diference between the inverse amount and the intial amount. E.g. 1.02 eth - 1 eth ---> 0.02 eth
            if (isInverseTransactionAmountConversionSuccessful && isTransactionAmountConversionSuccessful)
                difference = inverseAmount - amount;

            //if the difference is positive log the results as there is arbitrage. 
            if (difference > 0)
            {
                Logging.CreateTextFile(transaction);
                Logging.WriteToConsole(transaction);
                Console.WriteLine($"Arb oppurtunity :  {inverseAmount} - {this.amount} : {difference}");
                Console.WriteLine("Quote api from token price: " + inverseTransaction.quote?.fromToken?.symbol + "-" + inverseTransaction.quote?.toToken?.symbol + " : " + inverseTransaction.quote?.toTokenAmount + " " + inverseTransaction.quote?.toToken?.symbol);
                Console.Beep();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Price Difference : {difference} {transaction.quote?.fromToken?.symbol} Gas Fee : {transaction.quote?.estimatedGas} {transaction.quote?.fromToken?.symbol}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            return;

        }

    }
}
