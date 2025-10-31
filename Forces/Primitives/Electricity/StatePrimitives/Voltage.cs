namespace Forces.Primitives.Electricity.Momentum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public class Voltage : ComplexNumber
    {
        public Voltage(long startPosition, long endPosition, long resolution) : base(startPosition, endPosition, resolution)
        {
        }
    }
}
