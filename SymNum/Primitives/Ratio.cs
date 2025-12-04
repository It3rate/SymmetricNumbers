using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

// This is probably an INumber. If you add two ratios of different directions you get a complex number.
// A 'number' is one ratio connected to an opposite ratio (potentially with dynamic measures and equations).
// These ratios can be the same type numerically (depth<->height debt<->savings) or opposite types (sad<->happy, mad<->happy).
// The type conceptually starts at zero, but you can conceptually have negative depth (height) or negative savings (debt).

// This is different than 'Straps' that join two concepts by an equation, like color<->temperature.
// A color path goes from straw colored to red to blue to white hot, as the temp goes from 100 to 1000.
// They map all the way when solved, and the equations required to acheive this are the intelligence.
// The goal of prediction is the align two units with each other somehow.

public class Ratio : Number
{
	// Q. Could have one of numerator, denomonator or Value being calculated. Allows storing float as source of truth.
	public RefMeasure Numerator => IsImaginaryPart ? LeftMeasure : RightMeasure;
	public RefMeasure Denominator => IsImaginaryPart ? LeftUnit : RightUnit;
	public Polarity Polarity { get; }
	public float Value
	{
		get => GetValue(Numerator, Denominator);
		set => SetValue(Numerator, Denominator, value);
	}

	public bool IsRationalPart => Polarity.IsRationalPart();
	public bool IsImaginaryPart => Polarity.IsImaginaryPart();
	public float ValueOrOne => Denominator.Position != 0 && Numerator.Position != 0 ? Value : 1f;
	public void MapToValue(float value) => Numerator.SetPositionAndInvoke((long)(value * Denominator.Position));
	public float ValueForPolarity(Polarity polarity) => Polarity.IsMirror(polarity) ? -Value : Value;
	public Ratio(RefMeasure numerator, RefMeasure denominator, Polarity polarity = Polarity.FacesRight)
		: base(
			polarity.IsNotImaginary() ? numerator : Measure.Zero,
			polarity.IsNotImaginary() ? Measure.Zero : denominator,
			polarity.IsNotImaginary() ? denominator : Measure.Zero,
			polarity.IsNotImaginary() ? Measure.Zero : numerator
		)
	{
		Polarity = polarity;
	}
	//public Ratio(long numerator, long denominator, Polarity polarity = Polarity.FacesRight):
	//	this(new Measure(numerator), new Measure(denominator), polarity)
	//{
	//}
	//public Ratio(float value, long resolution, Polarity polarity = Polarity.FacesRight):
	//	this(new Measure(0), new Measure(resolution), polarity)
	//{
	//	Value = value;
	//}
	public void InvertPolarity()
	{
		Mirror();
	}
	private static long _highRes = 0x10000;
	//public static Ratio Zero => new Ratio(0, _highRes);
	//public static Ratio One => new Ratio(_highRes, _highRes);
	//public static Ratio HighResNumber(float num, Polarity polarity = Polarity.FacesRight) => new Ratio((long)(_highRes * num), _highRes, polarity);

	//public static float GetValue(Measure numerator, Measure denominator)
	//{
	//	var result = 0f;
	//	// todo: Need to apply the math of uncertaintly to generate a range or probability distribution.
	//	if (denominator.Position != 0)
	//	{
	//		result = numerator.Position / (float)denominator.Position;
	//	}
	//	else
	//	{
	//		result = numerator.Position;// > 0 ? float.MaxValue : (numerator.Position < 0 ? float.MinValue : 0f);
	//	}
	//	return result;
	//}
	//public static bool SetValue(Measure numerator, Measure denominator, float value)
	//{
	//	var result = false;
	//	if (numerator.IsFullDefined && denominator.IsFullDefined && denominator.Position != 0)
	//	{
	//		numerator.SetPosition((long)(value * denominator.Position));
	//		result = true;
	//	}
	//	return result;
	//}
}
