using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Pairs;

// Every type can be paired with another element of it's level to create a Pair Level.
// Every Pair has a length that is in [Level - 1] units.
// Pair elements are ordered A and B, but they have no way of knowing which direction is considered 'up', so don't know their direction.
// e.g. they may know A is higher than B, but they don't know if they are measuring height or depth, as there is no calibration unit.
public interface IPair<SubLevel> // Level 2 min
{
	SubLevel A { get; } // [Level - 1] units
	SubLevel B { get; } // [Level - 1] units
	SubLevel Length { get; } // Length is always in [Level - 1] units
	void Invert();
}
