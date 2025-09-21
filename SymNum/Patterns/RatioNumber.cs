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
	public long StartPosition => Denominator.StartValue;
	public long MidPosition => Numerator.EndValue;
	public long EndPosition => Denominator.EndValue;
	public float FloatValue => (MidPosition - StartPosition) / (float)(EndPosition - StartPosition);
	public RatioNumber(long startPoint, long midPoint, long endPoint, bool isPositiveRight)
	{
		Numerator = new Extent(isPositiveRight, midPoint);
		Denominator = new Extent(startPoint, endPoint, isPositiveRight);
	}
}
