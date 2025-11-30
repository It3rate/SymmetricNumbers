using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Expressions;
namespace SymNum.Primitives;

// Requirement: Groupings of 'IMeasure' need to be able to smoothly move between bools, enums, ints, floats, complex nums, symmetric numbs, equations, and probabilities.
//    There are a lot of types on numbers amidst these, but that is the general trend.
//    All of these can be represented by four Measures, a zero, and reptition (and primitive ops)

// Q: Should there be a type with two measures (a focal/span?). There are ratios, but those are numbers.
// there is a symmetry with this, 1:measure, 2:span, 4:number, 8:expression, 16:plane
// some critical equations happen on two measures, such as n/d, midpoint, length

// One consideration is there are two ways to convert measure(s) to ratios.
// First is to split the measure with a zero point that isn't on the end point. This creates a positive and negative side,
// or a positive polarity and negative polarity. This is more about interpolation, comparisions and bool ops than ratios.
// The second way is to have two measures and combine them with an operation. This gives a numerator and denominator, or 
// a subtracted difference, etc. This is more about the relations of two numbers in a result than a simple interpolation.
// The splitting is natural within the number, the combining of measures is driven externally.
// It seems this internal/external expansion is true all the way up the scales, and seems significant.
// Maybe this is like bool ops vs math ops.  Bool ops split a segment into parts, math ops combine two segments together.
// The bool ops modify something in it's existing level, math ops create elements in a higher layer.
//  bool ops can create + and - sections, but only if using the true and false results:
//       a segment from 3->2 'and' >7 results in 7->12 (the second segment is the 'not', 3->7)

/// <summary>
/// This is a measure not accessed from a number, 
/// so it doesn't have access to things like certainty, scale, bias, min/max values, etc.
/// Essentially it is used as a first level input and isn't manipulated directly.
/// Ultimately types come from primary inputs, like distance/time -> speed. 
/// Km or miles is an equation, speed is the primarires.
/// </summary>
public interface IMeasure : IOrderable
{
	event Action<IMeasure> PositionChanged;
	long Position { get; }
	void SetPosition(long position);
	void SetPositionAndInvoke(long position);







	public static IMeasure Zero => new Measure(0);
	public static IMeasure One => new Measure(1);
	//public static IMeasure operator -(IMeasure a) => new Measure(-a.Position);
	//public static IMeasure operator ++(IMeasure a) => new Measure(a.Position + 1);
	//public static IMeasure operator --(IMeasure a) => new Measure(a.Position - 1);
	//public abstract static bool operator ==(IMeasure a, IMeasure b);
	//public abstract static bool operator !=(IMeasure a, IMeasure b);

	//public static bool operator >(IMeasure a, IMeasure b) =>  a.Position > b.Position;
	//public static bool operator <(IMeasure a, IMeasure b) => a.Position < b.Position;
	//public static bool operator >=(IMeasure a, IMeasure b) => a.Position >= b.Position;
	//public static bool operator <=(IMeasure a, IMeasure b) => a.Position <= b.Position;

	//public static IMeasure operator +(IMeasure a, IMeasure b) => new Measure(a.Position + b.Position);

	//public static IMeasure operator -(IMeasure a, IMeasure b) => new Measure(a.Position - b.Position);

	//public static IMeasure operator *(IMeasure a, IMeasure b) => new Measure(a.Position * b.Position);

	//public static IMeasure operator /(IMeasure a, IMeasure b)
	//{
	//	if (b.Position == 0)
	//	{
	//		// Zero means below resolution, so dividing say 40/0 means greater than 40, but less than max.
	//		// at some point there may need to be +0 and -0, but for now the sign comes from the numerator.
	//		return new Measure(a.Position);
	//	}

	//	// Both finite, nonzero denominator
	//	long newPosition = a.Position / b.Position;
	//	return new Measure(newPosition);
	//}

}
