using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Pairs;

/// <summary>
/// A pair is two points that are aware of each other. Internally they know their length, and they can start to end, or end to start.
/// They don't internally know their direction, as that is calibrated relative to an external + or -, depending where they are.
/// </summary>
public class Pair : IPair<Measure>
{
	public Measure A { get; }
	public Measure B { get; }
	public Measure Length => new Measure(BPos - APos);
	public long APos => A.Position;
	public long BPos => B.Position;
	public (long, long) Positions => (A.Position, B.Position);


	//public int Direction => (APos > BPos) ? 1 : (APos < BPos) ? -1 : 0;
	//public long Midpoint => (BPos + APos) / 2;
	//public float Ratio => APos / (float)BPos;
	//public float Reciprocal => BPos / (float)APos;
	public Pair(Measure a, Measure b)
	{
		A = a;
		B = b;
	}
	public void Invert()
	{
		var atemp = A.Position;
		A.SetPosition(B.Position);
		B.SetPosition(atemp);
	}
}
