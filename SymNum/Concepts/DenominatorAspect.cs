using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Concepts;

/// <summary>
/// The denominator, is always '1 unit', but is composed of a parameterized comparable equation.
/// This is not a concept, as those are usually composed of many aspects, and this is a selected single 'type'.
/// E.g. A table is something with 4 legs, a flat surface, a place to work, a place to gather, etc. 
/// This would represent only one of those, and could also be used in 'plateau', 'floor' or 'parking lot'.
/// </summary>
public class DenominatorAspect : Extent
{
	public bool Polarity { get; protected set; }
	public DenominatorAspect(long start, long end, bool polarity) : base(start, end)
	{
		Polarity = polarity;
	}
}
