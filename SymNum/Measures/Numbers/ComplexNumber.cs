using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Numbers;

/// <summary>
/// A ComplexNumber is made of two extents, where the denominator is a -1 to 1 segment,
/// and the numerator is a segment not fixed at zero.
/// </summary>
public class ComplexNumber : UnconstrainedNumber
{
    public ComplexNumber(long resolution, long startPosition, long endPosition)
    {
        RightUnit = new Measure(resolution, Direction.FacesLeft, true);
        RightMeasure = new Measure(endPosition, Direction.Unknown, true);
        LeftUnit = new Measure(-resolution, Direction.FacesRight, true);
        LeftMeasure = new Measure(startPosition);
    }
    public ComplexNumber(long resolution, float startPosition, float endPosition) :
        this(resolution, (long)(startPosition / resolution), (long)(endPosition / resolution))
    { }
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
