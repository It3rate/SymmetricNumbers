using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public enum TwoMeasureOp
{
	None,
	Equal,
	NotEqual,
	GreaterThanOrEqual,
	LessThanOrEqual,
	GreaterThan,
	LessThan,
	Mod,
}
public enum TwoNumberOp
{
	None,
	Equal,
	NotEqual,
	GreaterThan,
	LessThan,
	Mod,
	Add,
	Subtract,
	Multiply,
	Divide,
	Pow,
	Interpolate, // There is a whole category of binary operations that represent expected motion.
				 // sin cos Math.etc
	Custom0,
	Custom1,
	Custom2,
	Custom3,
	Custom4,
	Custom5,
}
