using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Patterns
{
	/// <summary>
	/// A ComplexNumber is made of two extents, where the denominator is a -1 to 1 segment,
	/// and the numerator is a segment not fixed at zero.
	/// </summary>
	public class ComplexNumber : NumberBase
    {
        public ComplexNumber(long resolution, long startValue, long endValue)
        {
            Numerator = new Extent(-resolution, endValue);
            Denominator = new Extent(startValue, resolution);
        }
        public ComplexNumber(long resolution, float startValue, float endValue) : this(resolution, (long)(startValue / resolution), (long)(endValue / resolution)) { }  
        public ComplexValue ComplexValue => new ComplexValue(this);
    }
    public class ComplexValue : NumberValue
    {
        public ComplexNumber Reference { get; }
        public float Real => Reference.UpValue;
        public float Imaginary => Reference.DownValue;
        public override long UpResolution => Reference.Denominator.EndValue;
        public override long DownResolution => UpResolution; // real and imaginary resolution are always the same in complex numbers.
        public ComplexValue(ComplexNumber complexNumber) : base(complexNumber)
        {
            Reference = complexNumber;
        }
    }
}
