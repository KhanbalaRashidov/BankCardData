using System.Dynamic;

namespace BankCardData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bank = new BankCardReader();
            var data = bank.CardInfo;

            Console.WriteLine($"Card Number:{data.CardNumber}  Expiry Date:{data.ExpriyDate}  Card type:{data.CardType}");
        }
    }
}
