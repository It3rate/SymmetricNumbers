using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Primitives;
namespace SymNum.Expressions;

/// <summary>
/// IOrderable is anything that can be ordered, a measure, a number, expression, or pattern.
/// Expressions/bool operations and patterns do not have an obvious order, but the results do.
/// The order of the 16 bool expressions is therefore not universally set, but depends on the input.
/// The ordering depends on input as well as certainty, or even things like variance/acceleration (x^3 > x^2).
/// It has variable certainty, and can participate in mathematical like operations.
/// </summary>
public interface IOrderable
{
	// Note: The way IOrderable values scale in complexity is to combine as equations, without being simplified.
	// The result of an equations is always in the same level as the inputs, so combining two measures/numbers/equations creates a measure/number/equation.
	// However if you can't combine/solve it, two measures creates a segment or ratio, two ratios create a number, two numbers create an equation.
	// So as long as you are successfully combining, you can add as many inputs as you like without going up a level.
	// but to keep the equation dynamic causes the complexity to increase. 2*2*2=8 is a number,
	// but if you keep it as a*a*a, it is 3x more complex than a number, becase the result is the equation pattern, not the number.
	// 'Solving' means reducing the expression to a knowable value at that level.
	// So 2/3=1 if you solve it, but can remain .666 if you don't.
	// a/b or 2/3 is the equation pattern, not the result.
	// In the case of 1+2+3, the result and the pattern result are the same, but one is equivient to a+b+c, the other is 6.

	(float, float) ValueRange { get; }
	(long, long) PositionRange { get; }
	//CertaintyLevel Certainty { get; }
	// IMeasures can combine with INumbers in limited ways, perhaps one it by replacing a specific slot (the op is the slot).
	IOrderable CombineWith(IOrderable other, BinaryOp op);

	// gt, lt, eq, sort, hash, 
	List<BinaryOp> ValidOps { get; } // each level adds valid ops? Subsequent layers need to implement previous ones.
	IMeasure CompareTo(IOrderable other, BinaryOp op);

	bool Equals(object? obj);
	int GetHashCode();

}