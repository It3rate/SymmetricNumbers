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

	// probably needs to be long, but multiplied by a resolution? Resolution is Min(Real, Imaginary)?
	// maybe this should be area/imaginary^2, or real^2/imaginary^2?
	// essentially the ratio has a known resolution, and it should be preserved. The value is literally w/h at that resolution, but that doesn't reduce to a single value.
    public MeasureState RealState { get; private set; }
    public MeasureState ImaginaryState { get; private set; }
    public MeasureState RealResolutionState { get; private set; }
    public MeasureState ImaginaryResolutionState { get; private set; }

    public long MinResolution => Math.Min(RealResolution, ImaginaryResolution);
    public long MaxResolution => Math.Max(RealResolution, ImaginaryResolution);
	public RationalNumber RealPart => new RationalNumber(RealResolution, Real);
	public RationalNumber ImaginaryPart => new RationalNumber(RealResolution, Real);
	protected double RealValue => Real / (double)RealResolution;
	protected double ImaginaryValue => Imaginary / (double)ImaginaryResolution;


	public override bool IsFullyDefined =>
        base.IsFullyDefined &&
        RealState == MeasureState.Set &&
        ImaginaryState == MeasureState.Set &&
        RealResolutionState == MeasureState.Set &&
        ImaginaryResolutionState == MeasureState.Set;
    public int SetSlotsValue =>
        RealState.SlotValue(NumberQuadrant.TopRight) +
        ImaginaryState.SlotValue(NumberQuadrant.BottomRight) +
        RealResolutionState.SlotValue(NumberQuadrant.BottomLeft) +
        ImaginaryResolutionState.SlotValue(NumberQuadrant.TopLeft);

    // long tr, long tl, long br, long bl)
    public SplitNumber(long? real, long? aspect, long? imaginary, long? area)
    {
        RightMeasure = new Measure(real ?? 0, Direction.FacesRight, true);
        LeftUnit = new Measure(aspect ?? 0, Direction.FacesLeft, true);
        RightUnit = new Measure(imaginary ?? 0, Direction.FacesRight, true);
        LeftMeasure = new Measure(area ?? 0, Direction.FacesLeft, true);

        RealState = real != null ? MeasureState.Set : MeasureState.Unknown;
        ImaginaryState = imaginary != null ? MeasureState.Set : MeasureState.Unknown;
        RealResolutionState = area != null ? MeasureState.Set : MeasureState.Unknown;
        ImaginaryResolutionState = aspect != null ? MeasureState.Set : MeasureState.Unknown;
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
	public RationalNumber AspectRatioReal => new RationalNumber(MinResolution, (long)((RealValue / ImaginaryValue) * MinResolution));
	public RationalNumber AspectRatioImaginary => new RationalNumber(MinResolution, (long)((ImaginaryValue / RealValue) * MinResolution));
	public RationalNumber RealRatio => new RationalNumber(Real, Imaginary);
	public RationalNumber ImaginaryRatio => new RationalNumber(Imaginary, Real);

	public MeasureState GetState(NumberQuadrant kind)
    {
        var result = MeasureState.Unknown;
        switch (kind)
        {
            case NumberQuadrant.TopRight:
                result = RealState;
                break;
            case NumberQuadrant.BottomRight:
                result = ImaginaryState;
                break;
            case NumberQuadrant.BottomLeft:
                result = RealResolutionState;
                break;
            case NumberQuadrant.TopLeft:
                result = ImaginaryResolutionState;
                break;
        }
        return result;
    }
    public long GetValue(NumberQuadrant kind)
    {
        long result = 0; // probably should be max or min based on direction?
        if (GetState(kind).HasValue())
        {
            switch (kind)
            {
                case NumberQuadrant.TopRight:
                    result = Real;
                    break;
                case NumberQuadrant.BottomRight:
                    result = RealResolution;
                    break;
                case NumberQuadrant.BottomLeft:
                    result = Imaginary;
                    break;
                case NumberQuadrant.TopLeft:
                    result = ImaginaryResolution;
                    break;
            }
        }
        return result;
    }
    public void FixValue(NumberQuadrant kind)
    {
        switch (kind)
        {
            case NumberQuadrant.TopRight: RealState = MeasureState.Fixed; break;
            case NumberQuadrant.BottomRight: RealResolutionState = MeasureState.Fixed; break;
            case NumberQuadrant.BottomLeft: ImaginaryState = MeasureState.Fixed; break;
            case NumberQuadrant.TopLeft: ImaginaryResolutionState = MeasureState.Fixed; break;
        }
    }
    public void SetValue(NumberQuadrant kind, long value)
    {
        switch (kind)
        {
            case NumberQuadrant.TopRight:
                Real = value;
                RealState = MeasureState.Set;
                break;
            case NumberQuadrant.BottomRight:
                RealResolution = value;
                RealResolutionState = MeasureState.Set;
                break;
            case NumberQuadrant.BottomLeft:
                Imaginary = value;
                ImaginaryState = MeasureState.Set;
                break;
            case NumberQuadrant.TopLeft:
                ImaginaryResolution = value;
                ImaginaryResolutionState = MeasureState.Set;
                break;
        }
    }
    public void SetAndFix(NumberQuadrant toSet, long value, NumberQuadrant toFix)
    {
        UnfixAll();
        FixValue(toFix);
        SetValue(toSet, value);
        Calculate();
    }
    public void SetTwo(NumberQuadrant setA, long valueA, NumberQuadrant setB, long valueB)
    {
        UnfixAll();
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
        //Aspect = 8,
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
            UnfixAll();
        }
    }

    public void UnfixAll()
    {
        RealState = MeasureState.Set;
        ImaginaryState = MeasureState.Set;
        RealResolutionState = MeasureState.Set;
        ImaginaryResolutionState = MeasureState.Set;
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
}

public enum NumberQuadrant
{
    None = 0,
    TopRight = 1,
    BottomRight = 2,
    BottomLeft = 4,
    TopLeft = 8,
}
public static class NumberQuadrantExtensions
{
    public static int SlotValue(this NumberQuadrant kind, MeasureState state)
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
    public static int SlotValue(this MeasureState state, NumberQuadrant kind)
    {
        return state.HasValue() ? (int)kind : 0;
    }
}