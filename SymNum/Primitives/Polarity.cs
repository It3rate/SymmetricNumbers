using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

public enum Polarity { Unknown, Zero, FacesRight, FacesLeft };//, Zero, Max }

public static class PolarityExtension
{
	public static bool HasPolarity(this Polarity polarity) => polarity == Polarity.FacesRight || polarity == Polarity.FacesLeft;
	public static bool IsTrue(this Polarity polarity) => polarity == Polarity.FacesRight;
	public static bool IsFalse(this Polarity polarity) => polarity == Polarity.FacesLeft;
	public static int Direction(this Polarity polarity) => polarity == Polarity.FacesRight ? 1 : polarity == Polarity.FacesLeft ? -1 : 0;
	public static int ForceValue(this Polarity polarity) => polarity == Polarity.FacesLeft ? -1 : 1;

	public static Polarity Invert(this Polarity polarity)//Inverse
	{
		return polarity switch
		{
			Polarity.FacesRight => Polarity.FacesLeft,
			Polarity.FacesLeft => Polarity.FacesRight,
			_ => polarity
		};
	}
	public static bool IsMirror(this Polarity a, Polarity b)
	{
		return (a == Polarity.FacesRight && b == Polarity.FacesLeft) ||
			 (b == Polarity.FacesRight && a == Polarity.FacesLeft);
	}
}