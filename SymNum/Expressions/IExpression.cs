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
public interface IExpression
{
}
