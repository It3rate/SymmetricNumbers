namespace SymNum.Measures.PatternNumbers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Patterns;

public class AreaAspectNumber : UnconstrainedNumber
{
    public long Width { get => RightMeasure.Position; private set => RightMeasure.SetPosition(value); } // these are wrong, need to recalculate others. 
    public long Height { get => RightUnit.Position; private set => RightUnit.SetPosition(value); }
    // probably needs to be long, but multiplied by a resolution? Resolution is Min(Width, Height)?
    // maybe this should be area/height^2, or width^2/height^2?
    // essentially the ratio has a known resolution, and it should be preserved. The value is literally w/h at that resolution, but that doesn't reduce to a single value.
    public long Aspect { get => (long)(Width / (double)Height); private set => RightMeasure.SetPosition(value); } 
    public long Area { get => Width * Height; private set => RightMeasure.SetPosition(value); }
    public MeasureState WidthState { get; private set; }
    public MeasureState HeightState { get; private set; }
    public MeasureState AspectState { get; private set; }
    public MeasureState AreaState { get; private set; }

    public override bool IsFullyDefined =>
        base.IsFullyDefined &&
        WidthState == MeasureState.Set &&
        HeightState == MeasureState.Set &&
        AspectState == MeasureState.Set &&
        AreaState == MeasureState.Set;
    public int SetSlotsValue =>
        WidthState.SlotValue(NumberQuadrant.TopRight) +
        HeightState.SlotValue(NumberQuadrant.BottomRight) +
        AspectState.SlotValue(NumberQuadrant.BottomLeft) +
        AreaState.SlotValue(NumberQuadrant.TopLeft);

    // long tr, long tl, long br, long bl)
    public AreaAspectNumber(long? width, long? aspect, long? height, long? area)
    {
        RightMeasure = new Measure(width ?? 0, Direction.FacesRight, true);
        LeftUnit = new Measure(aspect ?? 0, Direction.FacesLeft, true);
        RightUnit = new Measure(height ?? 0, Direction.FacesRight, true);
        LeftMeasure = new Measure(area ?? 0, Direction.FacesLeft, true);

        WidthState = width != null ? MeasureState.Set : MeasureState.Unknown;
        HeightState = height != null ? MeasureState.Set : MeasureState.Unknown;
        AspectState = area != null ? MeasureState.Set : MeasureState.Unknown;
        AreaState = aspect != null ? MeasureState.Set : MeasureState.Unknown;
        Calculate();
    }
    public MeasureState GetState(NumberQuadrant kind)
    {
        var result = MeasureState.Unknown;
        switch (kind)
        {
            case NumberQuadrant.TopRight:
                result = WidthState;
                break;
            case NumberQuadrant.BottomRight:
                result = HeightState;
                break;
            case NumberQuadrant.BottomLeft:
                result = AspectState;
                break;
            case NumberQuadrant.TopLeft:
                result = AreaState;
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
                    result = Width;
                    break;
                case NumberQuadrant.BottomRight:
                    result = Height;
                    break;
                case NumberQuadrant.BottomLeft:
                    result = Area;
                    break;
                case NumberQuadrant.TopLeft:
                    result = Aspect;
                    break;
            }
        }
        return result;
    }
    public void FixValue(NumberQuadrant kind)
    {
        switch (kind)
        {
            case NumberQuadrant.TopRight: WidthState = MeasureState.Fixed; break;
            case NumberQuadrant.BottomRight: HeightState = MeasureState.Fixed; break;
            case NumberQuadrant.BottomLeft: AspectState = MeasureState.Fixed; break;
            case NumberQuadrant.TopLeft: AreaState = MeasureState.Fixed; break;
        }
    }
    public void SetValue(NumberQuadrant kind, long value)
    {
        switch (kind)
        {
            case NumberQuadrant.TopRight:
                Width = value;
                WidthState = MeasureState.Set;
                break;
            case NumberQuadrant.BottomRight:
                Height = value;
                HeightState = MeasureState.Set;
                break;
            case NumberQuadrant.BottomLeft:
                Area = value;
                AspectState = MeasureState.Set;
                break;
            case NumberQuadrant.TopLeft:
                Aspect = value;
                AreaState = MeasureState.Set;
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
        //Width = 1,
        //Height = 2,
        //Area = 4,
        //Aspect = 8,
        switch (ssv)
        {
            case 3: // VI
                SetWithVI();
                break;
            case 5: // VP
                SetWithVP();
                break;
            case 6: // IP
                SetWithIP();
                break;
            case 7: // VIP
                SetWithVIP();
                break;
            case 8: // VR
                SetWithVR();
                break;
            case 10: // IR
                SetWithIR();
                break;
            case 11: // VIR
                SetWithVIR();
                break;
            case 12: // PR
                SetWithPR();
                break;
            case 13: // VPR
                SetWithVPR();
                break;
            case 14: // IPR
                SetWithIPR();
                break;
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
        WidthState = MeasureState.Set;
        HeightState = MeasureState.Set;
        AspectState = MeasureState.Set;
        AreaState = MeasureState.Set;
    }

    /// <summary>
    /// If you set Area, then Height and Width can be anything that multiplies to the Area, and Aspect is the raito.
    /// This makes them uncertain, but still constrained, and the value becomes at best a probability based on history.
    /// It also allows for sampling. You can sample different currents based on a known Area and see how it fits when fully calculated.
    /// This is true of any gradients in this pattern, change the area of a rect, and you are free to sample any widths, lengths, or aspect ratios to test.
    /// This is sort of the concept separated from the instance, which is the key to encoding patterns.
    /// </summary>
    private void SetWithP()
    {
    }

    private void SetWithVIR()
    {
        Area = Width * Height;
    }
    private void SetWithIPR()
    {
        Width = Height * Aspect;
    }
    private void SetWithVPR()
    {
        Height = (long)(Width / (double)Aspect);
    }
    private void SetWithVIP()
    {
        Area = Width * Height;
    }
    private void SetWithVI()
    {
        Aspect = (long)(Width / (float)Height);
        Area = Width * Height;
    }
    private void SetWithIR()
    {
        Width = Height * Aspect;
        Area = Height * Height * Aspect;
    }
    private void SetWithIP()
    {
        Width = (long)(Area / (double)Height);
        Aspect = (long)(Area / (double)(Height * Height));
    }
    private void SetWithVR()
    {
        Height = (long)(Width / (double)Aspect);
        Area = Width * Height;
    }
    private void SetWithVP()
    {
        Height = (long)(Area / (double)Width);
        Aspect = (long)(Area / ((double)Width * Width));
    }
    private void SetWithPR()
    {
        Height = (long)(Math.Sqrt(Area / (double)Aspect));
        Width = (long)(Math.Sqrt(Area * Aspect));
    }
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