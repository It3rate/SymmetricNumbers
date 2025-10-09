using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public class ValueDouble : IValue
{
	private double _doubleValue;
	public double Value
	{
		get => _doubleValue;
		protected set => _doubleValue = value;
	}
	public CertaintyLevel CertaintyLevel => CertaintyLevel.Fixed;
	public ValueDouble(double value)
	{
		_doubleValue = value;
	}
}
