using SymNum.Expressions;
using System;
namespace SymNum.Primitives;

public class Measure : IMeasure<long>
{
	public event Action<IMeasure<long>>? PositionChanged;

	public virtual long Position { get; private set; }
	public long NegativePosition => -Position;
	public Measure(long position)
	{
		SetPosition(position);
	}
	public virtual void SetPosition(long position)
	{
		Position = position;
	}
	public virtual void SetPositionAndInvoke(long position)
	{
		SetPosition(position);
		InvokePositionChanged();
	}
	public virtual int Dimensions => 1;
	public virtual int Direction => Math.Sign(Position);

	//public (float, float) ValueRange => (0, Position);
	//public (long, long) PositionRange => (0, Position);
	void OnUpdate(IMeasure<long> source)
	{
		var change = CalculateChange(source);
		UpdateState(change);
	}
	protected virtual float CalculateChange(IMeasure<long> source)
	{
		var dif = source.Position - Position;
		return dif;
	}
	protected virtual void UpdateState(float change)
	{
	}
	protected void InvokePositionChanged()
	{
		PositionChanged?.Invoke(this);
	}

	// +-*/^,mod ,bools, ==,!=,<,>,<=,>=
	public virtual int CompareTo(object? other)
	{
		if (other is IMeasure<long> otherMeasure)
		{
			return Position.CompareTo(otherMeasure.Position);
		}
		else if (other is IOrderable orderedMeasure)
		{
			return Dimensions.CompareTo(orderedMeasure?.Dimensions);
		}
		else
		{
			throw new ArgumentException("Object is not a Measure or IOrderable");
		}
	}

	public static bool operator ==(Measure a, Measure b)
	{
		return a.Position == b.Position;
	}
	public static bool operator !=(Measure a, Measure b)
	{
		return !(a == b);
	}
	public override bool Equals(object? obj)
	{
		if (obj is IMeasure<long> other)
		{
			return this == other;
		}
		return false;
	}
	public override int GetHashCode()
	{
		return HashCode.Combine(Position);
	}
	public override string ToString()
	{
		return $"m{Position}";
	}



	public static Measure Zero => new Measure(0);
	public static Measure One => new Measure(1);
}









/*

public abstract class MeasureBase : Measure
{
	//public static readonly Measure Unknown = new Measure(0);
 //   public virtual CertaintyLevel Certainty { get; set; }
	//public long Minimum { get; set; } = long.MinValue;
	//public long Maximum { get; set; } = long.MaxValue;

	//public virtual bool IsFullDefined => !IsMin && !IsMax;
	//public virtual bool IsMax => Position == long.MaxValue;
	//public virtual bool IsMin => Position == long.MinValue;
	//public virtual bool IsZero => Position == 0 && IsFullDefined;
	//public virtual bool IsCertain => Certainty.HasValue();
	/// <summary>
	/// Some measures are positive only. 
	/// Units are positive only (imaginary units will point left, but will still be positive).
	/// Also used when a value is unambigous with polarity, like not allowing negative debt, or negative credit (only debt or credit)
	/// Used when you need one calculation above zero, and another below zero, like accumulating speed on a sine wave.
	/// </summary>
	public virtual bool CanBeNegative
	{
		get => Minimum < 0;
		set
		{
			Minimum = !value && Minimum > 0 ? Minimum : 0;
			SetPosition(Position); // re-clamp position if needed
		}
	}

	protected MeasureBase()
	{
	}
	public MeasureBase(long position, CertaintyLevel certainty = CertaintyLevel.Set, bool canBeNegative = true)
	{
		Certainty = certainty;
		Minimum = canBeNegative ? long.MinValue : 0;
		SetPosition(position);
	}
	public MeasureBase(long position, CertaintyLevel certainty, long min, long max)
	{
		Certainty = certainty;
		Minimum = min;
		Maximum = max;
		SetPosition(position);
	}
	public long ProposedPosition { get; private set; }
	public void Increment() => SetPositionAndInvoke(Position + 1);
	public void Decrement() => SetPositionAndInvoke(Position - 1);

	public virtual void Negate()
    {
        SetPositionAndInvoke(-Position);
    }
    public static Measure MaxDown()
	{
		return Measure.CreateExtreme(true);
	}
	public static Measure MinUp()
	{
		return Measure.CreateExtreme(true);
	}
	public static Measure ZeroDown()
	{
		return new Measure(0);
	}
	public static Measure ZeroUnknown()
	{
		return new Measure(0);
	}
	public static Measure ZeroUp()
	{
		return new Measure(0);
	}
	public static bool operator ==(MeasureBase a, MeasureBase b)
	{
		return a.Position == b.Position;
	}
	public static bool operator !=(MeasureBase a, MeasureBase b)
	{
		return !(a == b);
	}

	public static Measure Min(Measure a, Measure b) => a.Position < b.Position ? a : b;
	public static Measure Max(Measure a, Measure b) => a.Position >= b.Position ? a : b;
	public static long MinPosition(Measure a, Measure b) => Math.Min(a.Position, b.Position);
	public static long MaxPosition(Measure a, Measure b) => Math.Max(a.Position, b.Position);
	//Unary operators: +, -, !, ~, ++, --, true, false. The true and false operators must be overloaded together.
	//Binary operators: +, -, *, /, %, &, |, ^, <<, >>, >>>. These can be overloaded to perform custom operations on user-defined types.
	//Equality and comparison operators: ==, !=, <, >, <=, >=. These must be overloaded in pairs (e.g., == and !=, < and >, <= and >=).
	//Compound assignment operators: +=, -=, *=, /=, %=, &=, |=, ^=, <<=, >>=, >>>=.


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
		return HashCode.Combine(Position, (int)Certainty, CanBeNegative);
	}
	public override string ToString()
	{
		string posStr = IsMax ? "Max" : IsMin ? "Min" : Position.ToString();
		return $"{posStr}";
	}
}
*/