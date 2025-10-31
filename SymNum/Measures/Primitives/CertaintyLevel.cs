using SymNum.Measures.Numbers;
using SymNum.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Primitives;

public enum CertaintyLevel
{
	Dormant,
	Uncertain,
	UnderConstrained,
	OverConstrained,
	Partial,
	Derived,
	Set,
	Fixed,
	Error,
}

public static class CertaintyLevelExtensions
{
	public static bool HasValue(this CertaintyLevel state) => state == CertaintyLevel.Fixed || state == CertaintyLevel.Set;
	public static int SlotValue(this CertaintyLevel state, MultiplicativeRelation kind)
	{
		return state.HasValue() ? (int)kind : 0;
	}
}
