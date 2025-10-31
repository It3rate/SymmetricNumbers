using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Electricity.Gradients
{
    public class ResistanceByPower : SymmetricNumber
	{
		public List<ResistanceByPower> Samples { get; } = new List<ResistanceByPower>();
		public ResistanceByPower(long resistanceMeasure, long powerUnit, long resistanceUnit, long powerMeasure) :
			base(resistanceMeasure, powerUnit, resistanceUnit, powerMeasure)
		{
		}
	}
}
