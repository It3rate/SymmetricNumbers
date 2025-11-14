using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

public class Ratio
{
	public IMeasure Numerator { get; }
	public IMeasure Denominator { get; }
	public Direction Polarity { get; }
	public float Value
	{
		get => GetValue(Numerator, Denominator);
		set => SetValue(Numerator, Denominator, value);
	}
	public float ValueOrOne => Denominator.Position != 0 && Numerator.Position != 0 ? Value : 1f;
	public void MapToValue(float value) => Numerator.SetPosition((long)(value * Denominator.Position));
	public float ValueForPolarity(Direction polarity) => Polarity.IsMirror(polarity) ? -Value : Value; 
	public Ratio(IMeasure numerator, IMeasure denominator, Direction polarity = Direction.FacesRight)
	{
		Numerator = numerator;
		Denominator = denominator;
		Polarity = polarity;
	}
	public float GetValue(IMeasure numerator, IMeasure denominator)
	{
		var result = 0f;
		// todo: Need to apply the math of uncertaintly to generate a range or probability distribution.
		if (denominator.Position != 0)
		{
			result = numerator.Position / (float)denominator.Position;
		}
		else
		{
			result = numerator.Position;// > 0 ? float.MaxValue : (numerator.Position < 0 ? float.MinValue : 0f);
		}
		return result;
	}
	public static bool SetValue(IMeasure numerator, IMeasure denominator, float value)
	{
		var result = false;
		if (numerator.IsFullDefined && denominator.IsFullDefined && denominator.Position != 0)
		{
			numerator.SetPosition((long)(value * denominator.Position));
			result = true;
		}
		return result;
	}
}
