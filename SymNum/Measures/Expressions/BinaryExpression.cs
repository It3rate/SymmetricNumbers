using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;
public class BinaryExpression : IValue
{
	public IValue First { get; } // this is optionally a long value, a pointer to a Number and the measure, or a different ValueSample
	public IValue Second { get; }

	// This can not set its own value, as the value is calculated.
	// Instead, it examines which inputs (First and Second) are fixed and which are variable,
	// and adjusts them as needed. If both are variable, it creates a new constraint that
	// the solution must follow (or perhaps this is that constraint already, need to try).
	public double Value { get; protected set; }
	public MeasureState State { get; set; }
	public BinaryOp Op { get; set; }
	public bool IsSet => State.HasValue();
	public BinaryExpression(IValue first, IValue second, BinaryOp op, MeasureState state = MeasureState.Unknown)
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
				Value = First.Value * Second.Value;
				break;
			case BinaryOp.Divide:
				Value = Second.Value == 0 ? 0 : First.Value / Second.Value;
				break;
			case BinaryOp.Add:
				Value = First.Value + Second.Value;
				break;
			case BinaryOp.Subtract:
				Value = First.Value - Second.Value;
				break;
			case BinaryOp.Pow:
				Value = Math.Pow(First.Value, Second.Value);
				break;
			case BinaryOp.Mod:
				Value = First.Value % Second.Value;
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