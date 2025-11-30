
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SymNum.Numbers;

namespace SymNum.Expressions;

// Operations can happen on any two IOrderable values. The possible operations grow just as the numeric complexity does.
// 

public delegate INumber BinaryOp(INumber left, INumber right);
public static class Ops
{
	public static BinaryOp AddTo = (left, right) => left.AddTo(right);
	public static BinaryOp SubtractTo = (left, right) => left.SubtractTo(right);
	public static BinaryOp MultiplyTo = (left, right) => left.MultiplyTo(right);
	public static BinaryOp DivideTo = (left, right) => left.DivideTo(right);


	public static BinaryOp Add = (left, right) => left.Add( right);
	public static BinaryOp Subtract = (left, right) => left.Subtract(right);
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