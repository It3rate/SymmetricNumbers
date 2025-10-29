using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public class ValueRef : IValue
{
	public UnconstrainedNumber RefNumber;
	public RefKind RefKind;
	public CertaintyLevel CertaintyLevel { get; set; } = CertaintyLevel.Set;
	public double Value
	{
		get => (double)RefNumber.ValueFromRefKind(RefKind).Value;
		set => RefNumber.SetValueFromRefKind(RefKind, value);
	}
	public ValueRef(UnconstrainedNumber number, RefKind refKind)
	{
		RefNumber = number;
		RefKind = refKind;
	}
}
