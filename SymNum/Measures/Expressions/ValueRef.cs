using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public abstract class ValueRef : ValueBase
{
	private UnconstrainedNumber _number;
	private int _measureIndex;
	public override double Value
	{
		get => (double)_number.MeasureFromIndex(_measureIndex).Position;
		protected set => _number.MeasureFromIndex(_measureIndex).SetPosition((long)value);
	}
	public ValueRef(UnconstrainedNumber number, int index)
	{
		_number = number;
		_measureIndex = index;
	}
}
