using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Primitives;
namespace SymNum.Numbers;

// A number is made of at least 4 points, two for a unit (start and end), and two for a value (start and end).
// There is often an implied 5th point, 'here', or 'now', or 'this'. This is the floating zero point, or reference point.
// In regular numbers, 'here' is always 0, and both segments also start from 0.
//   The unit is arbitrary length (depending on resolution), but defined as 1.
//   So it seems there is one value, but there are 4 values (and the viewer's persective).
// In complex numbers, there is a unit for each direction.
//   The first segment goes from 'bad' (-1 position) to 'not bad' (postive direction value).
//   The second segment goes from 'good' (+1 position) to 'not good' (negative direction value).
//   The viewer's perspective is still 0, but there are both a postive direction unit and a negative direction unit.
//   The units are usually the same length (thus resolution), though perhaps they don't have to be.
//   All things must be defines relative to something else, and in the pure form two oppostive segments are defined relative to each other.
// In symmetric numbers, there are no units, everything is relative to each other.
//   The units slots are indicate what is happening on the polarity and direction.
//   Values can be positive or negative, and mirrored or not mirrored.
//   If you calculate the implications of these transformations,
//   you will get all symmetries, and contraditions the break into higher dimensions.

// A number's resolution can be a threshold or an average. This matters a lot when considering the meaning of 0 in a fraction.
// If 2 means an unknown between 1.5 and 2.5, then 2*2 is between 2.25 and 6.25
// If 2 means 2.0 or more, but less than 3.0, then 2*2 is between 4.0 and 9.0
// With averages, 3 / 0 (where zero has a value but it's below your resolution), 0 is between -0.5 and 0.5, so 3 / 0 is greater than 6, or less than -6.
// This is how you can get inverted ranges, a not operation (the whole numberline except some section)
// Inverting the average interpretation (0/3) gets the inverse, a range between -6 and 6.
// With thresholds you can get one sided ranges. If 0 is greater than zero and less than 1, then 3/0 is just greater than 3.
// The issue with thresholds is it requires a +0 and -0. There are two edges between -1 and 1, but only one node.
// This implies than resolution for numbers has to specify how it obtains it's resolution if you want to scale those values, which makes sense.
public interface INumber
{
	/// <summary>
	/// The holder for raw measure values. Can be redirected for refs.
	/// </summary>
	IMeasure[] Measures { get; }
	/// <summary>
	/// A positive right measured number of ticks.
	/// </summary>
	IMeasure RightMeasure { get; }
	/// <summary>
	/// A positive right resolution in ticks.
	/// </summary>
	IMeasure RightUnit { get; }
	/// <summary>
	/// A positive left measured number of ticks.
	/// </summary>
	IMeasure LeftMeasure { get; }
	/// <summary>
	/// A positive left resolution in ticks.
	/// </summary>
	IMeasure LeftUnit { get; }
	/// <summary>
	/// The Observer point, defaults to zero but by no means is required. Some numbers (ratios) do not need a zero point.
	/// </summary>
	IMeasure ZeroPoint { get; }
	void SetWith(INumber number);
	long RightMeasurePosition { get; }
	long RightUnitPosition { get; }
	long LeftMeasurePosition { get; }
	long LeftUnitPosition { get; }
	long ZeroPosition { get; }
	bool IsFullyDefined { get; }

	float LeftValue { get; }
	float RightValue { get; }

	RationalNumber GetRational();
	ImaginaryNumber GetImaginary();
	ComplexNumber GetComplex();
	UnbalancedNumber GetUnbalanced();
	SymmetricNumber GetSymmetric();
    void Mirror();
	INumber Negate();

	INumber AddTo(INumber other);
	INumber SubtractTo(INumber other);
	INumber MultiplyTo(INumber other);
	INumber DivideTo(INumber other);
	INumber AlignTo(INumber other);
	INumber Add(INumber other);
	INumber Subtract(INumber other);

	// Add   
	// Subtract   
	// Multiply   
	// Divide 
	// Reciprocal 
	// Power 
	// DotProduct     
	// CrossProduct
	// Modulo
	// Transform
	// 16 bool ops
	// adjust resolution
}

public interface IPartialNumber : INumber
{
	bool IsRationalPart { get; }
	bool IsImaginaryPart { get; }
}