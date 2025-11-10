namespace SymNum.Scraps;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Numbers;
using SymNum.Primitives;

/// <summary>
/// A number out of three samples. This may not need to be a separate class, 
/// or at least other number classes should be able to use it.
/// </summary>
public class RatioNumber : RawNumber
{
    public long StartPosition => LeftMeasure.Position;
    public long MidPosition => RightMeasure.Position;
    public long EndPosition => LeftUnit.Position;
    public float FloatValue => (MidPosition - StartPosition) / (float)(EndPosition - StartPosition);
    public RatioNumber(long startPoint, long midPoint, long endPoint, bool isPositiveRight)
    {
        RightMeasure = new Measure(midPoint);
        LeftMeasure = new Measure(endPoint);
        LeftUnit = new Measure(startPoint);
    }
}
