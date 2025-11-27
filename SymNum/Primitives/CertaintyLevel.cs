
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

[Flags]
public enum CertaintyLevel
{
	Input,
	Output,
	Uncertain,
	Error,

	//Calibration,
	//ReadOnly,
	//Dormant,
	//UnderConstrained,
	//OverConstrained,
	//Partial,
	//Derived,
	//Perceived,
}

public static class CertaintyLevelExtensions
{
	public static bool IsInput(this CertaintyLevel state) => 
		(state & CertaintyLevel.Input) != 0;
}
