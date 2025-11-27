using SymNum.Numbers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Expressions;
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
public class BinaryExpression 
{
	public INumber First { get; set; } // this is optionally a long value, a pointer to a Number and the measure, or a different ValueSample
    public INumber Second { get; set; }
    public virtual BinaryOp Op { get; protected set; }
    public INumber Result { get; protected set; }
    // Calculations and predictions are expected results, the actual result is measured.
    // The diff is error/information and can be used in if/else etc.
    public INumber ExpectedResult { get; set; } 

	// This can not set its own value, as the value is calculated.
	// Instead, it examines which inputs (First and Second) are fixed and which are variable,
	// and adjusts them as needed. If both are variable, it creates a new constraint that
	// the solution must follow (or perhaps this is that constraint already, need to try).
	public INumber Value { get => Calculate(); protected set => _value = value; }
	private INumber _value;
	public CertaintyLevel Certainty { get; set; }
    public virtual string Name { get; } = "";
	public bool IsSet => Certainty.IsInput();
	public BinaryExpression(INumber? first, INumber? second, BinaryOp op, INumber? result, CertaintyLevel state = CertaintyLevel.Uncertain)
	{
		First = first ?? RawNumber.Unknown;
		Second = second ?? RawNumber.Unknown;
		Op = op;
        Result = result ?? new UnbalancedNumber(0,0,0,0);
        Calculate();
        Certainty = state;
		
    }
    protected BinaryExpression()
    {
    }

    public void SetCertaintyLevel(CertaintyLevel certaintyLevel)
    {
        Certainty = certaintyLevel;
    }

	public void Set() => Certainty = CertaintyLevel.Output;
	public void Unset() => Certainty = CertaintyLevel.Uncertain;
	public INumber Calculate()
	{
        if (First.IsFullyDefined && Second.IsFullyDefined)
        {
			Result.SetWith(Op.Invoke(First, Second));
        }
        else
        {
            Result = RawNumber.Unknown;
        }
        return Result;
	}
}

//public class AnchorExpression : BinaryExpression
//{
//	public AnchorNumber AnchorResult => (Result as AnchorNumber)!;
//	public AnchorExpression(INumber? first, INumber? second,  CertaintyLevel state = CertaintyLevel.Uncertain) :
//		base(first, second, Ops.Subtract, new AnchorNumber(0, first.RightUnitPosition), state)
//	{
//	}

//}