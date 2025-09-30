using SymNum.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Electricity.Gradients
{
	public class FrequencyByCurrent : SymmetricNumber
	{
		public List<FrequencyByCurrent> Samples { get; } = new List<FrequencyByCurrent>();
		public FrequencyByCurrent(long frequencyMeasure, long currentUnit, long frequencyUnit, long currentMeasure) :
			base(frequencyMeasure, currentUnit, frequencyUnit, currentMeasure)
		{
		}
	}
}
