using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presli.Models;
public class CurrencyInfo
{
    [Key]
    [Required]
    public ulong DiscordId { get; set; }
    [DefaultValue(500)]
    public long Mito { get; set; } = 500;
    [DefaultValue(500)]
    public long BettingCurrency { get; set; } = 500;
}
