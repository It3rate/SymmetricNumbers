using SymNum.Measures.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Numbers;
/// <summary>
/// These are relations across four measures that form a constraint.
/// Different values can be locked or relaxed while others are changed.
/// This allows the constraint to resolve using the uncertain values in order to resolidify the constraint.
/// If it is not solveable, a new constraint can be created that the solution must fall within.
/// E.g. Set a rect's area, you don't know if you need to change the width, height, or both.
/// but you do know the height for whatever the width is set to and vice versa.
/// This fact can be be expressed with a new constraint number, or derived from the area constraint (a=wh) in fact.
/// </summary>
public class ConstrainedNumber : RawNumber
{
	/// <summary>
	/// Values are constrained by equations that must be true.
	/// Starting with BinaryExpressions, but will include things like greater than, shared start point, 
	/// and more complex relations such are perpendicular to, etc.
	/// </summary>
	public List<BinaryExpression> Constraints { get; }

	public ConstrainedNumber(params BinaryExpression[] constraints)  
	{
		Constraints = constraints.ToList();
	}

	public void Calculate()
	{
		// check fixed and settable, calculate result.
		// account for custom equations.
		// can loop through Binary expression and get them to set things.
		// may require multiple passes.
	}
	public virtual void SetCustom0() { }
	public virtual void SetCustom0Value(double value) { }
	public virtual double GetCustom0() => 0;
	public virtual void SetCustom1() { }
	public virtual double GetCustom1() => 0;
	public virtual void SetCustom2() { }
	public virtual double GetCustom2() => 0;
	public virtual void SetCustom3() { }
	public virtual double GetCustom3() => 0;
	public virtual void SetCustom4() { }
	public virtual double GetCustom4() => 0;
	public virtual void SetCustom5() { }
	public virtual double GetCustom5() => 0;
}
// example