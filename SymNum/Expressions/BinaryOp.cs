
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SymNum.Numbers;

namespace SymNum.Expressions;

public delegate INumber BinaryOp(INumber left, INumber right);
public static class Ops
{
	public static BinaryOp Add = (left, right) => left.Add(right);
	public static BinaryOp Subtract = (left, right) => left.Subtract(right);
	public static BinaryOp Multiply = (left, right) => left.Multiply(right);
	public static BinaryOp Divide = (left, right) => left.Divide(right);
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