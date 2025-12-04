using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Expressions;
namespace SymNum.Primitives;

// Requirement: Groupings of 'Measure' need to be able to smoothly move between bools, enums, ints, floats, complex nums, symmetric numbs, equations, and probabilities.
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

// another take
// 1 point, no location, motion, direction, length - just a point
// 2 points gives a segment with direction and length
// now min extra information is internally duplicate the length, to get a third point.
//    Lengths are equal, so will be additive, like counting. Order is direction of build.
//    Each chain can grow from either end, but only knows its seg count, it has no way of recording its direction preference (both are valid and equal)
// if you have 2 segments (external, not required to be the same), you can attach to create a comparison (four points).
//    This ratio is the basis of multiplication. It records a ratio by impling a new point, but can't solve for it.

/// <summary>
/// This is a measure not accessed from a number, 
/// so it doesn't have access to things like certainty, scale, bias, min/max values, etc.
/// Essentially it is used as a first level input and isn't manipulated directly.
/// It is more like an unset memory location than a typed value. It has no value, but a layer above can set it's value.
/// Ultimately types come from primary inputs, like distance/time -> speed. 
/// Km or miles is an equation, speed is the primarires.
/// </summary>
public interface IMeasure<T> : IOrderable
{
	event Action<IMeasure<T>> PositionChanged;
	T Position { get; }
	T NegativePosition { get; }

	// While the position is correct, it is calibrated externally. Internally, an Measure has no idea where it is, because it has no dimension.
	//void SetPosition(T position);
	//void SetPositionAndInvoke(T position);







	//public static Measure operator -(Measure a) => new Measure(-a.Position);
	//public static Measure operator ++(Measure a) => new Measure(a.Position + 1);
	//public static Measure operator --(Measure a) => new Measure(a.Position - 1);
	//public abstract static bool operator ==(Measure a, Measure b);
	//public abstract static bool operator !=(Measure a, Measure b);

	//public static bool operator >(Measure a, Measure b) =>  a.Position > b.Position;
	//public static bool operator <(Measure a, Measure b) => a.Position < b.Position;
	//public static bool operator >=(Measure a, Measure b) => a.Position >= b.Position;
	//public static bool operator <=(Measure a, Measure b) => a.Position <= b.Position;

	//public static Measure operator +(Measure a, Measure b) => new Measure(a.Position + b.Position);

	//public static Measure operator -(Measure a, Measure b) => new Measure(a.Position - b.Position);

	//public static Measure operator *(Measure a, Measure b) => new Measure(a.Position * b.Position);

	//public static Measure operator /(Measure a, Measure b)
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
