using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers;

// B | A
// D | C
public enum MeasureSlot
{
	Zero = 0,
	A = 1, B = 2, C = 3, D = 4,
	AB = 7, AC = 8, AD = 9,
	BA = 11, BC = 13, BD = 14,
	CA = 16, CB = 17, CD = 19,
	DA = 21, DB = 22, DC = 23,
	ABC = 38, ABD = 39, ACB = 42, ACD = 44, ADB = 47, ADC = 48,
	BAC = 58, BAD = 59, BCA = 66, BCD = 69, BDA = 71, BDC = 73,
	CAB = 82, CAD = 84, CBA = 86, CBD = 89, CDA = 96, CDB = 97,
	DAB = 107, DAC = 108, DBA = 111, DBC = 113, DCA = 116, DCB = 117,
	ABCD = 194, BCDA = 346, DABC = 538, DCBA = 586, CBAD = 434, BADC = 298,
	ABDC = 198, BACD = 294, ACBD = 214, DBCA = 566, ADBC = 238, CDBA = 486,
	ACDB = 222, ADCB = 242, BCAD = 334, BDAC = 358, BDCA = 366, CABD = 414,
	CADB = 422, CBDA = 446, CDAB = 482, DACB = 542, DBAC = 558, DCAB = 582,

	RightMeasure = A,
	LeftUnit = B,
	RightUnit = C,
	LeftMeasure = D,
	RealValue = AC,
	ImaginaryValue = DB,
	ComplexNumber = DBAC, // start -> end
	SymmetricNumber = ABCD, // dynamic units and measures
}
public static class MeasureSlotExtensions
{
	public static bool IsSingleValue(this MeasureSlot slot) => 
		slot >= MeasureSlot.Zero && slot <= MeasureSlot.LeftMeasure;

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



	private const int BASE = 5;

	public static bool ContainsA(this MeasureSlot kind) => HasDigit((int)kind, 1);
	public static bool ContainsB(this MeasureSlot kind) => HasDigit((int)kind, 2);
	public static bool ContainsC(this MeasureSlot kind) => HasDigit((int)kind, 3);
	public static bool ContainsD(this MeasureSlot kind) => HasDigit((int)kind, 4);

	private static bool HasDigit(int value, int targetDigit)
	{
		while (value > 0)
		{
			if (value % BASE == targetDigit) return true;
			value /= BASE;
		}
		return false;
	}
}
