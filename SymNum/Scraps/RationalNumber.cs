using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Numbers;
using SymNum.Primitives;
namespace SymNum.Scraps;

/// <summary>
/// A Real Part number is the right side of a complex number (or split number).
/// This is essentially our normal numbers, but with a resolution component (the ticks in the denominator).
/// It has a unit (bottom right) and a measure (top right).
/// The unit of the imaginary side has to match the real unit, but points in the opposite direction.
/// When combining with an imaginary part into a SplitNumber, the unit of the imaginary part is used.
/// When combining to a complex number (identical units) the min resolution is used
/// </summary>
public class RationalNumber : RawNumber, IPartialNumber
{
    // Q. Rather than a separate rational and imaginary split,
    // perhaps just use a PointNumber, where real and imaginary are the same ref?
    public bool IsRationalPart => true;
    public bool IsImaginaryPart => false;

    // the only constraints are that left unit is always UnitPosition pointing left, and left measure is always 0.
    // All operations on complex numbers should apply? (and imaginary part will always be zero)
    public long MeasurePosition
    {
        get => RightMeasure.Position;
        set { RightMeasure.SetPositionAndInvoke(value); LeftMeasure.SetPositionAndInvoke(0); }
    }
    public long UnitPosition
    {
        get => RightUnit.Position;
        set { RightUnit.SetPositionAndInvoke(value); LeftUnit.SetPositionAndInvoke(-value); }
    }
    public long TickCount => MeasurePosition;
    public long Resolution
    {
        get => UnitPosition;
        protected set
        {
            MeasurePosition = (long)(Value * value);
            UnitPosition = value;
        }
    }
    public float Value
    {
        get => RightValue;
        protected set => RightValue = value;
    }
    /// <summary>
    /// A RationalNumber is a complex number with constraints on the left side.
    /// </summary>
    public RationalNumber(long value, long resolution) :
         this(new Measure(value), new Measure(resolution))
    { }
    public RationalNumber(float value, long resolution) : this((long)(value * resolution), resolution) { }
    public RationalNumber(Measure measure, Measure unit)
    {
        RightMeasure = measure;
        RightUnit = unit;
        LeftUnit = unit;
        LeftMeasure = new Measure(0);
    }
    private static long _highRes = 0x10000;
    public static RationalNumber Zero => new RationalNumber(0, _highRes);
    public static RationalNumber One => new RationalNumber(_highRes, _highRes);
    public static RationalNumber HighResNumber(float num) => new RationalNumber((long)(_highRes * num), _highRes);

    // ScaleAll - multiply both measure and unit by the same factor, increasing resolution but not changing the value.
    // Value - measure / unit, what we normally consider the number to be.
    // Reciprocal - unit/value.
    // Shift - this is akin to add and subtract, but there is no added value. Essentially it is shifting the zero point. Add requires two things.

    // Add, multiply etc all require a second rational number.

    public override string ToString()
    {
        return RightMeasurePosition + "/" + RightUnitPosition + ":" + RightValue.ToString("0.##");
    }

}
