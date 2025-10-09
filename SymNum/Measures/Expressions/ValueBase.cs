using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public abstract class ValueBase
{
	public abstract double Value { get; protected set; }
}
