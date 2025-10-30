using SymNum.Measures.Expressions;
using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Numbers.Common;

public class RectangleNumber : ComplexNumber
{
	private INumber _numberRef;
	public override IMeasure[] Measures => _numberRef.Measures;	
	IPartialNumber Length => _numberRef.GetImaginary();
	IPartialNumber Width => _numberRef.GetRational();
	public BinaryExpression? Area { get; }
	public BinaryExpression? AspectRatio { get; }
	public BinaryExpression? Perimeter { get; }
	public RectangleNumber(INumber num)
	{
		_numberRef = num;
		Area = new BinaryExpression(
			Length,
			Width,
			Ops.Multiply,
			null,
			CertaintyLevel.Derived);
		AspectRatio = new BinaryExpression(
			Length,
			Width,
			Ops.Divide,
			null,
			CertaintyLevel.Derived);
		Perimeter = new BinaryExpression(
			Length,
			Width,
			PerimeterOp,
			null,
			CertaintyLevel.Derived);
		Calculate();
	}
	public static BinaryOp PerimeterOp = (left, right) => left.Add(right).Multiply(CreateRational(2f));
}
