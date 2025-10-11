namespace Forces.Primitives.Electricity.Momentum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SymNum.Measures.Numbers;

    public class Current : ComplexNumber 
    {
        public Current(long resolution, long startPosition, long endPosition) : base(resolution, startPosition, endPosition)
        {
        }
    }
}
