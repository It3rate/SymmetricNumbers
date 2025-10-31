
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Numbers;
using SymNum.Primitives;
namespace SymNum.Expressions;

public class Accumulator : IValue
{
	private RawNumber _number;
	public double Value => throw new NotImplementedException();

	public CertaintyLevel Certainty => CertaintyLevel.Fixed;
	public Accumulator(RawNumber number)
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
