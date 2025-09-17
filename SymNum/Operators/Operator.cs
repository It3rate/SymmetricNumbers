
using SymNum.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Operators;

/// <summary>
/// An operator is anything that recognizes or causes 'less random' at some scale.
/// Typical math operations cover a lot of these, scale, add, through to the mandelbrot set.
/// Note operators say nothing about the utility, accuracy or even the randomness of the equation.
/// Lottery numbers are randomly generated, but can easily be fit to some operations after the fact.
/// Operators will tell you about a system if they are in fact cleaving something non-random out of an event,
/// but they can't tell you if the pattern you see in 2,4,6,8 is actually a real pattern or just a conincidence.
/// The operators do describe a 'less random' pattern (which is what a pattern is), 
/// but more samples (2,4,6,8,77,13...) may still show the '+2' operator is a bad fit.
/// 
/// All to say, an operator is like a math operator, but not restricted to that and is probably from a much larger space.
/// It does however obey whatever rules cause it to be less than random,
/// and these rules are fixed across all domains by their own definition. Addition is addition regardless of what you add.
/// </summary>
public class Operator
{
    public Extent Value { get; set; }
    public Func<Extent, Extent> Op { get; set; }
}
