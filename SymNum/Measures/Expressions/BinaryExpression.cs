using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;
public class BinaryExpression : ValueBase
{
	public double First { get; } // this is optionally a long value, a pointer to a Number and the measure, or a different ValueSample
	public double Second { get; }
	public override double Value { get; protected set; }
	public MeasureState State { get; set; }
	public BinaryOp Op { get; set; }
	public bool IsSet => State.HasValue();
	public BinaryExpression(double first, double second, BinaryOp op, MeasureState state = MeasureState.Unknown)
	{
		First = first;
		Second = second;
		Op = op;
		State = state;
		Calculate();
	}
	public void UpdateValue(double value)
	{
		Value = value;
	}
	public void Fix() => State = MeasureState.Fixed;
	public void Set() => State = MeasureState.Set;
	public void Unset() => State = MeasureState.Uncertain;
	private void Calculate()
	{
		switch (Op)
		{
			case BinaryOp.Multiply:
				Value = First * Second;
				break;
			case BinaryOp.Divide:
				Value = Second == 0 ? 0 : First / (double)Second;
				break;
			case BinaryOp.Add:
				Value = First + Second;
				break;
			case BinaryOp.Subtract:
				Value = First - Second;
				break;
			case BinaryOp.Pow:
				Value = Math.Pow(First, Second);
				break;
			case BinaryOp.Mod:
				Value = First % Second;
				break;
		}
	}
}

public enum BinaryOp
{
	None,
	Multiply,
	Divide,
	Add,
	Subtract,
	Pow,
	Mod,
	// sin cos Math.etc
}