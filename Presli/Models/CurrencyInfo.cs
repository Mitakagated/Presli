using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presli.Models;
public class CurrencyInfo
{
    public ulong DiscordId { get; set; }
    public int Mito { get; set; } = 500;
    public int BettingCurrency { get; set; } = 500;
}
