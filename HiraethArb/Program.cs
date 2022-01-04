
using HiraethArb.BusinessLogic.Classes;


const string tokensUrl = @"https://api.1inch.io/v4.0/137/tokens";

double amount = 10000000000000000;


TokenAPI tokenAPI = new TokenAPI(tokensUrl);
tokenAPI.GetAPI();


Dictionary<string, Quote> quotes = new Dictionary<string, Quote>();

foreach (var tokenFrom in tokenAPI.tokensDictionary!)
{
    string fromTokenAddress = tokenAPI.tokensDictionary[tokenFrom.Key].address!;


    foreach (var tokenTo in tokenAPI.tokensDictionary!)
    {
        string toTokenAddress = tokenAPI.tokensDictionary[tokenTo.Key].address!;
        string quoteUrl = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={fromTokenAddress}&toTokenAddress={toTokenAddress}&amount={amount}";
        QuoteAPI quote = new QuoteAPI(quoteUrl);
        quote.GetAPI();
        Console.WriteLine($"{quote.quote?.fromToken?.symbol} - {quote.quote?.toToken?.symbol} Price : {quote.quote?.toTokenAmount} Gas Fees {quote.quote?.estimatedGas}");
    }


}

//foreach (var token in tokenAPI.tokensDictionary!)
//{
//    Console.WriteLine(tokenAPI.tokensDictionary[token.Key].name);
//}

