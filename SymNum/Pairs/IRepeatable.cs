using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Pairs;

// an operation that repeats/grpws by combining with elements of its level, withougt changing level.
// This is like addition/subtraction, where two points can make a segment, and then a third can extend or retard it.
// All the original properties of the pair are available in the triple, and there is no new transforms, only the extra positions.
public interface IRepeatable<Level>
{
	List<Level> Repeats { get; } // All the original Level ops must still be valid
}
