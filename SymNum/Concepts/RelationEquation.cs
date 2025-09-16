using SymNum.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Concepts;


public class RelationEquation // this will be more of an expression class
{
	public Equation Equation { get; set; }
	public Func<DenominatorAspect, Extent> Relation { get; set; } // test of how things relate to this aspect
}
