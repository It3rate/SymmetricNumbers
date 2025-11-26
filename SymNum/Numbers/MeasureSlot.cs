using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers;

public enum MeasureSlot
{
	RightMeasure = 0,
	LeftUnit = 1,
	RightUnit = 2,
	LeftMeasure = 3
}
public static class MeasureSlotExtensions
{
	public static MeasureSlot GetOppositeMeasureSlot(this MeasureSlot slot)
	{
		return slot switch
		{
			MeasureSlot.RightMeasure => MeasureSlot.LeftMeasure,
			MeasureSlot.LeftMeasure => MeasureSlot.RightMeasure,
			MeasureSlot.LeftUnit => MeasureSlot.RightUnit,
			MeasureSlot.RightUnit => MeasureSlot.LeftUnit,
			_ => throw new NotImplementedException(),
		};
	}
	public static bool IsLeft(this MeasureSlot slot) => slot == MeasureSlot.LeftMeasure || slot == MeasureSlot.LeftUnit;
	public static bool IsRight(this MeasureSlot slot) => slot == MeasureSlot.RightMeasure || slot == MeasureSlot.RightUnit;
}
