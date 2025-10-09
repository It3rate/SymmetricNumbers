using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public interface IConstraint : IValue
{
	IValue First { get; }
	IValue Second { get; }
	BinaryOp Op { get; set; }
	string Name { get; }
}
