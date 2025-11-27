using SymNum.Numbers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Expressions;

// An expression is a way to calibrate measurements. This include ratios, numbers, and more complex expressions.
// A Measure is not an expression, but all expressions use measures.
// Expressions can work on multiple Numbers, Number parts, and Measures themselves.
// So a+b should allow number + measure. This implies measures always need to be part of a number (in a slot) to have meaning.
// 
// An expression is anything that recognizes or can potentially cause 'less random' at some scale.
// Typical math operations cover a lot of these, scale, add, through to the mandelbrot set.
// Note expressions say nothing about the utility, accuracy or even the randomness of the equation.
// Lottery numbers are randomly generated, but can easily be fit to some 'equation' after the fact.
// expressions will tell you about a system if they are in fact cleaving something non-random out of an event,
// but they can't tell you if the pattern you see in 2,4,6,8 is actually a real pattern or just a conincidence.
// The expressions do describe a 'less random' pattern (which is what a pattern is), 
// but more samples (2,4,6,8,77,13...) may still show the '+2' expression is a bad fit.
// 
// All to say, an expression is like a math expression, but not restricted to that and is probably from a much larger space.
// It does however obey whatever rules cause it to be less than random,
// and these rules are fixed across all domains by their own definition. Addition is addition regardless of what you add.

// Note: the higher the values of a thing, the more certain we are of it. Values can drift and disappear even,
// but as the numerator and denominator accumulate to higher values (2/3 -> 200/300), the stability increases.
// As measures are useful they increase, and as they drift to zero (not useful) they die and a new pairing is created.
// These expressions are like 'straps' between other values, and processing them in certain ways can increase understanding,
// and utility. So it is important that they are not only tuned, but they are operationally useful, and they connect useful measures.
// These higher accumulated values also can suggest focus and preferences.
public interface IExpression : INumber
{
	// Q.perhaps expressions always apply to IMeasures?
	// If it is to a number value you need to do the n/d division as an op?
	// Q. Or there are expressions for for the IMeasure level, and expression for the INumber level?
	// In this case, the expressions may always be computed as doubles and then returned to fractions.
	// In a bool segment sense there are the bool ops, then addition is bool appends to each end. Scaling is bool ratios.
	
	// A. No, these must be INumbers, because a single IMeasure is not a segment, so bool ops don't work
	// As numbers, the denominators are used in the ratios, and bools ops happen on the segments
	// That said, there are a number of ops that happen within a number,
	// basically any measures can op with each other in ways that make sense (n/d, d/n, length, midpoint, norm, etc)
	// these are not expressions, more Number interpretations.

	// Each op moves input or not, adjusts or replaces output, (can it create new output?)
	// Q. always 2 inputs and 1 output? To adjust, one input is the same as the output?
	// move would be 1 input to 1 output, no op?


	INumber InputA { get; }
	INumber InputB { get; }
	INumber Output { get; }

	BinaryOp Op { get; }
	void Calculate();
}
