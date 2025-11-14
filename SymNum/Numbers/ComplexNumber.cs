using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Primitives;
namespace SymNum.Numbers;

/// <summary>
/// A ComplexNumber is made of two extents, where the denominator is a -1 to 1 segment,
/// and the numerator is a segment not fixed at zero.
/// </summary>
public class ComplexNumber : UnbalancedNumber
{
    protected ComplexNumber() { }
    public ComplexNumber(long startPosition, long endPosition, long resolution) : 
        base(endPosition, resolution, resolution, startPosition)
    { }
    public ComplexNumber(float startPosition, float endPosition, long resolution) :
        this((long)(startPosition * resolution), (long)(endPosition * resolution), resolution)
    { }
    public ComplexNumber(IMeasure imaginaryMeasure, IMeasure rationalMeasure, IMeasure resolution) 
    {
        RightUnit = resolution;
        RightMeasure = rationalMeasure;
        LeftUnit = resolution; // this is 'negative' when interpreted in the rational perspective, but positive to itself.
        LeftMeasure = imaginaryMeasure; // +leftunit/+leftmeasure is a positive imaginary number, but negative if converted to real.
    }
}
