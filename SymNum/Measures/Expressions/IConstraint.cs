using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public interface IConstraint : IValue
{
	//IValue First { get; }
	//IValue Second { get; }
	//TwoNumberOp Op { get; }
	public double Calculate();
	string? Name { get; }
}
