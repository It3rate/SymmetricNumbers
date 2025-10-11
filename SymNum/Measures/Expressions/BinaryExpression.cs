using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;
/// <summary>
/// A binary expression is a constraint. It can be named and optionally set.
/// Setting it will constrain/adjust the other values to satisfy the new value, based on their CertaintyLevel.
/// A number can have multiple constraints, which will be solved together.
/// Example: You have a complex number that is length and width, and add an Area constraint.
///   You can now lock width, set area, and it will adjust length to satisfy the constraint.
///   You could do the same with aspect ratio, or perimeter, the constraints determine the utility and character of a number.
///   These constraints could also happen on multiple numbers, like area and height making volume. 
///   Volume is then an expression (which is a type of number), constrained by the product of area and height.
/// </summary>
public class BinaryExpression : IValue, IConstraint
{
	public IValue First { get; set; } // this is optionally a long value, a pointer to a Number and the measure, or a different ValueSample
    public IValue Second { get; set; }
    public BinaryOp Op { get; set; }

	// This can not set its own value, as the value is calculated.
	// Instead, it examines which inputs (First and Second) are fixed and which are variable,
	// and adjusts them as needed. If both are variable, it creates a new constraint that
	// the solution must follow (or perhaps this is that constraint already, need to try).
	public double Value { get; protected set; }
	public CertaintyLevel CertaintyLevel { get; set; }
    public virtual string Name { get; } = "";
	public bool IsSet => CertaintyLevel.HasValue();
	public BinaryExpression(IValue first, IValue second, BinaryOp op, CertaintyLevel state = CertaintyLevel.Unknown)
	{
		First = first;
		Second = second;
		Op = op;
		CertaintyLevel = state;
		Calculate();
    }
    protected BinaryExpression()
    {
    }
    public void UpdateValue(double value)
	{
		Value = value;
	}
	public void Fix() => CertaintyLevel = CertaintyLevel.Fixed;
	public void Set() => CertaintyLevel = CertaintyLevel.Set;
	public void Unset() => CertaintyLevel = CertaintyLevel.Uncertain;
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
	Interpolate, // There is a whole category of binary operations that represent expected motion.
	// sin cos Math.etc
	Custom0,
	Custom1,
	Custom2,
	Custom3,
	Custom4,
	Custom5,
}