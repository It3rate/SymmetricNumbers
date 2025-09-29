using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Concepts;

namespace SymNum.Patterns;



// A number is made of at least 4 points, two for a unit (start and end), and two for a value (start and end).
// There is often an implied 5th point, 'here', or 'now', or 'this'. This is the floating zero point, or reference point.
// In regular numbers, 'here' is always 0, and both segments also start from 0.
//   The unit is arbitrary length (depending on resolution), but defined as 1.
//   So it seems there is one value, but there are 4 values (and the viewer's persective).
// In complex numbers, there is a unit for each direction.
//   The first segment goes from 'bad' (-1 position) to 'not bad' (postive direction value).
//   The second segment goes from 'good' (+1 position) to 'not good' (negative direction value).
//   The viewer's perspective is still 0, but there are both a postive direction unit and a negative direction unit.
//   The units are usually the same length (thus resolution), though perhaps they don't have to be.
//   All things must be defines relative to something else, and in the pure form two oppostive segments are defined relative to each other.
// In symmetric numbers, there are no units, everything is relative to each other.
//   The units slots are indicate what is happening on the polarity and direction.
//   Values can be positive or negative, and mirrored or not mirrored.
//   If you calculate the implications of these transformations,
//   you will get all symmetries, and contraditions the break into higher dimensions.

// A number's resolution can be a threshold or an average. This matters a lot when considering the meaning of 0 in a fraction.
// If 2 means an unknown between 1.5 and 2.5, then 2*2 is between 2.25 and 6.25
// If 2 means 2.0 or more, but less than 3.0, then 2*2 is between 4.0 and 9.0
// With averages, 3 / 0 (where zero has a value but it's below your resolution), 0 is between -0.5 and 0.5, so 3 / 0 is greater than 6, or less than -6.
// This is how you can get inverted ranges, a not operation (the whole numberline except some section)
// Inverting the average interpretation (0/3) gets the inverse, a range between -6 and 6.
// With thresholds you can get one sided ranges. If 0 is greater than zero and less than 1, then 3/0 is just greater than 3.
// The issue with thresholds is it requires a +0 and -0. There are two edges between -1 and 1, but only one node.
// This implies than resolution for numbers has to specify how it obtains it's resolution if you want to scale those values, which makes sense.

/// <summary>
/// There are many types of numbers, they just mean measures calibrated in some way. 
/// This is the base class which can grow into numbers, complex numbers, symmetric numbers, dynamic zero point numbers,
/// equations, patterns etc. All of these are just ways of organizing and calibrating values to have meaning 
/// (aka be non random).
/// Note that these can describe any non random pattern, but have no information about what the pattern represents.
/// This requires some kind of unit tied to the number. Numbers themselves are pure patterns.
/// </summary>
public class NumberBase
{
    //   public Extent RightExtent { get; protected set; }
    //   public Extent LeftExtent { get; protected set; }
    //   public long RightMeasure 
    //{ 
    //	get => RightExtent.StartPosition;
    //	set => RightExtent.StartPosition = value; 
    //}
    //public long RightUnit
    //{
    //	get => RightExtent.EndPosition;
    //	set => RightExtent.EndPosition = value;
    //}
    //public long LeftMeasure
    //{
    //	get => LeftExtent.EndPosition;
    //	set => LeftExtent.EndPosition = value;
    //}
    //public long LeftUnit
    //{
    //	get => LeftExtent.StartPosition;
    //	set => LeftExtent.StartPosition = value;
    //

    /// A number can be composed of multiple measures with different restrictions. 
    /// A simple number has a numerator and denominator, where a position in 3D space has 12 measures (6 extents, 3 numbers).
    /// An equation (eg a sinewave, bezier curve, color curve of heating) can be arbitrarily complex, 
    /// but they are all 'numbers' for lack of a better word. Values are always constructed, even in the number 9 (which is 9 * unit).

    private Measure[] _measures;
    public Measure RightMeasure { get => _measures[0]; protected set => _measures[0] = value; }
    public Measure RightUnit { get => _measures[1]; protected set => _measures[1] = value; }
    public Measure LeftMeasure { get => _measures[2]; protected set => _measures[2] = value; }
    public Measure LeftUnit { get => _measures[3]; protected set => _measures[3] = value; }


    public virtual bool IsFullyDefined => _measures.All(m => m.IsFullDefined);

    // this might be best to use floats as external facing values - it can stil use internal resolution and polarities,
    // but the outside world is floats (and ratios still need to be represented somehow).
    // That said, all training and inference should be on these types of numbers.

    public static float GetValue(Measure numerator, Measure denominator)
    {
        var result = float.NaN;
        if (numerator.IsFullDefined && denominator.IsFullDefined && denominator.Position != 0)
        {
            result = numerator.Position / (float)denominator.Position;
        }
        return result;
    }
    public static bool SetValue(Measure numerator, Measure denominator, float value)
    {
        var result = false;
        if (numerator.IsFullDefined && denominator.IsFullDefined && denominator.Position != 0)
        {
            denominator.SetPosition((long)(value * denominator.Position));
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
    public NumberBase NumberReference { get; }
    public abstract long RightResolution { get; }
    public abstract long LeftResolution { get; }

    public NumberValue(NumberBase number)
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
