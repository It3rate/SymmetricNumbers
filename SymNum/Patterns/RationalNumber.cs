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
    public long TickCount => RightExtent.EndPosition;
    public long Resolution{
        get => RightUnit;
        protected set
        {
            RightMeasure = (long)(FloatValue * value);
            RightUnit = value;
        }
    } 
    public float FloatValue
    {
        get =>  RightValue;
        protected set => RightValue = value;
    }
    public RationalNumber(long resolution, long value)
    {
        RightExtent = new Extent(value, resolution);
        LeftExtent = new Extent(0, 0);
    }
    public RationalNumber(long resolution, float value) : this(resolution, (long)(value * resolution)) { }

}
public class RationalValue : NumberValue
{
    public RationalNumber Reference { get; }
    public float Value => Reference.FloatValue;
    public override long RightResolution => Reference.Resolution;
    public override long LeftResolution => 0;

    public RationalValue(RationalNumber rationalNumber) : base(rationalNumber)
    {
        Reference = rationalNumber;
    }
}
