using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

/// <summary>
/// The denominator, is always '1 unit', but is composed of a parameterized comparable equation.
/// This is not a concept, as those are usually composed of many aspects, and this is a selected single 'type'.
/// E.g. A table is something with 4 legs, a flat surface, a place to work, a place to gather, etc. 
/// This would represent only one of those, and could also be used in 'plateau', 'floor' or 'parking lot'.
/// </summary>
public class Aspect
{
	public Extent Extent {get; set; } // default 1
	public bool Polarity { get; protected set; }

	// There exists a long list of 'equation shapes' that are gradually more specific,
	// starting with random/ordered, to ==/!=, ><, offset,
	// thru to sample/time relations like linear, curved, alternating, all the way to just sampled.
	// Then there is something like a grid of aspects and objects, where you fill in best guess for the relation.
	// This list of relations cluster with similar things (different apples) and that's what defines them.
	// Values can be predicted based on the results of similar clusters, and different clusters filtered with only specific aspects.
	// This grid like thing lives in Brain, not aspect.
	public Func<Aspect, long> Relation { get; set; } // test of how things relate to this aspect
}
