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
public class RationalNumber : UnconstrainedNumber
{
    public long MeasurePosition
    {
        get => RightMeasure.Position;
        set => RightMeasure.SetPosition(value);
    }
    public long UnitPosition
    {
        get => RightUnit.Position;
        set => RightUnit.SetPosition(value);
    }
    public long TickCount => MeasurePosition;
    public long Resolution{
        get => UnitPosition;
        protected set
        {
            MeasurePosition = (long)(FloatValue * value);
            UnitPosition = value;
        }
    } 
    public float FloatValue
    {
        get =>  RightValue;
        protected set => RightValue = value;
    }
    public RationalNumber(long resolution, long value)
    {
        LeftMeasure = Measure.ZeroUnknown();
        LeftUnit = Measure.ZeroUnknown();
        RightMeasure = new Measure(value, Direction.Unknown, true);
        RightUnit = new Measure(resolution, Direction.FacesRight, true);
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
