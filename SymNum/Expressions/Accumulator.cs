
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Numbers;
using SymNum.Primitives;
using SymNum.Scraps;
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
	//public void Add(ValueRef a)
	//{
	//	if (a.RefKind.ContainsA())
	//	{
	//		_number.RightMeasure.SetPositionAndInvoke(_number.RightMeasure.Position + a.RefNumber.RightMeasurePosition);
	//	}
	//	if (a.RefKind.ContainsB())
	//	{
	//		_number.LeftUnit.SetPositionAndInvoke(_number.LeftUnit.Position + a.RefNumber.LeftUnitPosition);
	//	}

	//	if (a.RefKind.ContainsD())
	//	{
	//		_number.LeftMeasure.SetPositionAndInvoke(_number.LeftMeasure.Position + a.RefNumber.LeftMeasurePosition);
	//	}
	//	if (a.RefKind.ContainsC())
	//	{
	//		_number.RightUnit.SetPositionAndInvoke(_number.RightUnit.Position + a.RefNumber.RightUnitPosition);
	//	}

	//}
}
