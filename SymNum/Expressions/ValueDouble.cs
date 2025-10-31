using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Primitives;
namespace SymNum.Expressions;

public class ValueDouble : IValue
{
	private double _doubleValue;
	public double Value
	{
		get => _doubleValue;
		protected set => _doubleValue = value;
	}
	public CertaintyLevel Certainty => CertaintyLevel.Fixed;
	public ValueDouble(double value)
	{
		_doubleValue = value;
	}
}
