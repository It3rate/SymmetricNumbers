using SpiceSharpParser.Lexers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Experimental;

public interface Level<TSelf, T>
	where TSelf : Level<TSelf, T>
{
	delegate T Transform(T b, T a);
	record TransformRecord(TSelf self, Transform Transform);
	T A { get; }
	T B { get; }
	TSelf Self { get; }
}
public interface IInvertable<TSelf, T> : Level<TSelf, T>
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
	//Level1 Reverse();
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
