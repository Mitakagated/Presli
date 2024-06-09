using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presli.Classes
{
    internal static class randomYuGiOhCard
    {
        public static FileStream RandomCard()
        {
            string[] cardPaths = Directory.GetFiles("../../.././yugioh");
            var random = new Random();
            var randomCardNumber = random.Next(0, cardPaths.Length);
            var randomCard = cardPaths[randomCardNumber];
            FileStream fs = File.OpenRead(randomCard);
            return fs;
        }
    }
}
