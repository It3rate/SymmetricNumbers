using SymNum.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Electricity.Gradients
{
	public class CurrentByFrequency : SymmetricNumber
	{
		public List<CurrentByFrequency> Samples { get; } = new List<CurrentByFrequency>();
		public CurrentByFrequency(long currentMeasure, long frequencyUnit, long currentUnit, long frequencyMeasure) : 
			base(currentMeasure, frequencyUnit, currentUnit, frequencyMeasure)
		{
		}
	}
}
