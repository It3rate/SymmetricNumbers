namespace Forces.Primitives.Electricity.Momentum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SymNum.Numbers;

    /// <summary>
    /// Momentum values are stand alone, so a frequency gradient from no frequency to max frequency.
    /// </summary>
    public class Frequency : ComplexNumber
    {
        public Frequency(long startPosition, long endPosition, long resolution) : base(startPosition, endPosition, resolution)
		{
        }
    }
}
