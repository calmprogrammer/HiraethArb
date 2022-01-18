
using System.Numerics;
using HiraethArb.BusinessLogic.Classes;

//1Inch url for tokens
const string tokensUrl = @"https://api.1inch.io/v4.0/137/tokens";

//amount in fromToken e.g 1BTC
string amount = "1000000000000000000";

//creating token api object
TokenAPI tokenAPI = new TokenAPI(tokensUrl);

//interaction with token api in order to recevie a json object of the tokens from 1inch
tokenAPI.GetAPI();

//convert dictionary of tokens returned from 1inch api into a list of tokens for easiser interaction with the tokens
var tokenList = tokenAPI.tokensDictionary!.Values.ToList<Token>();

// this is a for loop mphaso, hahahahaha im so funny lolz, but in seriousness this loops for the amount of tokens so that we can compare each and every token to each other
for (int i = 0; i < tokenList.Count; i++)
{
    // creating an object of the searchArb class which essesntially searches for arbitrage oppurtuinities it takes 3 params the, the from token amount, the list of tokens, and the from token address
    SearchForArb arb = new SearchForArb(amount, tokenList, tokenList[i].address!);

    // Load quotes is a method in the search arb class which basically runs the whole operation
    arb.LoadQuotes();
}


Console.WriteLine("Complete");

