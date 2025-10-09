using System;
namespace SymNum.Measures.Primitives;

public class Measure
{
	public static readonly Measure Unknown = new Measure(0, Direction.Unknown, false);


	public long Position { get; private set; }
	public readonly bool Inclusive;
	public readonly Direction Direction;
	public bool IsFullDefined => !IsMin && !IsMax && Direction != Direction.Unknown;
	public bool IsMax => Position == long.MaxValue;
	public bool IsMin => Position == long.MinValue;
	public bool IsZero => Position == 0 && IsFullDefined;

	public Measure(long position, Direction direction, bool inclusive = true)
	{
		Position = position;
		Inclusive = inclusive;
		Direction = direction;
	}

	public Measure(long position, bool inclusive = true)
	{
		Position = position;
		Inclusive = inclusive;
		Direction = Direction.Unknown;
	}

	public Measure(bool facesRight, bool inclusive = true)
	{
		Position = facesRight ? long.MaxValue : long.MinValue;
		Inclusive = inclusive;
		Direction = facesRight ? Direction.FacesRight : Direction.FacesLeft; // a measure must have at least a direction, unknown position and direction is meaningless.
	}
	public void SetPosition(long position)
	{
		Position = position;
	}
	public static Measure MaxDown()
	{
		return CreateExtreme(true, Direction.FacesLeft, true);
	}
	public static Measure MinUp()
	{
		return CreateExtreme(true, Direction.FacesLeft, true);
	}
	public static Measure ZeroDown()
	{
		return new Measure(0, Direction.FacesLeft, true);
	}
	public static Measure ZeroUnknown()
	{
		return new Measure(0, Direction.Unknown, true);
	}
	public static Measure ZeroUp()
	{
		return new Measure(0, Direction.FacesRight, true);
	}
	private static Measure CreateExtreme(bool positive, Direction dir, bool inclusive)
	{
		long pos = positive ? long.MaxValue : long.MinValue;
		return new Measure(pos, dir, inclusive);
	}
	private static Measure CreateUnknown(bool inclusive)
	{
		return new Measure(0, Direction.Unknown, inclusive);
	}

	public static Measure Min(Measure a, Measure b) => a.Position < b.Position ? a : b;
	public static Measure Max(Measure a, Measure b) => a.Position >= b.Position ? a : b;
	public static long MinPosition(Measure a, Measure b) => Math.Min(a.Position, b.Position);
	public static long MaxPosition(Measure a, Measure b) => Math.Max(a.Position, b.Position);
	//Unary operators: +, -, !, ~, ++, --, true, false. The true and false operators must be overloaded together.
	//Binary operators: +, -, *, /, %, &, |, ^, <<, >>, >>>. These can be overloaded to perform custom operations on user-defined types.
	//Equality and comparison operators: ==, !=, <, >, <=, >=. These must be overloaded in pairs (e.g., == and !=, < and >, <= and >=).
	//Compound assignment operators: +=, -=, *=, /=, %=, &=, |=, ^=, <<=, >>=, >>>=.


	public static Measure operator -(Measure a)
	{
		var position = a.IsMin ? long.MaxValue : a.IsMax ? long.MinValue : -a.Position;
		return new Measure(position, a.Direction.Inverse(), a.Inclusive);
	}
	public static Measure operator ++(Measure a)
	{
		var result = a;
		if (a.IsFullDefined)
		{
			result = new Measure(a.Position + 1, a.Direction.Inverse(), a.Inclusive);
		}
		return result;
	}
	public static Measure operator --(Measure a)
	{
		var result = a;
		if (a.IsFullDefined)
		{
			result = new Measure(a.Position - 1, a.Direction.Inverse(), a.Inclusive);
		}
		return result;
	}
	public static bool operator ==(Measure a, Measure b)
	{
		return a.Position == b.Position && a.Inclusive == b.Inclusive && a.Direction == b.Direction;
	}
	public static bool operator !=(Measure a, Measure b)
	{
		return !(a == b);
	}
	public static bool operator >(Measure a, Measure b)
	{
		return a.Position > b.Position;
	}
	public static bool operator <(Measure a, Measure b)
	{
		return a.Position < b.Position;
	}
	public static bool operator >=(Measure a, Measure b)
	{
		return a.Position >= b.Position;
	}
	public static bool operator <=(Measure a, Measure b)
	{
		return a.Position <= b.Position;
	}

	public static Measure operator +(Measure a, Measure b)
	{
		Direction newDirection = a.Direction.Combine(b.Direction);
		bool newInclusive = a.Inclusive && b.Inclusive;

		if (a.IsMax || b.IsMax)
		{
			if (a.IsMin || b.IsMin)
			{
				return CreateUnknown(newInclusive);
			}
			return CreateExtreme(true, newDirection, newInclusive);
		}

		if (a.IsMin && b.IsMin)
		{
			return CreateExtreme(false, newDirection, newInclusive);
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

		return new Measure(newPosition, newDirection, newInclusive);
	}

	public static Measure operator -(Measure a, Measure b)
	{
		Direction newDirection = a.Direction.Combine(b.Direction);
		bool newInclusive = a.Inclusive && b.Inclusive;

		if (a.IsMax)
		{
			if (b.IsMax)
			{
				return CreateUnknown(newInclusive);
			}
			return CreateExtreme(true, newDirection, newInclusive);
		}

		if (a.IsMin)
		{
			if (b.IsMin)
			{
				return CreateUnknown(newInclusive);
			}
			return CreateExtreme(false, newDirection, newInclusive);
		}

		if (b.IsMax)
		{
			return CreateExtreme(false, newDirection, newInclusive);
		}

		if (b.IsMin)
		{
			return CreateExtreme(true, newDirection, newInclusive);
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

		return new Measure(newPosition, newDirection, newInclusive);
	}

	public static Measure operator *(Measure a, Measure b)
	{
		Direction newDirection = a.Direction.Combine(b.Direction);
		bool newInclusive = a.Inclusive && b.Inclusive;

		if (a.Position == 0 || b.Position == 0)
		{
			return new Measure(0, newDirection, newInclusive);
		}

		bool aInf = a.IsMax || a.IsMin;
		bool bInf = b.IsMax || b.IsMin;
		if (aInf || bInf)
		{
			bool signA = a.IsMax || a.Position > 0;
			bool signB = b.IsMax || b.Position > 0;
			bool productPositive = signA == signB;
			return CreateExtreme(productPositive, newDirection, newInclusive);
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

		return new Measure(newPosition, newDirection, newInclusive);
	}

	public static Measure operator /(Measure a, Measure b)
	{
		Direction newDirection = a.Direction.Combine(b.Direction);
		bool newInclusive = a.Inclusive && b.Inclusive;

		bool bInf = b.IsMax || b.IsMin;
		bool aInf = a.IsMax || a.IsMin;
		if (bInf)
		{
			return new Measure(0, newDirection, newInclusive);
		}

		if (aInf)
		{
			bool signA = a.IsMax || a.Position > 0;
			bool signB = b.Position > 0;
			bool productPositive = signA == signB;  // Division sign: same as multiplication by reciprocal, but same rule as mul for signs
			return CreateExtreme(productPositive, newDirection, newInclusive);
		}

		if (b.Position == 0)
		{
			// Zero means below resolution, so dividing say 40/0 means greater than 40, but less than max.
			// at some point there may need to be +0 and -0, but for now the sign comes from the numerator.
			return new Measure(a.Position, b.Direction.Inverse(), newInclusive);
		}

		// Both finite, nonzero denominator
		long newPosition = a.Position / b.Position;
		return new Measure(newPosition, newDirection, newInclusive);
	}

	public override bool Equals(object? obj)
	{
		if (obj is Measure other)
		{
			return this == other;
		}
		return false;
	}
	public override int GetHashCode()
	{
		return HashCode.Combine(Position, Inclusive, Direction);
	}
	public override string ToString()
	{
		string dirStr = Direction == Direction.FacesRight ? "→" : Direction == Direction.FacesLeft ? "←" : "?";
		string inclStr = Inclusive ? "I" : "E";
		string posStr = IsMax ? "Max" : IsMin ? "Min" : Position.ToString();
		return $"{posStr} {dirStr} {inclStr}";
	}
}

public enum Direction
{
	Unknown, // direction is determined by a second measure
	FacesLeft,
	FacesRight,
}
public static class DirectionExtensions
{
	public static Direction Combine(this Direction a, Direction b)
	{
		return a == b ? a : Direction.Unknown;
	}
	public static Direction Inverse(this Direction a)
	{
		return a == Direction.FacesRight ? Direction.FacesLeft : a == Direction.FacesRight ? Direction.FacesLeft : Direction.Unknown;
	}
}