using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Scraps;

// todo: Change evals a lot. Bool ops are equations that are all filled in and evaluated for truths. Not just true or false, but near, overlap, contain, touch etc. Akin to prepositions.
public class BoolExpression : IConstraint
{
    public CertaintyLevel Certainty { get; set; } = CertaintyLevel.Uncertain;
	public virtual string Name { get; } = "";

	public Measure First { get; set; } // this is optionally a long value, a pointer to a Number and the measure, or a different ValueSample
	public Measure Second { get; set; }
	public TwoMeasureOp Op { get; set; }

	// This can not set its own value, as the value is calculated.
	// Instead, it examines which inputs (First and Second) are fixed and which are variable,
	// and adjusts them as needed. If both are variable, it creates a new constraint that
	// the solution must follow (or perhaps this is that constraint already, need to try).
	public INumber Result { get => Calculate(); protected set => _result = value; }
	private INumber _result;
	public bool BoolValue => false; // todo: Check if the result is a potential subset of the expression result
	private bool _boolValue;
	public bool IsSet => Certainty.HasValue();
	public BoolExpression(Measure first, Measure second, TwoMeasureOp op, CertaintyLevel state = CertaintyLevel.Uncertain)
	{
		First = first;
		Second = second;
		Op = op;
		Certainty = state;
		Result = Calculate();
	}
	protected BoolExpression()
	{
	}
	public void Fix() => Certainty = CertaintyLevel.Fixed;
	public void Set() => Certainty = CertaintyLevel.Set;
	public void Unset() => Certainty = CertaintyLevel.Uncertain;
    public INumber Calculate()
    {
		long val = First.Position - Second.Position;
		//_result = (long)val;
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
			//case TwoMeasureOp.Mod:
			//	_result = First.Position % Second.Position;
			//	_boolValue = val != 0;
			//	break;
			default:
				break;
		}
		return _result;
	}

    public void SetCertaintyLevel(CertaintyLevel certaintyLevel)
    {
        throw new NotImplementedException();
    }

}
