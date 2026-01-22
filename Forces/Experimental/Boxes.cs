using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Experimental.Boxes;

/// <summary>
/// These hold space definitions of arbitrary dimension, and can be combined into expressions.
/// An area can hold other areas or lines, but lines in an area box must be defined with (xy start and xy end points).
/// Lines in a line space only need x start and x end points.
/// These boxes also constraint the limits of what can be inside them without error (a seg from 5 to 10 can't hold 12 without error).
/// </summary>
public interface Box
{
	int Dims { get; }
	List<Expr> Children { get; }
	long[] Value { get; } // value counts map to Dims, and nuges from other dims act on all of these?
	Dictionary<Certainty, long[]> Resolvers { get; } // external forces acting on this expression
	Dictionary<Certainty, long[]> History { get; } // history of resolution, one per tick

	Box Parent { get;  } // link back to parent box
	float Validity { get;  } // how well the value fits in the box, if it exists, certainty of it etc.
}
public interface Expr : Box
{
	Box Left { get; }
	Box Right { get; }
	Op Op { get; }
	int Repeats { get; }
}

public class Certainty
{
	// confidence of source
	float Level { get; set; } // 0-1
}