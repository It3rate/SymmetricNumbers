using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

/// <summary>
/// An expression is a number, as in, a way to calibrate measurements.
/// An expression is anything that recognizes or can cause 'less random' at some scale.
/// Typical math operations cover a lot of these, scale, add, through to the mandelbrot set.
/// Note expressions say nothing about the utility, accuracy or even the randomness of the equation.
/// Lottery numbers are randomly generated, but can easily be fit to some 'equation' after the fact.
/// expressions will tell you about a system if they are in fact cleaving something non-random out of an event,
/// but they can't tell you if the pattern you see in 2,4,6,8 is actually a real pattern or just a conincidence.
/// The expressions do describe a 'less random' pattern (which is what a pattern is), 
/// but more samples (2,4,6,8,77,13...) may still show the '+2' expression is a bad fit.
/// 
/// All to say, an expression is like a math expression, but not restricted to that and is probably from a much larger space.
/// It does however obey whatever rules cause it to be less than random,
/// and these rules are fixed across all domains by their own definition. Addition is addition regardless of what you add.
/// </summary>
public interface IExpression
{
}
