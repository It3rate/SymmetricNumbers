using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Patterns;

/// <summary>
/// A repeating operation that may be parameterized. 
/// This is probably a property on all types of numbers, or on an equation etc, but separating out the concept for now.
/// This requires at least 6 longs (3 extents), 4 for a minimally calibrated number, and 2+ to characterize the repetition.
/// The easiest example of repetition is exponents, which is repeated multiplication, 
/// but can also apply to addition (which we confuse with multiplication, 3+3+3+3 combines substances, where 3*4 'stretches' substance)
/// This repetion can vary over iterations like a sine wave, and introspect like with fibonacci.
/// </summary>
public class RepeatableNumber
{
}
