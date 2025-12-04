namespace SymNum.Numbers;

using System;
using System.Data;
using Microsoft.VisualBasic;
using SymNum.Primitives;

// **** perhaps there is no unbalanced version of complex numbers as they may not share a zero point.

// There needs to be interoporability between measures, Numbers, and Number equations.
// Gradients are about the 0-1 part of interpolation, but interpolation has that AND two endpoints.
// An UnbalancedNumber (or complex number) defines the two endpoints. The interpolation point can be the zero, or defined elsewhere.
// When merging resolutions into a complex number, you don't always pick the minimum. For example:
//  - if time is one component, you can move from seconds to miliseconds (you need to track the meaning of the resolution though)
//  - if you have samples per minute over hours, your resolution per minute is higher through statistics.

/// <summary>
/// An UnbalancedNumber is a complex number where the two units do no need to be the same resolution.
/// They still need to point in opposite directions, and the right side is still the real part.
/// </summary>
public class UnbalancedNumber : Number
{
    protected float _rationalValue => RightRatio.Value;
    protected float _imaginaryValue => LeftRatio.Value;
    public bool RealIsCertain => RightMeasure.IsInput && RightUnit.IsInput;
    public bool ImaginaryIsCertain => LeftMeasure.IsInput && LeftUnit.IsInput;

    public long MinResolution => Math.Min(LeftUnitPosition, RightUnitPosition);
    public long MaxResolution => Math.Max(LeftUnitPosition, RightUnitPosition);

    protected UnbalancedNumber() { }
    public UnbalancedNumber(long? rational, long? imaginaryResolution, long? rationalResolution, long? imaginary)
    {
        Measures[0] = new Measure(rational ?? 0);
		Measures[1] = new Measure(imaginaryResolution ?? 0);
		Measures[2] = new Measure(rationalResolution ?? 0);
		Measures[3] = new Measure(imaginary ?? 0);
        Initialize();
    }
    public UnbalancedNumber(Measure rational, Measure imaginaryResolution, Measure rationalResolution, Measure imaginary) :
        base(rational, imaginaryResolution, rationalResolution, imaginary)
    {
        Initialize();
	}
	public UnbalancedNumber(Ratio left, Ratio right) : base(left, right)
	{
		Initialize();
	}
	private void Initialize()
    {
    }
    /// <summary>
    /// Resets the value based on a discovered zero (may be an average or expectation), calibrating the measures as well as the two sides.
    /// The resolution can be manipulated to vary the 'importance' of each side assuming there are enough bits (lower denominator => higher value)
    /// </summary>
    public void Reset(long leftZero, long leftMeasure, long leftResolution, long rightZero, long rightMeasure, long rightResolution)
    {
        LeftMeasure.SetPositionAndInvoke(leftMeasure - leftZero);
        LeftUnit.SetPositionAndInvoke(leftResolution);
        RightMeasure.SetPositionAndInvoke(rightMeasure - rightZero);
        RightUnit.SetPositionAndInvoke(rightResolution);
    }

    public override ComplexNumber GetComplex()
    {
        var res = MinResolution;
        var real = (long)(_rationalValue * res);
        var imag = (long)(_imaginaryValue * res);
        return new ComplexNumber(imag, real, res);
    }

    // Need to allow setting by area or ratio by fixing real or imaginary parts.
    // Mimics setting voltage and power by knowing current and resistance,
    // or setting width and aspect ratio by knowing area and length, etc.
    //public Ratio Area => new Ratio((long)(_rationalValue * _imaginaryValue * MinResolution), MinResolution);
    //public Ratio RatioReal => new Ratio((long)((_rationalValue / _imaginaryValue) * MinResolution), MinResolution);
    //public Ratio RatioImaginary => new Ratio((long)((_rationalValue / _imaginaryValue) * MinResolution), MinResolution);

}


