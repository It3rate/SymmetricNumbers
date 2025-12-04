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
    public ComplexNumber(Measure imaginaryMeasure, Measure rationalMeasure, Measure resolution) 
    {
        Measures[0] = rationalMeasure;
		Measures[1] = resolution; // this is 'negative' when interpreted in the rational perspective, but positive to itself.
		Measures[2] = imaginaryMeasure; // +leftunit/+leftmeasure is a positive imaginary number, but negative if converted to real.
		Measures[3] = resolution;  
    }
	public override string ToString()
	{
        return "{" + LeftValue.ToString("0.##") + "i, " + RightValue.ToString("0.##") + "}";
	}
}
