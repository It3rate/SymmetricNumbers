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
    public PartialExtent PartialNumerator { get; private set; }
	public long StartPosition => Denominator.StartValue;
	public long MidPosition => Numerator.EndValue;
	public long EndPosition => Denominator.EndValue;
	public float FloatValue => (MidPosition - StartPosition) / (float)(EndPosition - StartPosition);
	public RatioNumber(long startPoint, long midPoint, long endPoint, bool isPositiveRight)
	{
		PartialNumerator = new PartialExtent(midPoint, false);
		Denominator = new Extent(startPoint, endPoint);
	}
}
public class RatioValue : NumberValue
{
    public RatioNumber Reference { get; }
    public float Value => (MidPosition - StartPosition) / (float)(EndPosition - StartPosition);
    public override long UpResolution => Math.Abs(Reference.Denominator.EndValue - Reference.Denominator.StartValue);
    public override long DownResolution => UpResolution;
    private long StartPosition => Reference.Denominator.StartValue;
    private long MidPosition => Reference.PartialNumerator.EndElseStartValue;
    private long EndPosition => Reference.Denominator.EndValue;
    public RatioValue(RatioNumber baseNumber) : base(baseNumber)
    {
        Reference = baseNumber;
    }
}
