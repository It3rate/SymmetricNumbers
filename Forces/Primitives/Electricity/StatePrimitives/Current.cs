namespace Forces.Primitives.Electricity.Momentum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public class Current : ComplexNumber 
    {
        public Current(long startPosition, long endPosition, long resolution) : base(startPosition, endPosition, resolution)
        {
        }
    }
}
