using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public class ValueLong : IValue
{
	private long _longValue;
	public double Value
	{
		get => (double)_longValue;
		protected set => _longValue = (long)value;
	}
	public ValueLong(long value)
	{
		_longValue = value;
	}
}
