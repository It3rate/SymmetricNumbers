using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public class Accumulator : IValue
{
	private UnconstrainedNumber _number;
	public double Value => throw new NotImplementedException();

	public CertaintyLevel CertaintyLevel => CertaintyLevel.Fixed;
	public Accumulator(UnconstrainedNumber number)
	{
		_number = number;
	}
	public void Add(ValueRef a)
	{
		if (a.RefKind.ContainsA())
		{
			_number.RightMeasure.SetPosition(_number.RightMeasure.Position + a.RefNumber.RightMeasurePosition);
		}
		if (a.RefKind.ContainsB())
		{
			_number.LeftUnit.SetPosition(_number.LeftUnit.Position + a.RefNumber.LeftUnitPosition);
		}

		if (a.RefKind.ContainsD())
		{
			_number.LeftMeasure.SetPosition(_number.LeftMeasure.Position + a.RefNumber.LeftMeasurePosition);
		}
		if (a.RefKind.ContainsC())
		{
			_number.RightUnit.SetPosition(_number.RightUnit.Position + a.RefNumber.RightUnitPosition);
		}

	}
}
