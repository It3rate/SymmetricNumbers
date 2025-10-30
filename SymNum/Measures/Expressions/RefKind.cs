using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

// B | A
// C | D
public enum RefKind
{
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

	TopRight = A,
	TopLeft = B,
	BottomRight = D,
	BottomLeft = C,
	RealValue = AD,
    ImaginaryValue = CB,
    ComplexNumber = CBAD, // start -> end
    SymmetricNumber = ABCD, // dynamic units and measures
}
public static class RefKindExtensions
{
	private const int BASE = 5;

	public static bool ContainsA(this RefKind kind) => HasDigit((int)kind, 1);
	public static bool ContainsB(this RefKind kind) => HasDigit((int)kind, 2);
	public static bool ContainsC(this RefKind kind) => HasDigit((int)kind, 3);
	public static bool ContainsD(this RefKind kind) => HasDigit((int)kind, 4);

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
//public enum RefKindx
//{
//	TopRight,
//	TopLeft,
//	BottomRight,
//	BottomLeft,
//	RealValue,
//	ImaginaryValue,
//	UnitAverage,
//	ValueAverage,
//	// others...
//}
