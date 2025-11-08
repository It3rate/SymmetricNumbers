using System;
namespace SymNum.Primitives;

public class Measure : MeasureBase
{
	public Measure(long position,
		bool inclusive = true, CertaintyLevel certainty = CertaintyLevel.Uncertain) :
				base(position, inclusive, certainty)
	{
	}


	//public Measure(long position, bool inclusive = true)
	//{
	//	Position = position;
	//	Inclusive = inclusive;
	//	Direction = Direction.Unknown;
	//}

	//public Measure(bool facesRight, bool inclusive = true)
	//{
	//	Position = facesRight ? long.MaxValue : long.MinValue;
	//	Inclusive = inclusive;
	//	Direction = facesRight ? Direction.FacesRight : Direction.FacesLeft; // a measure must have at least a direction, unknown position and direction is meaningless.
	//}
}
public abstract class MeasureBase : IMeasure
{
	public static readonly IMeasure Unknown = new Measure(0, false);

	public event Action<IMeasure> PositionChanged;

	public virtual long Position { get; protected set; }
	public virtual bool Inclusive { get; protected set; }
	public virtual bool IsFullDefined => !IsMin && !IsMax;
	public virtual bool IsMax => Position == long.MaxValue;
	public virtual bool IsMin => Position == long.MinValue;
	public virtual bool IsZero => Position == 0 && IsFullDefined;
	public virtual bool IsCertain => Certainty.HasValue();
	public virtual double Value
	{
		get => (double)Position;
		protected set => Position = (long)value;
	}
    public virtual CertaintyLevel Certainty { get; set; }

	protected MeasureBase()
	{
	}
	public MeasureBase(long position, bool inclusive = true, CertaintyLevel certainty = CertaintyLevel.Set)
	{
		Position = position;
		Inclusive = inclusive;
		Certainty = certainty;
	}


    public virtual void SetPosition(long position)
	{
		Position = position;
		PositionChanged?.Invoke(this);
    }
    public virtual void Negate()
    {
        SetPosition(-Position);
    }
    public static IMeasure MaxDown()
	{
		return IMeasure.CreateExtreme(true, true);
	}
	public static IMeasure MinUp()
	{
		return IMeasure.CreateExtreme(true, true);
	}
	public static IMeasure ZeroDown()
	{
		return new Measure(0, true);
	}
	public static IMeasure ZeroUnknown()
	{
		return new Measure(0, true);
	}
	public static IMeasure ZeroUp()
	{
		return new Measure(0, true);
	}
	public static bool operator ==(MeasureBase a, MeasureBase b)
	{
		return a.Position == b.Position && a.Inclusive == b.Inclusive;
	}
	public static bool operator !=(MeasureBase a, MeasureBase b)
	{
		return !(a == b);
	}

	public static IMeasure Min(IMeasure a, IMeasure b) => a.Position < b.Position ? a : b;
	public static IMeasure Max(IMeasure a, IMeasure b) => a.Position >= b.Position ? a : b;
	public static long MinPosition(IMeasure a, IMeasure b) => Math.Min(a.Position, b.Position);
	public static long MaxPosition(IMeasure a, IMeasure b) => Math.Max(a.Position, b.Position);
	//Unary operators: +, -, !, ~, ++, --, true, false. The true and false operators must be overloaded together.
	//Binary operators: +, -, *, /, %, &, |, ^, <<, >>, >>>. These can be overloaded to perform custom operations on user-defined types.
	//Equality and comparison operators: ==, !=, <, >, <=, >=. These must be overloaded in pairs (e.g., == and !=, < and >, <= and >=).
	//Compound assignment operators: +=, -=, *=, /=, %=, &=, |=, ^=, <<=, >>=, >>>=.


	public override bool Equals(object? obj)
	{
		if (obj is IMeasure other)
		{
			return this == other;
		}
		return false;
	}
	public override int GetHashCode()
	{
		return HashCode.Combine(Position, Inclusive);
	}
	public override string ToString()
	{
		string inclStr = Inclusive ? "In" : "Ex";
		string posStr = IsMax ? "Max" : IsMin ? "Min" : Position.ToString();
		return $"{posStr} {inclStr}";
	}
}

public enum Direction
{
    Unknown, // direction is determined by a second measure
    Zero,
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