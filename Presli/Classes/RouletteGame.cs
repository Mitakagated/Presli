using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presli.Classes;
public class RouletteGame
{
    private List<int> _nums;

    public RouletteGame()
    {
        _nums = Enumerable.Range(0, 36).ToList();
        _nums.Insert(1, 00);
    }

    private enum Choice
    {
        One,
        Two,
        Three,
        Six,
        Black,
        Red,
        Even,
        Odd,
        OneToTwelve,
        ThirteenToTwentyFour,
        TwentyfiveToThirtySix,
        FirstHalf,
        SecondHalf
    }

    public int PullNumber()
    {
        _nums = _nums.OrderBy(x => Random.Shared.Next()).ToList();
        return _nums[0];
    }
}
