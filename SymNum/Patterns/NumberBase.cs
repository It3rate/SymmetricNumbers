using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Concepts;

namespace SymNum.Patterns;

/// <summary>
/// There are many types of numbers, they just mean measures calibrated in some way. 
/// This is the base class which can grow into numbers, complex numbers, symmetric numbers, dynamic zero point numbers,
/// equations, patterns etc. All of these are just ways of organizing and calibrating values to have meaning 
/// (aka be non random).
/// Note that these can describe any non random pattern, but have no information about what the pattern represents.
/// This requires some kind of unit tied to the number. Numbers themselves are pure patterns.
/// </summary>
public class NumberBase
{
    /// <summary>
    /// A number can be composed of multiple extents with different restrictions. 
    /// A simple number has a numerator and denominator, where a position in 3D space has 6 extents (3 numbers).
    /// An equation (eg a sinewave, bezier curve, color curve of heating) can be arbitrarily complex, 
    /// but they are all 'numbers' for lack of a better word.
    /// </summary>
    public List<Extent> Extents { get; set; }

}
