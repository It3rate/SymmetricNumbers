using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Numbers;

namespace Forces.Primitives.Electricity.Gradients
{
    public class VoltageByFrequency : SymmetricNumber
	{
		public List<VoltageByFrequency> Samples { get; } = new List<VoltageByFrequency>();
		public VoltageByFrequency(long voltageMeasure, long frequencyUnit, long voltageUnit, long frequencyMeasure) :
			base(voltageMeasure, frequencyUnit, voltageUnit, frequencyMeasure)
		{
		}
	}
}
