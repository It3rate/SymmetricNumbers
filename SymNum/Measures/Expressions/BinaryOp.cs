using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public delegate INumber BinaryOp(INumber left, INumber right);
public static class Ops
{
	public static BinaryOp Add = (left, right) => left.Add(right);
	public static BinaryOp Subtract = (left, right) => left.Subtract(right);
	public static BinaryOp Multiply = (left, right) => left.Multiply(right);
	public static BinaryOp Divide = (left, right) => left.Divide(right);
}
public enum BinaryOpKind
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