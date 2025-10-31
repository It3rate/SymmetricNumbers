using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Measures.Expressions;
using SymNum.Measures.Primitives;

namespace SymNum.Measures.Numbers;


/// <summary>
/// There are many types of numbers, they just mean measures calibrated in some way. 
/// Left units and measures are positive left, so do not negate them unless pointing right.
/// This is the base class which can grow into numbers, complex numbers, symmetric numbers, dynamic zero point numbers,
/// equations, patterns etc. All of these are just ways of organizing and calibrating values to have meaning 
/// (aka be non random).
/// Note that these can describe any non random pattern, but have no information about what the pattern represents.
/// This requires some kind of unit tied to the number. Numbers themselves are pure patterns.
/// </summary>
public class RawNumber : INumber
{
    public static RawNumber Unknown => new RawNumber();
    // A number can be composed of multiple measures with different restrictions. 
    // A simple number has a numerator and denominator, where a position in 3D space has 12 measures (6 extents, 3 numbers).
    // An equation (eg a sinewave, bezier curve, color curve of heating) can be arbitrarily complex, 
    // but they are all 'numbers' for lack of a better word. Values are always constructed, even in the number 9 (which is 9 * unit).

    // needs to be protected to allow subclasses to enforce their constraints.

    // B | A   // A/D forms the real part, and C/B the imaginary part
    // C | D   // this is so the whole quad can be rotated 180 and show the same values for the i perspective.
    // zero point is E
    public virtual IMeasure[] Measures { get; } = new IMeasure[5];
    public virtual IMeasure RightMeasure { get => Measures[0]; protected set => Measures[0] = value; }
	public virtual IMeasure RightUnit { get => Measures[3]; protected set => Measures[3] = value; }
	public virtual IMeasure LeftMeasure { get => Measures[2]; protected set => Measures[2] = value; }
	public virtual IMeasure LeftUnit { get => Measures[1]; protected set => Measures[1] = value; }
	public virtual IMeasure ZeroPoint { get => Measures[4]; protected set => Measures[4] = value; }

	public long RightMeasurePosition => RightMeasure.Position;
	public long RightUnitPosition => RightUnit.Position;
	public long LeftMeasurePosition => LeftMeasure.Position;
	public long LeftUnitPosition => LeftUnit.Position;
    public long ZeroPosition => ZeroPoint.Position;

	public virtual bool IsFullyDefined => Measures.All(m => m.IsFullDefined);

    protected RawNumber()
    {
        RightMeasure = new Measure(0, Direction.FacesRight, true, CertaintyLevel.Uncertain);
        LeftUnit = new Measure(0, Direction.FacesLeft, true, CertaintyLevel.Uncertain);
        RightUnit = new Measure(0, Direction.FacesRight, true, CertaintyLevel.Uncertain);
        LeftMeasure = new Measure(0, Direction.FacesLeft, true, CertaintyLevel.Uncertain);
        ZeroPoint = new Measure(0, Direction.Unknown, true, CertaintyLevel.Uncertain);
    }
    protected RawNumber(long tr, long tl, long br, long bl, long zeroPoint = 0)
    {
		RightMeasure = new Measure(tr, Direction.FacesRight, true);
		LeftUnit = new Measure(tl, Direction.FacesLeft, true);
		RightUnit = new Measure(br, Direction.FacesRight, true);
		LeftMeasure = new Measure(bl, Direction.FacesLeft, true);
        ZeroPoint = new Measure(zeroPoint, Direction.Unknown);
    }
    /// <summary>
    /// Cast internals of another number into this type.
    /// eg complex to rational will ignore the i values and not allow setting them, but they will be preserved if cast back.
    /// </summary>
    public RawNumber(RawNumber number)
    {
        Array.Copy(number.Measures, Measures, Measures.Length);
    }

    protected static long _resolution = 1000000;
	protected static RationalNumber CreateRational(float value) => new RationalNumber(value, _resolution);



	public virtual RationalNumber GetRational() { throw new NotImplementedException(); }
	public virtual ImaginaryNumber GetImaginary() { throw new NotImplementedException(); }
	public virtual ComplexNumber GetComplex() { throw new NotImplementedException(); }
	public virtual UnbalancedNumber GetUnbalanced() { throw new NotImplementedException(); }
	public virtual SymmetricNumber GetSymmetric() { throw new NotImplementedException(); }

	public INumber Add(INumber other)
    {
        throw new NotImplementedException();
    }

    public INumber Subtract(INumber other)
    {
        throw new NotImplementedException();
    }

    public INumber Multiply(INumber other)
    {
        throw new NotImplementedException();
    }

    public INumber Divide(INumber other)
    {
        throw new NotImplementedException();
    }

    public IMeasure MeasureFromIndex(int index)
	{
		if (index < 0 || index >= Measures.Length)
		{
			throw new ArgumentOutOfRangeException("MeasureFromIndex index");
		}
		return Measures[index];
	}
	public ValueRef CreateValueRef(RefKind refKind) => new ValueRef(this, refKind);
	public IValue ValueFromRefKind(RefKind refKind)
	{
		IValue result = EmptyValue.Instance;
        switch (refKind)
        {
            case RefKind.TopRight:
                result = RightMeasure;
                break;
            case RefKind.TopLeft:
                result = LeftUnit;
                break;
            case RefKind.BottomRight:
                result = RightUnit;
                break;
            case RefKind.BottomLeft:
                result = LeftUnit;
                break;
			case RefKind.RealValue:
				result = new ValueDouble(RightValue);
				break;
			case RefKind.ImaginaryValue:
				result = new ValueDouble(LeftValue);
				break;
			//case RefKind.UnitAverage:
			//	result = new ValueDouble((LeftUnit.Value + RightUnit.Value) / 2.0);
			//	break;
			//case RefKind.ValueAverage:
			//	result = new ValueDouble((LeftMeasure.Value + RightMeasure.Value) / (LeftUnit.Value + RightUnit.Value));
			//	break;
		}
        return result;
	}
    public void SetValueFromRefKind(RefKind refKind, double value)
	{
        switch (refKind)
        {
            case RefKind.TopRight:
                RightMeasure.SetPosition((long)value);
                break;
            case RefKind.TopLeft:
                LeftUnit.SetPosition((long)value);
                break;
            case RefKind.BottomRight:
                RightUnit.SetPosition((long)value);
                break;
            case RefKind.BottomLeft:
                LeftUnit.SetPosition((long)value);
                break;
			case RefKind.RealValue:
                // need to check for locks etc.
				break;
			case RefKind.ImaginaryValue:
				// need to check for locks etc.
				break;
		}
	}

    //public static bool GetBool(IMeasure a, IMeasure b,)
    public static float GetValue(IMeasure numerator, IMeasure denominator)
    {
        var result = float.NaN;
        // todo: Need to apply the math of uncertaintly to generate a range or probability distribution.
		//if (numerator.IsFullDefined && denominator.IsFullDefined && denominator.Position != 0)
		if (denominator.Position != 0)
		{
            result = numerator.Position / (float)denominator.Position;
        }
        else
        {
            result = numerator.Position > 0 ? float.MaxValue : (numerator.Position < 0 ? float.MinValue : 0f);
        }
        return result;
    }
    public static bool SetValue(IMeasure numerator, IMeasure denominator, float value)
    {
        var result = false;
        if (numerator.IsFullDefined && denominator.IsFullDefined && denominator.Position != 0)
        {
            numerator.SetPosition((long)(value * denominator.Position));
            result = true;
        }
        return result;
    }



    /// <summary>
    /// RightValue is the positive polarity value, up vs down, right vs left, large vs small etc.
    /// We generally have opposites in a definition, and one of them is 'dominant', like getting bigger (up) vs getting smaller (down).
    /// </summary>
    public float RightValue
    {
        get => GetValue(RightMeasure, RightUnit);
        set => SetValue(RightMeasure, RightUnit, value);
    }
    /// <summary>
    /// LeftValue is the negative polarity value, down vs up, left vs right, small vs large etc.
    /// We generally have opposites in a definition, and one of them is 'dominant', like getting bigger (up) vs getting smaller (down).
    /// </summary>
    public float LeftValue
    {
        get => GetValue(LeftMeasure, LeftUnit);
        set => SetValue(LeftMeasure, LeftUnit, value);
    }

    // there is a general question with operations, what happens to the original value? Three possibilities:
    // 1) It can be modified in place, like 'gaining' in the additive sense, stretching, or exponentially growing (repeated stretching)
    // 2) It can be combined with something else to create a new value, like adding, or repeatedly adding (multiply, but use additive exponents 2+2+2 vs 2x3), and repeat that.
    // 3) It can be unchanged, but the perspective shifts. Like move the zero point and a static segment shifts (+), or move closer and it grows (*).
    // The system of numbers and operations should be able to express all three of these without metadata (words that tell of the operation).
    // When modeling the world and its transforms using numbers, these operations matter.

    // B | A
    // D | C
    // Multiply         A/C * D/B
    // Relation         A/C / D/B
    // Ratio            A/C | D/B
    // DotProduct       A*B/C*D
    // CrossProduct     A*D/B*C
    // NormalizeMin     resolution is min(B,C), scale other (BD or AC) to this resolution -- needed to not overstate resolution, may be more elaborate or better expressed as a range
    // NormalizeMax     resolution is max(B,C), scale other (BD or AC) to this resolution
    // ComplexAdd       NormalizeMin, (D+A)/B <- B and C will be same value after norm
    // ComplexSubtract  NormalizeMin, (D-A)/B <- B and C will be same value after norm
    // ComplexMult      NormalizeMin, A=(AC-BD), B=-res, C=res, D=(AD+BC)




    //public int AbsDirection => HasTwoValues ? (EndValue > StartValue ? 1 : (EndValue < StartValue ? -1 : 0)) : 0;
    //public bool IsPositiveDirection => IsPositiveRight ? AbsDirection > 0 : AbsDirection < 0;
    //public bool IsNegativeDirection => !IsPositiveDirection;
    //public bool IsAmbiguousDirection => AbsDirection == 0;
    //public long Min => StartValue <= EndValue ? StartValue : EndValue;
    //public long Max => StartValue >= EndValue ? StartValue : EndValue;
    //public long DirectionalMin => IsPositiveDirection ? Min : Max;
    //public long DirectionalMax => IsPositiveDirection ? Max : Min;
    //public bool CanHaveDirection => HasTwoValues;

}

public abstract class NumberValue
{
    public RawNumber NumberReference { get; }
    public abstract long RightResolution { get; }
    public abstract long LeftResolution { get; }

    public NumberValue(RawNumber number)
    {
        NumberReference = number;
    }
}

public class AnnotatedFloat
{
    public float Value { get; set; }
    public float Resolution { get; set; }
    public bool IsPositiveRight { get; set; }
    public AnnotatedFloat(float value, float resolution, bool isPositiveRight)
    {
        Value = value;
        Resolution = resolution;
        IsPositiveRight = isPositiveRight;
    }
}
