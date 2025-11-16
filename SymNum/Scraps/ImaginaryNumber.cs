using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;
namespace SymNum.Scraps;

/// <summary>
/// A Imaginary Part number is the left side of a complex number (or split number).
/// It has a unit (top left) and a measure (bottom left).
/// If you rotate it 180 degrees, it becomes a RealPartNumber, with the numerator on top.
/// The unit of the real side has to match the imaginary unit, but points in the opposite direction.
/// When combining with a real part into a SplitNumber, the unit of the real part is used.
/// When combining to a complex number (identical units) the min resolution is used
/// </summary>
public class ImaginaryNumber : RawNumber, IPartialNumber
{
    public bool IsRationalPart => false;
    public bool IsImaginaryPart => true;

    // the only constraints are that right unit is always UnitPosition pointing right, and right measure is always 0.
    // All operations on complex numbers should apply? (and real value will always be zero)
    public long MeasurePosition
    {
        get => LeftMeasure.Position;
        set { LeftMeasure.SetPositionAndInvoke(value); LeftMeasure.SetPositionAndInvoke(0); }
    }
    public long UnitPosition
    {
        get => LeftUnit.Position;
        set { LeftUnit.SetPositionAndInvoke(value); LeftUnit.SetPositionAndInvoke(-value); }
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
        get => LeftValue;
        protected set => LeftValue = value;
    }
    /// <summary>
    /// A RationalNumber is a complex number with constraints on the left side.
    /// </summary>
    public ImaginaryNumber(long value, long resolution) :
         this(new Measure(value), new Measure(value))
    { }
    public ImaginaryNumber(float value, long resolution) : this((long)(value * resolution), resolution) { }

    public ImaginaryNumber(IMeasure measure, IMeasure unit)
    {
        LeftMeasure = measure;
        LeftUnit = unit;
        RightUnit = unit;
        RightMeasure = new Measure(0);
    }
    // ScaleAll - multiply both measure and unit by the same factor, increasing resolution but not changing the value.
    // Value - measure / unit, what we normally consider the number to be.
    // Reciprocal - unit/value.
    // Shift - this is akin to add and subtract, but there is no added value. Essentially it is shifting the zero point. Add requires two things.

    // Add, multiply etc all require a second rational number.

}
