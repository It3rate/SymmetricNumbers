using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Expressions;
namespace SymNum.Primitives;

// Requirement: Groupings of 'IMeasure' need to be able to smoothly move between bools, enums, ints, floats, complex nums, symmetric numbs, equations, and probabilities.
//    There are a lot of types on numbers amidst these, but that is the general trend.
//    All of these can be represented by four Extents, a zero, and reptition (and primitive ops)

public interface IMeasure : IValue
{
	long Position { get; }
	void SetPosition(long position);

	public event Action<IMeasure> PositionChanged;
	long ProposedPosition { get; }
	void SetPositionAndInvoke(long position);
	long Minimum { get; set; } // does measure have a min max?
	long Maximum { get; set; }
	public bool CanBeNegative { get; set; }
	void Negate();
    //public CertaintyLevel CertaintyLevel { get; } // inherited
    public bool IsCertain { get; }
	bool IsFullDefined => !IsMin && !IsMax;
	bool IsMax => Position == long.MaxValue;
	bool IsMin => Position == long.MinValue;
	bool IsZero => Position == 0 && IsFullDefined;
	public static IMeasure Zero => new Measure(0);
	public static IMeasure One => new Measure(1);
	public static IMeasure CreateExtreme(bool positive)
	{
		long pos = positive ? long.MaxValue : long.MinValue;
		return new Measure(pos);
	}
	public static IMeasure CreateUnknown()
	{
		return new Measure(0);
	}
	public void Increment();
	public void Decrement();
	public static IMeasure operator -(IMeasure a)
	{
		var position = a.IsMin ? long.MaxValue : a.IsMax ? long.MinValue : -a.Position;
		return new Measure(position);
	}
	public static IMeasure operator ++(IMeasure a)
	{
		var result = a;
		if (a.IsFullDefined)
		{
			result = new Measure(a.Position + 1);
		}
		return result;
	}
	public static IMeasure operator --(IMeasure a)
	{
		var result = a;
		if (a.IsFullDefined)
		{
			result = new Measure(a.Position - 1);
		}
		return result;
	}
	//public abstract static bool operator ==(IMeasure a, IMeasure b);
	//public abstract static bool operator !=(IMeasure a, IMeasure b);

	public static bool operator >(IMeasure a, IMeasure b)
	{
		return a.Position > b.Position;
	}
	public static bool operator <(IMeasure a, IMeasure b)
	{
		return a.Position < b.Position;
	}
	public static bool operator >=(IMeasure a, IMeasure b)
	{
		return a.Position >= b.Position;
	}
	public static bool operator <=(IMeasure a, IMeasure b)
	{
		return a.Position <= b.Position;
	}

	public static IMeasure operator +(IMeasure a, IMeasure b)
	{

		if (a.IsMax || b.IsMax)
		{
			if (a.IsMin || b.IsMin)
			{
				return CreateUnknown();
			}
			return CreateExtreme(true);
		}

		if (a.IsMin && b.IsMin)
		{
			return CreateExtreme(false);
		}

		// Both finite
		long newPosition;
		if (a.Position > 0 && b.Position > 0 && a.Position > long.MaxValue - b.Position)
		{
			newPosition = long.MaxValue;
		}
		else if (a.Position < 0 && b.Position < 0 && a.Position < long.MinValue - b.Position)
		{
			newPosition = long.MinValue;
		}
		else
		{
			newPosition = a.Position + b.Position;
		}

		return new Measure(newPosition);
	}

	public static IMeasure operator -(IMeasure a, IMeasure b)
	{
		if (a.IsMax)
		{
			if (b.IsMax)
			{
				return CreateUnknown();
			}
			return CreateExtreme(true);
		}

		if (a.IsMin)
		{
			if (b.IsMin)
			{
				return CreateUnknown();
			}
			return CreateExtreme(false);
		}

		if (b.IsMax)
		{
			return CreateExtreme(false);
		}

		if (b.IsMin)
		{
			return CreateExtreme(true);
		}

		// Both finite
		long newPosition;
		bool bNegative = b.Position < 0;
		if (a.Position > 0 && bNegative && a.Position > long.MaxValue + b.Position)
		{
			newPosition = long.MaxValue;
		}
		else if (a.Position < 0 && !bNegative && a.Position < long.MinValue + b.Position)
		{
			newPosition = long.MinValue;
		}
		else
		{
			newPosition = a.Position - b.Position;
		}

		return new Measure(newPosition);
	}

	public static IMeasure operator *(IMeasure a, IMeasure b)
	{
		if (a.Position == 0 || b.Position == 0)
		{
			return new Measure(0);
		}

		bool aInf = a.IsMax || a.IsMin;
		bool bInf = b.IsMax || b.IsMin;
		if (aInf || bInf)
		{
			bool signA = a.IsMax || a.Position > 0;
			bool signB = b.IsMax || b.Position > 0;
			bool productPositive = signA == signB;
			return CreateExtreme(productPositive);
		}

		// Both finite and nonzero
		long absA = Math.Abs(a.Position);
		long absB = Math.Abs(b.Position);
		bool sameSign = a.Position > 0 == b.Position > 0;

		long newPosition;
		if (absA > long.MaxValue / absB)
		{
			newPosition = sameSign ? long.MaxValue : long.MinValue;
		}
		else
		{
			newPosition = a.Position * b.Position;
		}

		return new Measure(newPosition);
	}

	public static IMeasure operator /(IMeasure a, IMeasure b)
	{

		bool bInf = b.IsMax || b.IsMin;
		bool aInf = a.IsMax || a.IsMin;
		if (bInf)
		{
			return new Measure(0);
		}

		if (aInf)
		{
			bool signA = a.IsMax || a.Position > 0;
			bool signB = b.Position > 0;
			bool productPositive = signA == signB;  // Division sign: same as multiplication by reciprocal, but same rule as mul for signs
			return CreateExtreme(productPositive);
		}

		if (b.Position == 0)
		{
			// Zero means below resolution, so dividing say 40/0 means greater than 40, but less than max.
			// at some point there may need to be +0 and -0, but for now the sign comes from the numerator.
			return new Measure(a.Position);
		}

		// Both finite, nonzero denominator
		long newPosition = a.Position / b.Position;
		return new Measure(newPosition);
	}

}
