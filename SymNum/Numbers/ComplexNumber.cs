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
    //public RationalNumber RealPart { get; }
    protected ComplexNumber() { }
    public ComplexNumber(long startPosition, long endPosition, long resolution) : 
        base(endPosition, resolution, resolution, startPosition)
    {
        //RightUnit = new Measure(resolution, Direction.FacesLeft, true);
        //RightMeasure = new Measure(endPosition, Direction.Unknown, true);
        //LeftUnit = new Measure(-resolution, Direction.FacesRight, true);
        //LeftMeasure = new Measure(startPosition, Direction.Unknown, true);
    }
    public ComplexNumber(float startPosition, float endPosition, long resolution) :
        this(resolution, (long)(startPosition / resolution), (long)(endPosition / resolution))
    { }
    public ComplexNumber(IMeasure imaginaryMeasure, IMeasure rationalMeasure, IMeasure resolution) 
    {
        RightUnit = resolution;
        RightMeasure = rationalMeasure;
        LeftUnit = resolution;
        LeftMeasure = imaginaryMeasure;
    }
    public ComplexValue Complex => new ComplexValue(this);
    public class ComplexValue : NumberValue
    {
        public ComplexNumber Reference { get; }
        public float Real => Reference.RightValue;
        public float Imaginary => Reference.LeftValue;
        public override long RightResolution => Reference.LeftUnit.Position;
        public override long LeftResolution => RightResolution; // real and imaginary resolution are always the same in complex numbers.
        public ComplexValue(ComplexNumber complexNumber) : base(complexNumber)
        {
            Reference = complexNumber;
        }
    }
}
