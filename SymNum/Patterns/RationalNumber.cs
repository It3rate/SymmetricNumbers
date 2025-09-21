using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Patterns;

/// <summary>
/// A rational number is made of two segments, where the denominator is a segment from zero to a positive number,
/// and the numerator is a segment from zero to an arbitrary postion.
/// </summary>
public class RationalNumber : NumberBase
{
    public long TickCount => Numerator.EndValue;
    public long Resolution{
        get => Denominator.EndValue;
        set
        {
            Numerator.EndValue = (long)(FloatValue * value);
            Denominator.EndValue = value;
        }
    } 
    public float FloatValue
    {
        get =>  (float)Numerator.EndValue / Denominator.EndValue;
        set => Numerator.EndValue = (long)(value * Resolution);
    }
    public RationalNumber(long resolution, long value)
    {
        Numerator = new Extent(0, value, true);
        Denominator = new Extent(0, resolution, true);
    }
    public RationalNumber(long resolution, float value) : this(resolution, (long)(value * resolution)) { }

}
