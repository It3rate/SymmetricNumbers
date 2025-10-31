using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;

public class ValueLong : IValue
{
	private long _longValue;
	public double Value
	{
		get => (double)_longValue;
		protected set => _longValue = (long)value;
	}
	public CertaintyLevel Certainty => CertaintyLevel.Fixed;
	public ValueLong(long value)
	{
		_longValue = value;
	}
}
