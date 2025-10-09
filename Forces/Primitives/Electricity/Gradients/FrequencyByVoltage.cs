using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Electricity.Gradients
{
    public class FrequencyByVoltage : SymmetricNumber
	{
		public List<FrequencyByVoltage> Samples { get; } = new List<FrequencyByVoltage>();
		public FrequencyByVoltage(long frequencyMeasure, long voltageUnit, long frequencyUnit, long voltageMeasure) :
			base(frequencyMeasure, voltageUnit, frequencyUnit, voltageMeasure)
		{
		}
	}
}
