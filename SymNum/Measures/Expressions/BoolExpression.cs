using SymNum.Measures.Numbers;
using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public class BoolExpression : IValue, IConstraint
{
	public IMeasure First { get; set; } // this is optionally a long value, a pointer to a Number and the measure, or a different ValueSample
	public IMeasure Second { get; set; }
	public TwoMeasureOp Op { get; set; }

	// This can not set its own value, as the value is calculated.
	// Instead, it examines which inputs (First and Second) are fixed and which are variable,
	// and adjusts them as needed. If both are variable, it creates a new constraint that
	// the solution must follow (or perhaps this is that constraint already, need to try).
	public double Value { get => Calculate(); protected set => _value = value; }
	private double _value;
	public bool BoolValue => _value != 0;
	private bool _boolValue;
	public CertaintyLevel CertaintyLevel { get; set; }
	public virtual string Name { get; } = "";
	public bool IsSet => CertaintyLevel.HasValue();
	public BoolExpression(IMeasure first, IMeasure second, TwoMeasureOp op, CertaintyLevel state = CertaintyLevel.Unknown)
	{
		First = first;
		Second = second;
		Op = op;
		CertaintyLevel = state;
		Calculate();
	}
	protected BoolExpression()
	{
	}
	public void UpdateValue(double value)
	{
		_value = value;
	}
	public void Fix() => CertaintyLevel = CertaintyLevel.Fixed;
	public void Set() => CertaintyLevel = CertaintyLevel.Set;
	public void Unset() => CertaintyLevel = CertaintyLevel.Uncertain;
	public double Calculate()
	{
		long val = First.Position - Second.Position;
		_value = (long)val;
		switch (Op)
		{
			case TwoMeasureOp.Equal:
				_boolValue = val == 0;
				break;
			case TwoMeasureOp.NotEqual:
				_boolValue = val != 0;
				break;
			case TwoMeasureOp.GreaterThan:
				_boolValue = val > 0;
				break;
			case TwoMeasureOp.LessThan:
				_boolValue = val < 0;
				break;
			case TwoMeasureOp.GreaterThanOrEqual:
				_boolValue = val >= 0;
				break;
			case TwoMeasureOp.LessThanOrEqual:
				_boolValue = val <= 0;
				break;
			case TwoMeasureOp.Mod:
				_value = First.Position % Second.Position;
				_boolValue = val != 0;
				break;
			default:
				break;
		}
		return _value;
	}
}
