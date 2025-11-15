using SymNum.Expressions;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Numbers.Common;

public class RectangleNumber : ComplexNumber
{
	private INumber _numberRef;
	public override IMeasure[] Measures => _numberRef.Measures;	
	Ratio Length => _numberRef.GetImaginary();
	Ratio Width => _numberRef.GetRational();
	public BinaryExpression? Area { get; }
	public BinaryExpression? AspectRatio { get; }
	public BinaryExpression? Perimeter { get; }
	public RectangleNumber(INumber num)
	{
		_numberRef = num;
		Area = new BinaryExpression(
			Length,
			Width,
			Ops.MultiplyTo,
			null,
			CertaintyLevel.Derived);
		AspectRatio = new BinaryExpression(
			Length,
			Width,
			Ops.DivideTo,
			null,
			CertaintyLevel.Derived);
		Perimeter = new BinaryExpression(
			Length,
			Width,
			PerimeterOp,
			null,
			CertaintyLevel.Derived);
	}
	public static BinaryOp PerimeterOp = (left, right) => left.AddTo(right).MultiplyTo(Ratio.HighResNumber(2f, Polarity.FacesRight));
}
