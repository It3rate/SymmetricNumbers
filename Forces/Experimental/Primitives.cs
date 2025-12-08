using SpiceSharpParser.Lexers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Experimental;

public interface ILevel0<TSelf, T> // point with no internal information
	where TSelf : ILevel0<TSelf, T>
{
	int[] Type {get;} // [0]->scalar, [0,0]scalar expression [0,A]->scaled unit [A,A]->acceleration [A,B]->area [A,B,C]->volume
	T Zero { get; }
	TSelf Self { get; }
}
public interface ILevel1<TSelf, T> : ILevel0<TSelf, T>// A zero based segment made from a point with a zero relation
	where TSelf : ILevel1<TSelf, T>
{
	T Length { get; }
	// can invert
	delegate T UnaryTransform(T a); // transforms probably have a count based on the unit count, rather than unary and binary?
	record UnaryTransformRecord(TSelf self, UnaryTransform Transform);
	T ApplyTransform(UnaryTransformRecord record, int count = 1); // A transform is always a loss, as it combines two inputs into one
	(TSelf, TSelf) Decompose(T combo);// this needs to also handle repeats, that is probably on the unit. This clarifies the loss when compared with original input.
}
public interface ILevel2<TSelf, T> : ILevel1<TSelf, T>// A ratio or area, formed by a segment with another segment relation
	where TSelf : ILevel2<TSelf, T>
{
	T Numerator { get; }
	// attached at zeros (otherwise is just Level1 repeat, which is addition and subtraction)
	// can be orthogonal area (b*a), or parallel ratio (b/a)
	delegate T BinaryTransform(T a);
	record BinaryTransformRecord(TSelf self, BinaryTransform Transform);
	T ApplyBinaryTransform(BinaryTransformRecord record, int count = 1);
}
class L0 
{
	public long Position { get; protected set; } // random position
	public L0 Self => this;
}
class L1 : ILevel1<L1, L0>
{
	private class L0_1 : L0
	{
		public L0_1(long position) {  Position = position; }
		public void SetPosition(long position) { Position = position; }
	}
	public L0 Zero { get; }
	public L0 Length => _length;

	private L0_1 _length;
	public long LengthValue
	{
		get => _length.Position - Zero.Position;
		protected set => new L0_1(value + Zero.Position);
	}
	public L1 Self => this;
	public L1(L0 zero, long position)
	{
		Zero = zero;
		LengthValue = position;
	}

	public L0 ApplyTransform(ILevel1<L1, L0>.UnaryTransformRecord record, int count = 1)
	{
		throw new NotImplementedException();
	}
}
class L2 : ILevel2<L2, L1>
{
	public L1 Numerator => throw new NotImplementedException();

	public L1 Length => throw new NotImplementedException();

	public L1 Zero => throw new NotImplementedException();

	public L2 Self => throw new NotImplementedException();

	public L1 ApplyBinaryTransform(ILevel2<L2, L1>.BinaryTransformRecord record, int count = 1)
	{
		throw new NotImplementedException();
	}

	public L1 ApplyTransform(ILevel1<L2, L1>.UnaryTransformRecord record, int count = 1)
	{
		throw new NotImplementedException();
	}
}
public interface ILevel<TSelf, T>
	where TSelf : ILevel<TSelf, T>
{
	delegate T UnaryTransform(T a);
	delegate T BinaryTransform(T b, T a);
	record TransformRecord(TSelf self, BinaryTransform Transform);
	T A { get; }
	T B { get; }
	TSelf Self { get; }
	T ApplyTransform(TransformRecord record);
	T ApplyTransform(TransformRecord record, int count);
}
public interface IInvertable<TSelf, T> : ILevel<TSelf, T>
	where TSelf : IInvertable<TSelf, T>
{
	TSelf Inverse { get; }
}
public interface IDiffable<TSelf, T> : IInvertable<TSelf, T>
	where TSelf : IDiffable<TSelf, T>
{
	T Subtract(T b, T a); 
	TransformRecord Diff => new(Self, Subtract);// positive
	TransformRecord InvDiff => new(Inverse, Subtract);// negative
	T DiffCalc => Subtract(B, A);
}

public interface IDivideable<TSelf, T> : IDiffable<TSelf, T>
	where TSelf : IDivideable<TSelf, T>
{
	T Divide(T b, T a);
	TransformRecord Ratio => new(Self, Divide);
	TransformRecord InvRatio => new(Inverse, Divide);// reciprocal (inv divide, or like multiply)
	T RatioCalc => Divide(B, A);

	// Orthogonal, Area, Aspect
}


// Orthogonal: ISubtractable






public interface Level0 : IInvertable<Level0, Level0>
{
}
public interface Level1 : IDiffable<Level1, Level0>
{
	//public delegate Level0 Transform(Level0 b, Level0 a);
	//Level0 A { get; }
	//Level0 B { get; }
	//TransformRecord Diff => new(B, A, Subtract);
	//Level0 DiffCalc => Subtract(B, A);
	//Level1 Inverse();
	//Level0 Subtract(Level0 b, Level0 a);
}
public interface Level1Series : Level1
{
	//Level0 A => Landmarks[0];
	//Level0 B => Landmarks[Count - 1];
	//uint Diff { get; } // (B-A)+(C-B)+(D-C)... // or consolidate to start and end
	Level0[] Landmarks { get; } // [A, B, extra]
	uint Count => (uint)Landmarks.Length; // number of segments
	void Append(Level0 value);
}
public interface Level2 : IDivideable<Level2, Level1>
{
	//Level1 A { get; }
	//Level1 B { get; }
	//Level2 Reverse();

	//Level1 Subtract(Level1 b, Level1 a);
	//TransformRecord Diff => new(B, A, Subtract);
	//Level1 DiffCalc => Subtract(B, A);

	//Level1 Divide(Level1 b, Level1 a);
	//TransformRecord Ratio => new(B, A, Divide);
	//Level1 RatioCalc => Divide(B, A);
}
