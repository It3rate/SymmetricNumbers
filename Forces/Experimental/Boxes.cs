using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Experimental.Boxes;

public record Tag
{
	private static int _nextID = 0;
	public int ID { get; } = Interlocked.Increment(ref _nextID);
}
/// <summary>
/// These hold space definitions of arbitrary dimension, and can be combined into expressions.
/// An area can hold other areas or lines, but lines in an area box must be defined with (xy start and xy end points).
/// Lines in a line space only need x start and x end points.
/// These boxes also constraint the limits of what can be inside them without error (a seg from 5 to 10 can't hold 12 without error).
/// </summary>
public class Box
{
	public Tag[] Tags { get; }
	private long? _value;
	// value counts map to Dims, and nuges from other dims act on all of these?
	public long[]? Value { get; }
	public int Dims => Value != null ? Value.Length : 0;
	public OpType Op { get; } = OpType.None;
	public int Repeats { get; } = 0;
	// find all places where this is contained by something, which will give it position etc in that frame
	public List<Box> Usages { get; } = new List<Box>();
	//public Box Parent { get; } // always child driven as this may exist in multiple parents.
	public List<Box> Children { get; } = new List<Box>();
	public Certainty Certainty { get; private set; } // value counts map to Dims, and nuges from other dims act on all of these?
	public Dictionary<Certainty, long[]> History { get; } = new Dictionary<Certainty, long[]>(); // history of resolution, one per tick

	//Box Parent { get;  } // link back to parent box
	public float Validity { get; } // how well the value fits in the box, if it exists, certainty of it etc.
	public Box(Certainty certainty, OpType optype, params Tag[] tags)
	{
		Certainty = certainty;
		Op = optype;
		Tags = tags;
	}
	public void AddChild(Box child)
	{
		Children.Add(child);
		child.AddParent(this);
	}
	private void AddParent(Box parent)
	{
		Usages.Add(parent);
	}
}

public class Certainty
{
	public static Certainty Zero = new Certainty(0);
	// confidence of source
	float Level { get; set; } // 0-1
	public Certainty(float level = 0)
	{
		Level = level;
	}
}

public class Test
{
	public static void Test1()
	{
		var t0 = new Tag();
		var t1 = new Tag();
		var t2 = new Tag();
		var t3 = new Tag();
		var t4 = new Tag();
		var t5 = new Tag();

		Box b = new Box(new Certainty(1f), OpType.Merge, t0, t1);
	}
}

public enum OpType
{

	Never = 0x0000,
	Both = 0x0001,
	OnlyA = 0x0010,
	AMaybeB = 0x0011,
	OnlyB = 0x0100,
	BMaybeA = 0x0101,
	OneOf = 0x0110,
	AtLeastOne = 0x0111,
	NoOne = 0x1000,
	BothOrNoOne = 0x1001,
	AOrNoOne = 0x1010,
	NotBAlone = 0x1011,
	BOrNoOne = 0x1100,
	NotAAlone = 0x1101,
	NotBoth = 0x1110,
	Always = 0x1111,

	None = 0x10000,
	Append = 0x10001,
	Merge = 0x10002,
}


//public class Expr : Box
//{
//	public Box Left { get; }
//	public Box? Right { get; }
//	public Op Op { get; }
//	public int Repeats { get; }
//	public Expr(int dims, Box left, Box? right, Op op, int repeats = 0) : base (dims)
//	{
//		Left = left;
//		Right = right;
//		Op = op;
//		Repeats = repeats;
//	}
//}