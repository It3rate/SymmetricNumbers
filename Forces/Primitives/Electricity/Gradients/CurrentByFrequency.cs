using SymNum.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Electricity.Gradients
{
    /// <summary>
    /// This is Impedance (Z), the total opposition to the flow of alternating current (AC) in a circuit, 
    /// combining both resistive and reactive effects.
    /// So the gradient is from no current to high current, but only for the AC part of the current.
    /// The opposite is Admittance (Y), how easily the AC current flows.
    /// </summary>
	public class CurrentByFrequency : SymmetricNumber
	{
		public List<CurrentByFrequency> Samples { get; } = new List<CurrentByFrequency>();
		public CurrentByFrequency(long currentMeasure, long frequencyUnit, long currentUnit, long frequencyMeasure) : 
			base(currentMeasure, frequencyUnit, currentUnit, frequencyMeasure)
		{
		}
	}
}
