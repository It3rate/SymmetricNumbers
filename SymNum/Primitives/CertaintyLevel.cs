
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

[Flags]
public enum CertaintyLevel
{
	Set,
	ReadOnly,
	Uncertain,
	Error,

	//Dormant,
	//UnderConstrained,
	//OverConstrained,
	//Partial,
	//Derived,
	//Perceived,
}

public static class CertaintyLevelExtensions
{
	public static bool HasValue(this CertaintyLevel state) => 
		((state & CertaintyLevel.Set) | (state & CertaintyLevel.ReadOnly)) != 0;
}
