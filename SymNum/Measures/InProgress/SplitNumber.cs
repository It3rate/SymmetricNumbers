namespace SymNum.Measures.Numbers;

using System;
using System.Data;
using Microsoft.VisualBasic;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;
using SymNum.Measures.Primitives;

// There needs to be interoporability between measures, Numbers, and Number equations.
// Gradients are about the 0-1 part of interpolation, but interpolation has that AND two endpoints.
// A SplitNumber (or complex number) defines the two endpoints. The interpolation point can be the zero, or defined elsewhere.
// When merging resolutions into a complex number, you don't always pick the minimum. For example:
//  - if time is one component, you can move from seconds to miliseconds (you need to track the meaning of the resolution though)
//  - if you have samples per minute over hours, your resolution per minute is higher through statistics.

/// <summary>
/// A SplitNumber is a complex number where the two units do no need to be the same resolution.
/// They still need to point in opposite directions, and the right side is still the real part.
/// </summary>
public class SplitNumber : UnconstrainedNumber
{
    public long Real { get => RightMeasure.Position; private set => RightMeasure.SetPosition(value); } 
    public long RealResolution { get => RightUnit.Position; private set => RightUnit.SetPosition(value); }
    public long Imaginary { get => LeftMeasure.Position; private set => LeftMeasure.SetPosition(value); }
	public long ImaginaryResolution { get => LeftUnit.Position; private set => LeftUnit.SetPosition(value); }


	public long MinResolution => Math.Min(RealResolution, ImaginaryResolution);
    public long MaxResolution => Math.Max(RealResolution, ImaginaryResolution);
	public RationalNumber RealPart => new RationalNumber(RealResolution, Real);
	public RationalNumber ImaginaryPart => new RationalNumber(RealResolution, Real);


    // long tr, long tl, long br, long bl)
    public SplitNumber(long? real, long? imaginary, long? realResolution, long? imaginaryResolution)
    {
        RightMeasure = new Measure(real ?? 0, Direction.FacesRight, true);
        LeftUnit = new Measure(imaginary ?? 0, Direction.FacesLeft, true);
        RightUnit = new Measure(realResolution ?? 0, Direction.FacesRight, true);
        LeftMeasure = new Measure(imaginaryResolution ?? 0, Direction.FacesLeft, true);

        var rstate = (real != null && realResolution != null) ? CertaintyLevel.Set : CertaintyLevel.Unknown;
        RealState = new BinaryExpression(
            CreateValueRef(RefKind.TopRight),
			CreateValueRef(RefKind.BottomRight), 
            BinaryOp.Divide, rstate);
        var iState = (imaginary != null && imaginaryResolution != null) ? CertaintyLevel.Set : CertaintyLevel.Unknown;
		ImaginaryState = new BinaryExpression(
			CreateValueRef(RefKind.BottomLeft),
			CreateValueRef(RefKind.TopLeft),
            BinaryOp.Divide, rstate);
		var realRef = new ValueRef(this, RefKind.RealValue);
		var imagRef = new ValueRef(this, RefKind.ImaginaryValue);
		RatioState = new BinaryExpression(realRef, imagRef, BinaryOp.Divide);
		AreaState = new BinaryExpression(realRef, imagRef, BinaryOp.Multiply);
		Calculate();
	}
	public ComplexNumber GetComplexNumber()
	{
		var res = MinResolution;
		var real = (long)(RealValue * res);
		var imag = (long)(ImaginaryValue * res);
		return new ComplexNumber(res, imag, real);
	}

    // Need to allow setting by area or ratio by fixing real or imaginary parts.
    // Mimics setting voltage and power by knowing current and resistance,
    // or setting width and aspect ratio by knowing area and length, etc.
	public RationalNumber Area => new RationalNumber(MinResolution, (long)(RealValue * ImaginaryValue * MinResolution));
	public RationalNumber RatioReal => new RationalNumber(MinResolution, (long)((RealValue / ImaginaryValue) * MinResolution));
	public RationalNumber RatioImaginary => new RationalNumber(MinResolution, (long)((ImaginaryValue / RealValue) * MinResolution));
	public RationalNumber ActualRatioReal => new RationalNumber(Real, Imaginary);
	public RationalNumber ActualRatioImaginary => new RationalNumber(Imaginary, Real);




    // the natural constraints of a split number are r=rm/ru and i=im/iu.
    // Changing the real unit will either change the real value or the real measure, so one must be locked.
    // (locking the value changes the resolution, locking the measure changes the value and resolution.)

	#region Fix and Transform
    // this is going to be its own class, where you can assert relations in a number
    // and then lock the number into certain behaviours and patterns with them.

    // the inputs here need to be 'equations' of some sort, which includes ValueSamples (numbers with ops).

	public override bool IsFullyDefined =>
		base.IsFullyDefined &&
		RealState.IsSet && ImaginaryState.IsSet && AreaState.IsSet && RatioState.IsSet;
	public int SetSlotsValue =>
		RealState.CertaintyLevel.SlotValue(MultiplicativeRelation.Real) +
		RatioState.CertaintyLevel.SlotValue(MultiplicativeRelation.Ratio) +
		ImaginaryState.CertaintyLevel.SlotValue(MultiplicativeRelation.Imaginary) +
		AreaState.CertaintyLevel.SlotValue(MultiplicativeRelation.Area);

	public BinaryExpression RealState { get; private set; }
	public BinaryExpression ImaginaryState { get; private set; }
	public BinaryExpression AreaState { get; private set; }
	public BinaryExpression RatioState { get; private set; }
	protected double RealValue => Real / (double)RealResolution;
	protected double ImaginaryValue => Imaginary / (double)ImaginaryResolution;
	public double AreaValue => RealValue * ImaginaryValue;
	public double RatioValue => RealValue / ImaginaryValue;

    private void UpdateStates()
	{
		RealState.UpdateValue(RealValue);
		ImaginaryState.UpdateValue(ImaginaryValue);
        RatioState.UpdateValue(RatioValue);
        AreaState.UpdateValue(AreaValue);

	}
	public CertaintyLevel GetState(MultiplicativeRelation kind)
    {
        var result = CertaintyLevel.Unknown;
        switch (kind)
        {
            case MultiplicativeRelation.Real:
                result = RealState.CertaintyLevel;
                break;
            case MultiplicativeRelation.Ratio:
                result = RatioState.CertaintyLevel;
                break;
            case MultiplicativeRelation.Imaginary:
                result = ImaginaryState.CertaintyLevel;
                break;
            case MultiplicativeRelation.Area:
                result = AreaState.CertaintyLevel;
                break;
        }
        return result;
    }
    public long GetValue(MultiplicativeRelation kind)
    {
        long result = 0; // probably should be max or min based on direction?
        if (GetState(kind).HasValue())
        {
            switch (kind)
            {
                case MultiplicativeRelation.Real:
                    result = Real;
                    break;
                case MultiplicativeRelation.Ratio:
                    result = RealResolution;
                    break;
                case MultiplicativeRelation.Imaginary:
                    result = Imaginary;
                    break;
                case MultiplicativeRelation.Area:
                    result = ImaginaryResolution;
                    break;
            }
        }
        return result;
    }
    public void FixValue(MultiplicativeRelation kind)
    {
        switch (kind)
        {
            case MultiplicativeRelation.Real: 
                RealState.Fix(); 
                break;
            case MultiplicativeRelation.Ratio:
                RatioState.Fix(); 
                break;
            case MultiplicativeRelation.Imaginary: 
                ImaginaryState.Fix(); 
                break;
            case MultiplicativeRelation.Area:
                AreaState.Fix(); 
                break;
        }
    }
    public void SetValue(MultiplicativeRelation kind, long value)
    {
        switch (kind)
        {
            case MultiplicativeRelation.Real:
                Real = value;
                RealState.Set();
                break;
            case MultiplicativeRelation.Ratio:
                RatioState.Set();
                break;
            case MultiplicativeRelation.Imaginary:
                Imaginary = value;
                ImaginaryState.Set();
                break;
            case MultiplicativeRelation.Area:
                AreaState.Set();
                break;
        }
	}

	public void SetAll()
	{
		RealState.Set();
		ImaginaryState.Set();
		RatioState.Set();
		AreaState.Set();
	}
	public void UnsetAll()
	{
		RealState.Unset();
		ImaginaryState.Unset();
		RatioState.Unset();
		AreaState.Unset();
	}

	public void SetAndFix(MultiplicativeRelation toSet, long value, MultiplicativeRelation toFix)
	{
		UpdateStates();
		SetAll();
        FixValue(toFix);
        SetValue(toSet, value);
        Calculate();
    }
    public void SetTwo(MultiplicativeRelation setA, long valueA, MultiplicativeRelation setB, long valueB)
    {
        UpdateStates();
		SetAll();
        SetValue(setA, valueA);
        SetValue(setB, valueB);
        Calculate();
    }
    /// <summary>
    /// Use set and fixed/unknown to calculate all values, and then set them to 'set' when done.
    /// If not calculable then things turn to ranges with unknowns.
    /// </summary>
    private void Calculate()
    {
        // Calculate values based on what is set/fixed, and what is unknown/uncertain
        var ssv = SetSlotsValue;
        var canFullyCalc = true;
        //Real = 1,
        //Imaginary = 2,
        //Area = 4,
        //Ratio = 8,
        switch (ssv)
		{
			case 3: // RI
			case 7: // RIA
			case 11: // RIRa
				SetWithRI();
                break;
            case 5: // RA
			case 13: // RARa
				SetWithRA();
                break;
            case 6: // IA
			case 14: // IARa
				SetWithIA();
                break;
            case 9: // RRa
                SetWithRRa();
                break;
            case 10: // IRa
                SetWithIRa();
                break;
            case 12: // ARa
                SetWithARa();
                break;
            default:
                canFullyCalc = false;
                break;
        }
        if (canFullyCalc)
        {
            SetAll();
        }
    }
    /// <summary>
    /// If you set Area, then Imaginary and Real can be anything that multiplies to the Area, and Aspect is the raito.
    /// This makes them uncertain, but still constrained, and the value becomes at best a probability based on history.
    /// It also allows for sampling. You can sample different currents based on a known Area and see how it fits when fully calculated.
    /// This is true of any gradients in this pattern, change the area of a rect, and you are free to sample any reals, lengths, or aspect ratios to test.
    /// This is sort of the concept separated from the instance, which is the key to encoding patterns.
    /// </summary>
    private void SetWithArea()
    {
        // todo: this results in an ambiguous but constrained value.
    }
    private void SetWithRI()
    {
        UpdateStates();
    }
    private void SetWithRA()
    {
        Imaginary = (long)(AreaState.Value / RealState.Value);
		UpdateStates();
	}
    private void SetWithIA()
    {
        Real = (long)(AreaState.Value / ImaginaryState.Value);
		UpdateStates();
	}
    private void SetWithRRa()
    {
        Imaginary = (long)((1.0 / RatioState .Value) * RealState.Value);
		UpdateStates();
	}
    private void SetWithIRa()
    {
        Real = (long)(( RatioState .Value) * ImaginaryState.Value);
		UpdateStates();
	}
    private void SetWithARa()
	{
		Real = (long)Math.Sqrt(AreaState.Value * RatioState.Value);
		Imaginary = (long)(long)Math.Sqrt(AreaState.Value / RatioState.Value);
		UpdateStates();
	}
    #endregion
}

public enum NumberQuadrant
{
	None = 0,
	TopRight = 1,
	TopLeft = 2,
	BottomRight = 4,
	BottomLeft = 8,
}
public static class NumberQuadrantExtensions
{
    public static int SlotValue(this NumberQuadrant kind, CertaintyLevel state)
    {
        return state.HasValue() ? (int)kind : 0;
    }
}
public enum MultiplicativeRelation
{
    None = 0,
    Real = 1,
    Ratio = 2, // Dot?
    Imaginary = 4,
    Area = 8, // Cross?
}
public static class MultiplicativeRelationExtensions
{
    public static int SlotValue(this MultiplicativeRelation kind, CertaintyLevel state)
    {
        return state.HasValue() ? (int)kind : 0;
    }
}
public enum CertaintyLevel
{
    Unknown,
	Uncertain,
	Derived,
	Set,
    Fixed,
}

public static class MeasureStateExtensions
{
    public static bool HasValue(this CertaintyLevel state) => state == CertaintyLevel.Fixed || state == CertaintyLevel.Set;
    public static int SlotValue(this CertaintyLevel state, MultiplicativeRelation kind)
    {
        return state.HasValue() ? (int)kind : 0;
    }
}
