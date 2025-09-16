using SymNum.Concepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Expression;

public class Operation
{
	public Extent Value { get; set; }
	public Func<Extent, Extent> Operator { get; set; }
}
