namespace SymNum.Patterns;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A number out of three samples. This may not need to be a separate class, 
/// or at least other number classes should be able to use it.
/// </summary>
public class RatioNumber : NumberBase
{
	public long StartPosition => LeftMeasure.Position;
	public long MidPosition => RightMeasure.Position;
	public long EndPosition => LeftUnit.Position;
	public float FloatValue => (MidPosition - StartPosition) / (float)(EndPosition - StartPosition);
	public RatioNumber(long startPoint, long midPoint, long endPoint, bool isPositiveRight)
	{
		RightMeasure = new Measure(midPoint, Direction.Unknown, true);
        LeftMeasure = new Measure(endPoint, Direction.Unknown, true);
        LeftUnit = new Measure(startPoint, Direction.Unknown, true);
    }
}
public class RatioValue : NumberValue
{
    public RatioNumber Reference { get; }
    public float Value => (MidPosition - StartPosition) / (float)(EndPosition - StartPosition);
    public override long LeftResolution => Math.Abs(EndPosition - StartPosition);
    public override long RightResolution => LeftResolution;
    private long StartPosition => Reference.StartPosition;
    private long MidPosition => Reference.MidPosition;
    private long EndPosition => Reference.EndPosition;
    public RatioValue(RatioNumber baseNumber) : base(baseNumber)
    {
        Reference = baseNumber;
    }
}
