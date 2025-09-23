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
    /// <summary>
    /// A number can be composed of multiple extents with different restrictions. 
    /// A simple number has a numerator and denominator, where a position in 3D space has 6 extents (3 numbers).
    /// An equation (eg a sinewave, bezier curve, color curve of heating) can be arbitrarily complex, 
    /// but they are all 'numbers' for lack of a better word.
    /// </summary>
    public Extent Numerator { get; protected set; }
    public Extent Denominator { get; protected set; }

    // this might be best to use floats as external facing values - it can stil use internal resolution and polarities,
    // but the outside world is floats (and ratios still need to be represented somehow).
    // That said, all training and inference should be on these types of numbers.

    /// <summary>
    /// UpValue is the positive polarity value, up vs down, right vs left, large vs small etc.
    /// We generally have opposites in a definition, and one of them is 'dominant', like getting bigger (up) vs getting smaller (down).
    /// </summary>
    public float UpValue => Numerator.EndValue / (float)Denominator.EndValue;
    /// <summary>
    /// DownValue is the negative polarity value, down vs up, left vs right, small vs large etc.
    /// We generally have opposites in a definition, and one of them is 'dominant', like getting bigger (up) vs getting smaller (down).
    /// </summary>
    public float DownValue => Numerator.StartValue / (float)Denominator.StartValue;

}

public abstract class NumberValue
{
    public NumberBase NumberReference { get; }
    public abstract long UpResolution { get; }
    public abstract long DownResolution { get; }

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
