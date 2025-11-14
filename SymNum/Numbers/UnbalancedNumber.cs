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
public class UnbalancedNumber : RawNumber
{
    protected float _rationalValue => GetRational().Value;
    protected float _imaginaryValue => GetImaginary().Value;
    public bool RealIsCertain => RightMeasure.IsCertain && RightUnit.IsCertain;
    public bool ImaginaryIsCertain => LeftMeasure.IsCertain && LeftUnit.IsCertain;

    public long MinResolution => Math.Min(LeftUnitPosition, RightUnitPosition);
    public long MaxResolution => Math.Max(LeftUnitPosition, RightUnitPosition);

    protected UnbalancedNumber() { }
    public UnbalancedNumber(long? rational, long? imaginaryResolution, long? rationalResolution, long? imaginary)
    {
        RightMeasure = new Measure(rational ?? 0,
            (rational == null) ? CertaintyLevel.Uncertain : CertaintyLevel.Set);
        LeftUnit = new Measure(imaginaryResolution ?? 0,
            (imaginaryResolution == null) ? CertaintyLevel.Uncertain : CertaintyLevel.Set);
        RightUnit = new Measure(rationalResolution ?? 0,
            (rationalResolution == null) ? CertaintyLevel.Uncertain : CertaintyLevel.Set);
        LeftMeasure = new Measure(imaginary ?? 0,
            (imaginary == null) ? CertaintyLevel.Uncertain : CertaintyLevel.Set);
        Initialize();
    }
    public UnbalancedNumber(IMeasure rational, IMeasure imaginaryResolution, IMeasure rationalResolution, IMeasure imaginary)
    {
        RightMeasure = rational;
        LeftUnit = imaginaryResolution;
        RightUnit = rationalResolution;
        LeftMeasure = imaginary;
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
        LeftMeasure.SetPosition(leftMeasure - leftZero);
        LeftUnit.SetPosition(leftResolution);
        RightMeasure.SetPosition(rightMeasure - rightZero);
        RightUnit.SetPosition(rightResolution);
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
    public RationalNumber Area => new RationalNumber((long)(_rationalValue * _imaginaryValue * MinResolution), MinResolution);
    public RationalNumber RatioReal => new RationalNumber((long)((_rationalValue / _imaginaryValue) * MinResolution), MinResolution);
    public RationalNumber RatioImaginary => new RationalNumber((long)((_rationalValue / _imaginaryValue) * MinResolution), MinResolution);
    public static UnbalancedNumber Zero => new UnbalancedNumber(0, 1, 1, 0);
	public static UnbalancedNumber One => new UnbalancedNumber(1, 1, 1, 1);
}


