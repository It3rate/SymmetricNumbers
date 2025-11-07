using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SymNum.Expressions;
using SymNum.Primitives;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace SymNum.Numbers;


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
	public virtual IMeasure RightUnit { get => Measures[2]; protected set => Measures[2] = value; }
	public virtual IMeasure LeftMeasure { get => Measures[3]; protected set => Measures[3] = value; }
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
        ZeroPoint = new Measure(0, Direction.Zero, true, CertaintyLevel.Uncertain);
    }
    protected RawNumber(long tr, long tl, long br, long bl, long zeroPoint = 0)
    {
		RightMeasure = new Measure(tr, Direction.FacesRight, true);
		LeftUnit = new Measure(tl, Direction.FacesLeft, true);
		RightUnit = new Measure(br, Direction.FacesRight, true);
		LeftMeasure = new Measure(bl, Direction.FacesLeft, true);
        ZeroPoint = new Measure(zeroPoint, Direction.Zero);
    }
    /// <summary>
    /// Cast internals of another number into this type.
    /// eg complex to rational will ignore the i values and not allow setting them, but they will be preserved if cast back.
    /// </summary>
    public RawNumber(RawNumber number)
    {
        Array.Copy(number.Measures, Measures, Measures.Length);
    }
    public void SetWith(INumber number)
	{
		Array.Copy(number.Measures, Measures, Measures.Length);
	}

    protected static long _resolution = 1000000;
	protected static RationalNumber CreateRational(float value) => new RationalNumber(value, _resolution);



	public virtual RationalNumber GetRational() => new RationalNumber(RightMeasure, RightUnit);
    public virtual ImaginaryNumber GetImaginary() => new ImaginaryNumber(LeftMeasure, LeftUnit);
    public virtual RationalNumber GetImaginaryAsRational() => new RationalNumber(LeftMeasure, LeftUnit);
    public virtual ImaginaryNumber GetRationalAsImaginary() => new ImaginaryNumber(RightMeasure, RightUnit);
    public virtual ComplexNumber GetComplex() => new ComplexNumber(LeftMeasure, RightMeasure, RightUnit);
	public virtual UnbalancedNumber GetUnbalanced() => new UnbalancedNumber(RightMeasure, RightUnit, LeftUnit, LeftMeasure);
    public virtual SymmetricNumber GetSymmetric() { throw new NotImplementedException(); }
    public void Mirror()
    {
        var temprm = RightMeasure;
        var tempru = RightUnit;
        RightMeasure = LeftMeasure;
        RightUnit = LeftUnit;
        LeftUnit = tempru;
        LeftMeasure = temprm;
	}
	public INumber AddTo(INumber other)
	{
		RightMeasure.SetPosition(RightMeasure.Position + other.RightMeasure.Position);
		LeftMeasure.SetPosition(LeftMeasure.Position + other.LeftMeasure.Position);
		return this;
	}

	public INumber SubtractTo(INumber other)
	{
		RightMeasure.SetPosition(RightMeasure.Position - other.RightMeasure.Position);
		LeftMeasure.SetPosition(LeftMeasure.Position - other.LeftMeasure.Position);
		return this;
	}
	/// <summary>
	/// When resolution exists, aligning one number to another based on ratios is a first order operation.
	/// Normally we consider 1/2 and 2/4 to be the same number, but with resolution they aren't, and they need to be convertible.
	/// </summary>
	public INumber AlignTo(INumber other)
	{
		MapToValues(other.LeftValue, other.RightValue);
		return this;
	}
	public INumber Add(INumber other)
	{
		var r = RightMeasure.Position + other.RightMeasure.Position;
		var l = LeftMeasure.Position + other.LeftMeasure.Position;
        return new UnbalancedNumber(r, LeftUnit.Position, RightUnit.Position, l);

	}

	public INumber Subtract(INumber other)
	{
		var r = RightMeasure.Position - other.RightMeasure.Position;
		var l = LeftMeasure.Position - other.LeftMeasure.Position;
		return new UnbalancedNumber(r, LeftUnit.Position, RightUnit.Position, l);
	}

	public INumber MultiplyTo(INumber other)
    {
        throw new NotImplementedException();
    }

    public INumber DivideTo(INumber other)
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
	public float SumValue => LeftValue + RightValue;
	public float LeftMinusRightValue => LeftValue - RightValue;
	public float RightMinusLeftValue => RightValue - LeftValue;
	public float ProductValue => LeftValue * RightValue;
	public float LeftOverRightValue => RightValue != 0 ? LeftValue / RightValue : Math.Sign(LeftValue) > 0 ? float.MaxValue : float.MinValue;
	public float RighOverLeftValue => LeftValue != 0 ? RightValue / LeftValue : Math.Sign(RightValue) > 0 ? float.MaxValue : float.MinValue;

	public INumber MapLeftToValue(float leftValue)
	{
		LeftMeasure.SetPosition((long)(leftValue * LeftUnit.Position));
		return this;
	}
	public INumber MapRightToValue(float rightValue)
	{
		RightMeasure.SetPosition((long)(rightValue * RightUnit.Position));
		return this;
	}
	public INumber MapToValues(float leftValue, float rightValue)
	{
		LeftMeasure.SetPosition((long)(leftValue * LeftUnit.Position));
		RightMeasure.SetPosition((long)(rightValue * RightUnit.Position));
		return this;
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


	#region Bool Ops
	//public bool FullyContains(Number toTest, bool includeEndpoints = true) => toTest != null ? Value.FullyContains(toTest.Value, includeEndpoints) : false;

	//// segment comparison considers unot numbers to also have positive segments in the negative space, and
	//// unit numbers to be unot in its negative space. So two unit numbers that have a point on both ends of a range,
	//// can be considered an ordinary unot number. eg: [+++>....++>] == [....<---...]
	//// This should also handle things like gt lt div0 etc

	////  0000	Never			0			FALSE
	////  0001	Both            A ^ B       AND
	////  0010	Only A          A ^ !B      A AND NOT B
	////  0011	A Maybe B       A           A
	////  0100	Only B			!A ^ B      NOT A AND B
	////  0101	B Maybe A       B           B
	////  0110	One of          A xor B     XOR
	////  0111	At least one    A v B       OR
	////  1000	No one          A nor B     NOR
	////  1001	Both or no one  A XNOR B    XNOR
	////  1010	A or no one		!B          NOT B
	////  1011	Not B alone     A v !B      A OR NOT B
	////  1100	B or no one		!A          NOT A
	////  1101	Not A alone		!A v B      NOT A OR B
	////  1110	Not both        A nand B    NAND
	////  1111	Always			1			TRUE

	///*
	//function BooleanOperations()
	//{
	//  this.Never = function(a, b) { return 0; };
	//  this.And = function(a, b) { return a & b; };
	//  this.LeftandNotRight = function(a, b) { return a & ~b; };
	//  this.Left = function(a, b) { return a; };
	//  this.NotLeftAndRight = function(a, b) { return ~a & b; };
	//  this.Right = function(a, b) { return b; };
	//  this.Xor = function(a, b) { return a ^ b; };
	//  this.Or = function(a, b) { return a | b; };
	//  this.Nor = function(a, b) { return ~(a | b); };
	//  this.Xnor = function(a, b) { return ~(a ^ b); };
	//  this.NotRight = function(a, b) { return ~b; };
	//  this.LeftOrNotRight = function(a, b) { return a | ~b; };// implies
	//  this.NotLeft = function(a, b) { return ~a; };
	//  this.RightOrNotLeft = function(a, b) { return ~a | b; }; // implies
	//  this.Nand = function(a, b) { return ~(a & b); };
	//  this.Always = function(a, b) { return 1; };
	//}

	//*/
	//#endregion
	//#region Interpolation
	//public void InterpolateFromZero(Number t, Number result) => InterpolateFromZero(this, t, result);
	//public void InterpolateFrom(Number source, Number t, Number result) => Interpolate(source, this, t, result);
	//public void InterpolateTo(Number target, Number t, Number result) => Interpolate(this, target, t, result);
	//public static void InterpolateFromZero(Number target, Number t, Number result)
	//{
	//    var targetValue = target.Value;
	//    var tValue = t.Value;
	//    result.StartValue = targetValue.Start * tValue.Start;
	//    result.EndValue = targetValue.End * tValue.End;
	//}
	//public static void InterpolateFromOne(Number target, Number t, Number result)
	//{
	//    var targetValue = target.Value;
	//    var tValue = t.Value;
	//    result.StartValue = targetValue.Start * tValue.Start;
	//    result.EndValue = (targetValue.End - 1.0) * tValue.End + 1.0;
	//}
	//public void InterpolateFromOne(Number target, double t)
	//{
	//    if (target != null)
	//    {
	//        var targetValue = target.Value;
	//        StartValue = targetValue.Start * t;
	//        EndValue = (targetValue.End - 1.0) * t + 1.0;
	//    }
	//}
	//public void InterpolateFromOne(PRange range, double t)
	//{
	//    if (range != null)
	//    {
	//        StartValue = range.Start * t;
	//        EndValue = (range.End - 1.0) * t + 1.0;
	//    }
	//}
	//public static void Interpolate(Number source, Number target, Number t, Number result)
	//{
	//    var sourceValue = source.Value;
	//    var targetValue = target.Value;
	//    var tValue = t.Value;
	//    result.StartValue = (targetValue.Start - sourceValue.Start) * tValue.Start + sourceValue.Start;
	//    result.EndValue = (targetValue.End - sourceValue.End) * tValue.End + sourceValue.End;
	//}
	//#endregion
	//#region Internal Segments
	//// todo: public segments should be a class added with composition
	///// <summary>
	///// The number of subnumbers, will always be 1 for normal numbers, can be 0->n for masked or groups numbers.
	///// </summary>
	//public virtual int Count => 1;
	//public virtual long[] GetPositions()
	//{
	//    return new long[] { StartTickPosition, EndTickPosition };
	//}
	//public virtual void ClearInternalPositions() { }
	//public virtual void AddPosition(long start, long end)
	//{
	//    StartTickPosition = start;
	//    EndTickPosition = end;
	//}
	//public virtual void AddPosition(Focal focal)
	//{
	//    AddPosition(focal.StartPosition, focal.EndPosition);
	//}
	//public virtual void AddPosition(Number num)
	//{
	//    AddPosition(num.Focal.StartPosition, num.Focal.EndPosition);
	//}
	//public virtual void AddPosition(PRange range)
	//{
	//    var focal = Domain.CreateFocalFromRange(range);
	//    AddPosition(focal.StartPosition, focal.EndPosition);
	//}
	//public virtual IEnumerable<Number> InternalNumbers()
	//{
	//    if (IsValid)
	//    {
	//        yield return this;
	//    }
	//}
	//// probably need a ratio type, where first digit is ticks, and second ticks per unit.
	///// <summary>
	///// NumberGroups can have overlapping numbers, so this segmented version returns all partial ranges for each possible segment.
	///// Assumes aligned domains.
	///// </summary>
	//public static (long[], List<Number[]>) SegmentedTable(Domain domain, bool allowOverlap, params Number[] numbers)
	//{
	//    var result = new List<Number[]>();
	//    var internalNumberSets = new List<Number[]>();
	//    var sPositions = new SortedSet<long>();
	//    foreach (var number in numbers)
	//    {
	//        if (number.IsValid)
	//        {
	//            internalNumberSets.Add(number.InternalNumbers().ToArray());
	//            sPositions.UnionWith(number.GetPositions());
	//        }
	//        else
	//        {
	//            internalNumberSets.Add([]);
	//        }
	//    }
	//    var positions = sPositions.ToArray();

	//    for (int i = 1; i < positions.Length; i++)
	//    {
	//        var focal = new Focal(positions[i - 1], positions[i]);
	//        var matches = new List<Number>();
	//        foreach (var numSet in internalNumberSets)
	//        {
	//            if (numSet.Length == 0)
	//            {
	//                matches.Add(CreateSubsegment(domain, focal, Polarity.None));
	//            }
	//            else
	//            {
	//                foreach (var number in numSet)
	//                {
	//                    var intersection = Focal.Intersection(number.Focal, focal);
	//                    if (intersection != null)
	//                    {
	//                        matches.Add(CreateSubsegment(domain, intersection, number.Polarity));
	//                    }
	//                    else if (allowOverlap)
	//                    {
	//                        matches.Add(CreateSubsegment(domain, focal, Polarity.None)); // False is polarity none
	//                    }
	//                }
	//            }
	//        }
	//        result.Add(matches.ToArray());
	//    }
	//    return (positions, result);
	//}
	//protected static Number CreateSubsegment(Domain domain, Focal focal, Polarity polairty = Polarity.None)
	//{
	//    var result = new Number(focal.Clone(), polairty); // false
	//    result.Domain = domain;
	//    return result;
	//}
	//protected static (Focal[], Polarity[], bool) ApplyOpToSegmentedTable(List<Number[]> data, OperationKind operation)
	//{
	//    var focals = new List<Focal>();
	//    var polarities = new List<Polarity>();
	//    var dirResult = false;
	//    Focal? lastFocal = null;
	//    Polarity lastPolarity = Polarity.Unknown;

	//    foreach (var seg in data)
	//    {
	//        if (seg.Length == 0)
	//        {
	//        }
	//        else
	//        {
	//            var first = seg[0];
	//            var opResult = false;
	//            var polResult = false;
	//            var hasUndefSeg = false;
	//            var segDir = false;
	//            for (int i = 0; i < seg.Length; i++)
	//            {
	//                var curNum = seg[i];
	//                if (!curNum.Polarity.HasPolarity())
	//                {
	//                    hasUndefSeg = true; // undefined polarity is not considered for direction
	//                }
	//                var func = BoolOperation.GetFunc(operation);
	//                if (i == 0)
	//                {
	//                    polResult = curNum.IsAligned;
	//                    segDir = curNum.IsUnitPositivePointing;
	//                    opResult = curNum.HasPolairty;
	//                }
	//                else
	//                {
	//                    polResult = func(polResult, curNum.IsAligned); ;
	//                    segDir = func(segDir, curNum.IsUnitPositivePointing);
	//                    opResult = func(opResult, curNum.HasPolairty);
	//                }
	//            }
	//            if (!hasUndefSeg)
	//            {
	//                dirResult = segDir;
	//            }

	//            if (opResult)
	//            {
	//                var focal = new Focal(first.MinTickPosition, first.MaxTickPosition);// dirResult ? new Focal(first.MinTickPosition, first.MaxTickPosition) : new Focal(first.MaxTickPosition, first.MinTickPosition);
	//                var polarity = polResult ? Polarity.Aligned : Polarity.Inverted;
	//                if (lastFocal != null)// && lastPolarity == polarity && lastFocal.IsPositiveDirection == focal.IsPositiveDirection) // merge continuous segments
	//                {
	//                    lastFocal.EndPosition = focal.EndPosition;
	//                    //if (lastFocal.IsPositiveDirection)
	//                    //{
	//                    //    lastFocal.EndPosition = focal.EndPosition;
	//                    //}
	//                    //else
	//                    //{
	//                    //    lastFocal.StartPosition = focal.StartPosition;
	//                    //}
	//                }
	//                else
	//                {
	//                    focals.Add(focal);
	//                    polarities.Add(polarity);
	//                    lastFocal = focal;
	//                    lastPolarity = polarity;
	//                }
	//            }
	//            else
	//            {
	//                lastFocal = null;
	//            }
	//        }
	//    }

	//    return (focals.ToArray(), polarities.ToArray(), dirResult);

	//}
	//#endregion

	//#region Equality
	//public Number Clone(bool addToStore = true)
	//{
	//    var result = new Number(Focal.Clone(), Polarity);
	//    return Domain.AddNumber(result, addToStore);
	//}
	//public static bool operator ==(Number? a, Number? b)
	//{
	//    if (a is null && b is null)
	//    {
	//        return true;
	//    }
	//    if (a is null || b is null)
	//    {
	//        return false;
	//    }
	//    return a.Equals(b);
	//}
	//public static bool operator !=(Number? a, Number? b)
	//{
	//    return !(a == b);
	//}
	//public override bool Equals(object? obj)
	//{
	//    return obj is Number other && Equals(other);
	//}
	//public bool Equals(Number? value)
	//{
	//    if (value is null) { return false; }
	//    return ReferenceEquals(this, value) ||
	//            (
	//            //IsValid && value.IsValid &&
	//            Polarity == value.Polarity &&
	//            Focal.Equals(this.Focal, value.Focal)
	//            );
	//}
	//public override int GetHashCode()
	//{
	//    unchecked
	//    {
	//        var hashCode = Focal.GetHashCode() * 17 ^ ((int)Polarity + 27) * 397;// + (IsValid ? 77 : 33);
	//        return hashCode;
	//    }
	//}
	#endregion

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
