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

/// <summary>
/// This is a measure not accessed from a number, 
/// so it doesn't have access to things like certainty, scale, bias, min/max values, etc.
/// Essentially it is used as a first level input and isn't manipulated directly.
/// Ultimately types come from primary inputs, like distance/time -> speed. 
/// Km or miles is an equation, speed is the primarires.
/// </summary>
public interface IMeasure 
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
