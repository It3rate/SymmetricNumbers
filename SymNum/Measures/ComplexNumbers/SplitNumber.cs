namespace SymNum.Measures.PatternNumbers;

using System;
using SymNum.Patterns;


// May need to rethink this. The aspect is a ratio, but the only valid reolution of the ratio is literally 
// this real in ticks over this imaginary in ticks.
// What are natural imaginary values here? Maybe w*h and w-h? 
// ** Also, there needs to be interoporability between measures, Numbers, and Number equations.
// Changing now, will delete after checkin.

/// <summary>
/// A SplitNumber is a complex number where the two units do no need to be the same resolution.
/// They still need to point in opposite directions, and the right side is the real part.
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

        var rstate = (real != null && realResolution != null) ? MeasureState.Set : MeasureState.Unknown;
        RealState = new ValueSample(Real, RealResolution, BinaryOp.Divide, rstate);
        var iState = (imaginary != null && imaginaryResolution != null) ? MeasureState.Set : MeasureState.Unknown;
		ImaginaryState = new ValueSample(Imaginary, ImaginaryResolution, BinaryOp.Divide, rstate);
		RatioState = new ValueSample(RealValue, ImaginaryValue, BinaryOp.Divide);
		AreaState = new ValueSample(RealValue, ImaginaryValue, BinaryOp.Multiply);
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


	#region Fix and Transform
    // this is going to be its own class, where you can assert relations in a number
    // and then lock the number into certain behaviours and patterns with them.
	public override bool IsFullyDefined =>
		base.IsFullyDefined &&
		RealState.IsSet && ImaginaryState.IsSet && AreaState.IsSet && RatioState.IsSet;
	public int SetSlotsValue =>
		RealState.State.SlotValue(MultiplicativeRelation.Real) +
		RealState.State.SlotValue(MultiplicativeRelation.Ratio) +
		ImaginaryState.State.SlotValue(MultiplicativeRelation.Imaginary) +
		ImaginaryState.State.SlotValue(MultiplicativeRelation.Area);

	public ValueSample RealState { get; private set; }
	public ValueSample ImaginaryState { get; private set; }
	public ValueSample AreaState { get; private set; }
	public ValueSample RatioState { get; private set; }
	protected double RealValue => Real / (double)RealResolution;
	protected double ImaginaryValue => Imaginary / (double)ImaginaryResolution;
	public double AreaValue => RealValue * ImaginaryValue;
	public double RatioValue => RealValue / ImaginaryValue;

	public MeasureState GetState(MultiplicativeRelation kind)
    {
        var result = MeasureState.Unknown;
        switch (kind)
        {
            case MultiplicativeRelation.Real:
                result = RealState.State;
                break;
            case MultiplicativeRelation.Ratio:
                result = RatioState.State;
                break;
            case MultiplicativeRelation.Imaginary:
                result = ImaginaryState.State;
                break;
            case MultiplicativeRelation.Area:
                result = AreaState.State;
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
        SetAll();
        FixValue(toFix);
        SetValue(toSet, value);
        Calculate();
    }
    public void SetTwo(MultiplicativeRelation setA, long valueA, MultiplicativeRelation setB, long valueB)
    {
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
            //case 3: // VI
            //    SetWithVI();
            //    break;
            //case 5: // VP
            //    SetWithVP();
            //    break;
            //case 6: // IP
            //    SetWithIP();
            //    break;
            //case 7: // VIP
            //    SetWithVIP();
            //    break;
            //case 8: // VR
            //    SetWithVR();
            //    break;
            //case 10: // IR
            //    SetWithIR();
            //    break;
            //case 11: // VIR
            //    SetWithVIR();
            //    break;
            //case 12: // PR
            //    SetWithPR();
            //    break;
            //case 13: // VPR
            //    SetWithVPR();
            //    break;
            //case 14: // IPR
            //    SetWithIPR();
            //    break;
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
    private void SetWithP()
    {
    }

	//private void SetWithVIR()
	//{
	//    Area = Real * Imaginary;
	//}
	//private void SetWithIPR()
	//{
	//    Real = Imaginary * Aspect;
	//}
	//private void SetWithVPR()
	//{
	//    Imaginary = (long)(Real / (double)Aspect);
	//}
	//private void SetWithVIP()
	//{
	//    Area = Real * Imaginary;
	//}
	//private void SetWithVI()
	//{
	//    Aspect = (long)(Real / (float)Imaginary);
	//    Area = Real * Imaginary;
	//}
	//private void SetWithIR()
	//{
	//    Real = Imaginary * Aspect;
	//    Area = Imaginary * Imaginary * Aspect;
	//}
	//private void SetWithIP()
	//{
	//    Real = (long)(Area / (double)Imaginary);
	//    Aspect = (long)(Area / (double)(Imaginary * Imaginary));
	//}
	//private void SetWithVR()
	//{
	//    Imaginary = (long)(Real / (double)Aspect);
	//    Area = Real * Imaginary;
	//}
	//private void SetWithVP()
	//{
	//    Imaginary = (long)(Area / (double)Real);
	//    Aspect = (long)(Area / ((double)Real * Real));
	//}
	//private void SetWithPR()
	//{
	//    Imaginary = (long)(Math.Sqrt(Area / (double)Aspect));
	//    Real = (long)(Math.Sqrt(Area * Aspect));
	//}
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
    public static int SlotValue(this NumberQuadrant kind, MeasureState state)
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
    public static int SlotValue(this MultiplicativeRelation kind, MeasureState state)
    {
        return state.HasValue() ? (int)kind : 0;
    }
}
public enum MeasureState
{
    Unknown,
    Uncertain,
    Set,
    Fixed,
}

public static class MeasureStateExtensions
{
    public static bool HasValue(this MeasureState state) => state == MeasureState.Fixed || state == MeasureState.Set;
    public static int SlotValue(this MeasureState state, MultiplicativeRelation kind)
    {
        return state.HasValue() ? (int)kind : 0;
    }
}
public enum BinaryOp
{
    None,
    Multiply,
    Divide,
    Add,
    Subtract,
	Pow,
	Mod,
    // sin cos Math.etc
}

public class ValueSample
{
    public double First { get; }
    public double Second { get; }
    public double Value { get; set; }
	public MeasureState State { get; set; }
	public BinaryOp Op { get; set; }
    public bool IsSet => State.HasValue();
	public ValueSample(double first, double second, BinaryOp op, MeasureState state = MeasureState.Unknown)
	{
		First = first;
		Second = second;
        Op = op;
		State = state;
		Calculate();
	}
    public void Fix() => State = MeasureState.Fixed;
    public void Set() => State = MeasureState.Set;
    public void Unset() => State = MeasureState.Uncertain;
    private void Calculate()
    {
        switch (Op)
        {
            case BinaryOp.Multiply:
                Value = First * Second;
                break;
            case BinaryOp.Divide:
                Value = Second == 0 ? 0 : First / (double)Second;
                break;
            case BinaryOp.Add:
                Value = First + Second;
                break;
            case BinaryOp.Subtract:
                Value = First - Second;
                break;
            case BinaryOp.Pow:
                Value = Math.Pow(First, Second);
                break;
            case BinaryOp.Mod:
                Value = First % Second;
                break;
        }
    }
}