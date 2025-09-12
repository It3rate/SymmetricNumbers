using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;


public class Relation // this will be more of an expression class
{
	public Extent[] Scale { get; set; }
	public Extent[] Offset { get; set; }
	public object RepetitionStyle { get; set; }

}
