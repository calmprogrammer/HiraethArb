
using System.Numerics;
using HiraethArb.BusinessLogic.Classes;


const string tokensUrl = @"https://api.1inch.io/v4.0/137/tokens"; // token api from 1 inch

double amount = 10000000000000000;


TokenAPI tokenAPI = new TokenAPI(tokensUrl);
tokenAPI.GetAPI();


uint rowCount = 0;

while (rowCount < 5)
    LoadQuotes(tokenAPI.tokensDictionary!.Values.ToList<Token>());
Console.WriteLine("Complete");

void LoadQuotes(List<Token> tokenList)
{
    int count = 1;
    for (int i = 0; i < tokenList.Count; i++)
    {
        string fromTokenAddress = tokenList[i].address!;

        string toTokenAddress = tokenList[count].address!;
        string quoteUrlOne = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={fromTokenAddress}&toTokenAddress={toTokenAddress}&amount={amount}";
        string quoteUrlTwo = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={toTokenAddress}&toTokenAddress={fromTokenAddress}&amount={amount}";

        QuoteAPI quoteAPIOne = new QuoteAPI(quoteUrlOne);// eth/btc 10 btc


        QuoteAPI quoteAPITwo = new QuoteAPI(quoteUrlTwo);


        quoteAPIOne.GetAPI();
        quoteAPITwo.GetAPI();
        using (StreamWriter outputFile = new StreamWriter("HiraethArb.txt", true))
        {

            outputFile.WriteLine($"{quoteAPIOne.quote?.fromToken?.symbol} - {quoteAPIOne.quote?.toToken?.symbol}" +
            $",{quoteAPIOne.quote?.toTokenAmount} {quoteAPIOne.quote?.toToken?.symbol},{quoteAPIOne.quote?.estimatedGas}");

            outputFile.WriteLine($"{quoteAPITwo.quote?.fromToken?.symbol} - {quoteAPITwo.quote?.toToken?.symbol}" +
            $",{quoteAPITwo.quote?.toTokenAmount} {quoteAPITwo.quote?.toToken?.symbol},{quoteAPITwo.quote?.estimatedGas}");

        }

        // excelObj.InsertCellInWorksheet($"{quote.quote?.toToken?.name}", rowCount, excelObj.worksheetPart!);
        Console.WriteLine("Quote"); 
        Console.WriteLine($"{quoteAPIOne.quote?.fromToken?.symbol} - {quoteAPIOne.quote?.toToken?.symbol}" +
            $" Price : {quoteAPIOne.quote?.toTokenAmount} {quoteAPIOne.quote?.toToken?.symbol} Gas Fees {quoteAPIOne.quote?.estimatedGas}");
        Console.WriteLine($"{quoteAPITwo.quote?.fromToken?.symbol} - {quoteAPITwo.quote?.toToken?.symbol}" +
           $" Price : {quoteAPITwo.quote?.toTokenAmount} {quoteAPITwo.quote?.toToken?.symbol} Gas Fees {quoteAPITwo.quote?.estimatedGas}");

        try
        {
            quoteUrlOne = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={fromTokenAddress}&toTokenAddress={toTokenAddress}&amount={quoteAPITwo.quote?.fromTokenAmount}"; //
            QuoteAPI quoteAPIArb = new QuoteAPI(quoteUrlOne);
            quoteAPIArb.GetAPI();//the inverse of quoteAPIOne 

            if(quoteAPIArb.quote == null)
            {
                continue; 
            }
          
            if (!(quoteAPIArb.quote!.toTokenAmount!.Equals(quoteAPIOne.quote!.toTokenAmount)))
            {
                Console.WriteLine($"Arb oppurtunity : {quoteAPIArb.quote!.toTokenAmount} {quoteAPIOne.quote!.toTokenAmount} ");
              //  BigInteger.TryParse(quoteAPIArb.quote!.toTokenAmount, out ulong arbAmount);
               // ulong.TryParse(quoteAPITwo.quote!.toTokenAmount, out ulong toTokenAmount);
              //  ulong arbOppurtunity = arbAmount - toTokenAmount; 
              //  Console.WriteLine($"Arb amount : {arbOppurtunity}"); 
            }

            quoteUrlTwo = @$"https://api.1inch.io/v4.0/137/quote?fromTokenAddress={toTokenAddress}&toTokenAddress={fromTokenAddress}&amount={quoteAPIOne.quote?.fromTokenAmount}";
            quoteAPIArb = new QuoteAPI(quoteUrlTwo);
            quoteAPIArb.GetAPI();


            if (quoteAPIArb.quote == null)
            {
                continue;
            }

            if (!(quoteAPIArb.quote!.toTokenAmount!.Equals(quoteAPITwo.quote!.toTokenAmount)))
            {
                Console.WriteLine($"Arb oppurtunity : {quoteAPIArb.quote!.toTokenAmount} {quoteAPITwo.quote!.toTokenAmount} ");
                ulong.TryParse(quoteAPIArb.quote!.toTokenAmount, out ulong arbAmount);
                ulong.TryParse(quoteAPITwo.quote!.toTokenAmount, out ulong toTokenAmount);
                ulong arbOppurtunity = arbAmount - toTokenAmount;
                Console.WriteLine($"Arb amount : {arbOppurtunity}");
                Console.WriteLine($"Arb amount : {(arbAmount - toTokenAmount)}");
            }
        }
        catch (Exception e)
        {

            Console.WriteLine("Error : " + e); 
        }

        rowCount++;
        Console.WriteLine(rowCount);

    }
}

void LoadQuotesFast(List<Token> tokenList)
{
    for (int i = 0; i < tokenList.Count; i++)
    {

    }
}
