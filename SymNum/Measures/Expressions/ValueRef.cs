using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public class ValueRef : IValue
{
	private UnconstrainedNumber _number;
	private RefKind _refKind;
	public CertaintyLevel CertaintyLevel { get; set; } = CertaintyLevel.Set;
	public double Value
	{
		get => (double)_number.ValueFromRefKind(_refKind).Value;
		set => _number.SetValueFromRefKind(_refKind, value);
	}
	public ValueRef(UnconstrainedNumber number, RefKind refKind)
	{
		_number = number;
		_refKind = refKind;
	}
}
