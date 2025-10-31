
using SymNum.Expressions;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Concepts;

/// <summary>
/// This is like a property, but it is universal, and everything can have a relation to it (even if not normally relevant).
/// </summary>
public class PropertyCharacteristics
{
	public PropertyUnit Unit { get; set; }
	public Equation Relation { get; set; }
	public Extent MinMax { get; set; }

}
