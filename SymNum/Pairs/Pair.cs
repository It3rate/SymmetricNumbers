using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Span;

/// <summary>
/// A Pair is two reference measures evaluated in a specific polarity context.
/// </summary>
public class Pair
{
	public IRefMeasure A { get; }
	public IRefMeasure B { get; }
	public Polarity Polarity { get; }

	public long APos => A.PositionAs(Polarity);
	public long BPos => B.PositionAs(Polarity);
	public (long, long) Positions => (APos, BPos);
	public int Direction(Polarity polarity) => APos > BPos ? 1 : APos < BPos ? -1 : 0;

	public long Length => BPos - APos; 
	public long Midpoint => Length / 2;
	public float Ratio => APos / BPos;
	public float Reciprocal => BPos / APos;

	public Pair(IRefMeasure a, IRefMeasure b, Polarity asPolarity)
	{
		A = a;
		B = b;
		Polarity = asPolarity;
	}

}
