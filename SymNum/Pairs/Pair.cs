using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Pairs;

/// <summary>
/// A Pair is two reference measures evaluated directly, disregarding their polarity or and value adjustments within a Number.
/// This can be used as Numerator/Denominator pairs for part of numbers with the same polarity, but is more for use 'pre-numbers'.
/// </summary>
public class Pair
{
	public IMeasure A { get; }
	public IMeasure B { get; }
	public long APos => A.Position;
	public long BPos => B.Position;
	public (long, long) Positions => (A.Position, B.Position);
	public int Direction => (APos > BPos) ? 1 : (APos < BPos) ? -1 : 0;
	public long Length => BPos - APos;
	public long Midpoint => (BPos + APos) / 2;
	public float Ratio => APos / (float)BPos;
	public float Reciprocal => BPos / (float)APos;
	public Pair(IMeasure a, IMeasure b)
	{
		A = a;
		B = b;
	}
}
