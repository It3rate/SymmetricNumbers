using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Numbers;

/// <summary>
/// A rational number is made of two segments, where the denominator is a segment from zero to a positive number,
/// and the numerator is a segment from zero to an arbitrary postion.
/// </summary>
public class RationalNumber : UnconstrainedNumber
{
    // the only constraints are that left unit is always -UnitPosition, and left measure is always 0.
    // All operations on complex numbers should apply? (and imaginary part will always be zero)
    // So really it is a 'subclass' of ComplexNumber, but this isn't really an oop system like that.
    public long MeasurePosition
    {
        get => RightMeasure.Position;
        set { RightMeasure.SetPosition(value); LeftMeasure.SetPosition(0); }
    }
    public long UnitPosition
    {
        get => RightUnit.Position;
        set { RightUnit.SetPosition(value); LeftUnit.SetPosition(-value); }
    }
    public long TickCount => MeasurePosition;
    public long Resolution
    {
        get => UnitPosition;
        protected set
        {
            MeasurePosition = (long)(FloatValue * value);
            UnitPosition = value;
        }
    }
    public float FloatValue
    {
        get => RightValue;
        protected set => RightValue = value;
    }
    /// <summary>
    /// A RationalNumber is a complex number with constraints on the left side.
    /// </summary>
    public RationalNumber(long resolution, long value)
    {
        RightMeasure = new Measure(value, Direction.FacesRight, true);
        RightUnit = new Measure(resolution, Direction.FacesRight, true);
        LeftUnit = new Measure(-resolution, Direction.FacesLeft, true);
        LeftMeasure = new Measure(0, Direction.FacesLeft, true);
    }
    public RationalNumber(long resolution, float value) : this(resolution, (long)(value * resolution)) { }

    // ScaleAll - multiply both measure and unit by the same factor, increasing resolution but not changing the value.
    // Value - measure / unit, what we normally consider the number to be.
    // Reciprocal - unit/value.
    // Shift - this is akin to add and subtract, but there is no added value. Essentially it is shifting the zero point. Add requires two things.

    // Add, multiply etc all require a second rational number.

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
