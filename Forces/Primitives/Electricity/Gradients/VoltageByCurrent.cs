using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Electricity.Gradients
{
    public class VoltageByCurrent : SymmetricNumber
	{
		public List<VoltageByCurrent> Samples { get; } = new List<VoltageByCurrent>();
		public VoltageByCurrent(long voltageMeasure, long currentUnit, long voltageUnit, long currentMeasure) :
			base(voltageMeasure, currentUnit, voltageUnit, currentMeasure)
		{
		}
	}
}
