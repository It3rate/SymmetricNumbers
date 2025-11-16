namespace SymNum.Scraps;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;
using SymNum.Scraps;

public class Relations : UnbalancedNumber
{
    // EXPERIMENTAL, not used, exploring mechanics of partially set equations, correct and incorrect.

    public long Real { get => RightMeasure.Position; private set => RightMeasure.SetPositionAndInvoke(value); }
    public long RealResolution { get => RightUnit.Position; private set => RightUnit.SetPositionAndInvoke(value); }
    public long Imaginary { get => LeftMeasure.Position; private set => LeftMeasure.SetPositionAndInvoke(value); }
    public long ImaginaryResolution { get => LeftUnit.Position; private set => LeftUnit.SetPositionAndInvoke(value); }

    public BinaryExpression RealState { get; private set; }
    public BinaryExpression ImaginaryState { get; private set; }
    public BinaryExpression AreaState { get; private set; }
    public BinaryExpression RatioState { get; private set; }
    protected double RealValue => Real / (double)RealResolution;
    protected double ImaginaryValue => Imaginary / (double)ImaginaryResolution;
    public double AreaValue => RealValue * ImaginaryValue;
    public double RatioValue => RealValue / ImaginaryValue;

    // the natural constraints of a UnbalancedNumber are r=rm/ru and i=im/iu.
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
        RealState.Certainty.SlotValue(MultiplicativeRelation.Real) +
        RatioState.Certainty.SlotValue(MultiplicativeRelation.Ratio) +
        ImaginaryState.Certainty.SlotValue(MultiplicativeRelation.Imaginary) +
        AreaState.Certainty.SlotValue(MultiplicativeRelation.Area);

    private void UpdateStates()
    {
    }
    public CertaintyLevel GetState(MultiplicativeRelation kind)
    {
        var result = CertaintyLevel.Uncertain;
        switch (kind)
        {
            case MultiplicativeRelation.Real:
                result = RealState.Certainty;
                break;
            case MultiplicativeRelation.Ratio:
                result = RatioState.Certainty;
                break;
            case MultiplicativeRelation.Imaginary:
                result = ImaginaryState.Certainty;
                break;
            case MultiplicativeRelation.Area:
                result = AreaState.Certainty;
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
                    result = (long)RealValue;
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
    protected void Calculate()
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
        //Imaginary = (long)(AreaState.Value / RealState.Value);
        UpdateStates();
    }
    private void SetWithIA()
    {
       // Real = (long)(AreaState.Value / ImaginaryState.Value);
        UpdateStates();
    }
    private void SetWithRRa()
    {
       // Imaginary = (long)((1.0 / RatioState.Value) * RealState.Value);
        UpdateStates();
    }
    private void SetWithIRa()
    {
       // Real = (long)((RatioState.Value) * ImaginaryState.Value);
        UpdateStates();
    }
    private void SetWithARa()
    {
        //Real = (long)Math.Sqrt(AreaState.Value * RatioState.Value);
        //Imaginary = (long)(long)Math.Sqrt(AreaState.Value / RatioState.Value);
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


