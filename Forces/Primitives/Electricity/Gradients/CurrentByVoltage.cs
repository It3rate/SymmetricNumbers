using SymNum.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Electricity.Gradients
{
	public class CurrentByVoltage : SymmetricNumber
	{
		public List<CurrentByVoltage> Samples { get; } = new List<CurrentByVoltage>();
		public CurrentByVoltage(long currentMeasure, long voltageUnit, long currentUnit, long voltageMeasure) :
			base(currentMeasure, voltageUnit, currentUnit, voltageMeasure)
		{
		}
	}
}
