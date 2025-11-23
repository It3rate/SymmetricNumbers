using System;
namespace SymNum.Primitives;

public class Measure : MeasureBase
{
	public Measure(long position, CertaintyLevel certainty = CertaintyLevel.Uncertain, bool canBeNegative = true) : 
		base(position, certainty, canBeNegative)
	{ }
	public Measure(CertaintyLevel certainty = CertaintyLevel.Uncertain) : base(0, certainty) { }

	public Measure(long position, CertaintyLevel certainty, long min, long max) : 
		base(position, certainty, min, max)
	{ }
	public static Measure CreateUnit(long resolution)
	{
		return new Measure(resolution, CertaintyLevel.Set, false);
	}
}
public class MeasureFromRatio : MeasureBase
{
	public Ratio RatioRef { get; }
	public override long Position => (long)RatioRef.Value;
	public override void SetPosition(long position)
	{
		RatioRef.Value = (float)position;
	}
	public MeasureFromRatio(Ratio ratio)
	{
		RatioRef = ratio;
	}
}

public abstract class MeasureBase : IMeasure
{
	public static readonly IMeasure Unknown = new Measure(0);

	public event Action<IMeasure> PositionChanged;

	public virtual long Position { get; private set; }
    public virtual CertaintyLevel Certainty { get; set; }
	public long Minimum { get; set; } = long.MinValue;
	public long Maximum { get; set; } = long.MaxValue;

	public virtual bool IsFullDefined => !IsMin && !IsMax;
	public virtual bool IsMax => Position == long.MaxValue;
	public virtual bool IsMin => Position == long.MinValue;
	public virtual bool IsZero => Position == 0 && IsFullDefined;
	public virtual bool IsCertain => Certainty.HasValue();
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
	public virtual void SetPosition(long position)
	{
		ProposedPosition = position;
		Position = long.Clamp(position, Minimum, Maximum);
	}
	public virtual void SetPositionAndInvoke(long position)
	{
		SetPosition(position);
		InvokePositionChanged();
	}
	protected void InvokePositionChanged()
	{
		PositionChanged?.Invoke(this);
	}
	public void Increment() => SetPositionAndInvoke(Position + 1);
	public void Decrement() => SetPositionAndInvoke(Position - 1);

	public virtual void Negate()
    {
        SetPositionAndInvoke(-Position);
    }
    public static IMeasure MaxDown()
	{
		return IMeasure.CreateExtreme(true);
	}
	public static IMeasure MinUp()
	{
		return IMeasure.CreateExtreme(true);
	}
	public static IMeasure ZeroDown()
	{
		return new Measure(0);
	}
	public static IMeasure ZeroUnknown()
	{
		return new Measure(0);
	}
	public static IMeasure ZeroUp()
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
		return HashCode.Combine(Position, (int)Certainty, CanBeNegative);
	}
	public override string ToString()
	{
		string posStr = IsMax ? "Max" : IsMin ? "Min" : Position.ToString();
		return $"{posStr}";
	}
}
