
using System.Numerics;
using HiraethArb.BusinessLogic.Classes;


const string tokensUrl = @"https://api.1inch.io/v4.0/137/tokens"; // token api from 1 inch

string amount = "100000000000000000000000000";


TokenAPI tokenAPI = new TokenAPI(tokensUrl);
tokenAPI.GetAPI();
var tokenList = tokenAPI.tokensDictionary!.Values.ToList<Token>();



for (int i = 0; i < tokenList.Count; i++)
{
    SearchForArb arb = new SearchForArb(amount, tokenList, tokenList[i].address!); 
    arb.LoadQuotes();
}





Console.WriteLine("Complete");

