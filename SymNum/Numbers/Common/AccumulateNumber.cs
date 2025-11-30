using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers.Common;

public class AccumulateNumber : Number, IDisposable
{
	public IMeasure Ref;
	public IMeasure RationalTotal => Measures[1];
	public IMeasure ImaginaryCount => Measures[2];
	public IMeasure RationalCount => Measures[3];
	public IMeasure ImaginaryTotal => Measures[4];
	public AccumulateNumber(IMeasure reference)
	{
		Ref = reference;
		Measures[0] = new Measure(0); // bias
		Measures[1] = new Measure(0); // rational total
		Measures[2] = new Measure(0); // imaginary count
		Measures[3] = new Measure(0); // rational count
		Measures[4] = new Measure(0); // imaginary total

		Ref.PositionChanged += OnReferenceUpdate;
	}

	private void OnReferenceUpdate(IMeasure measure)
	{
		if(measure.Position >= Bias.Position)
		{
			RationalTotal.SetPosition(RationalTotal.Position + (measure.Position - Bias.Position));

		}
		else
		{
			ImaginaryTotal.SetPosition(ImaginaryTotal.Position + (measure.Position - Bias.Position));
		}
		RationalCount.SetPosition(RationalCount.Position + 1);
		ImaginaryCount.SetPosition(ImaginaryCount.Position + 1);
	}
	public void Dispose()
	{
		Ref.PositionChanged -= OnReferenceUpdate;
	}
}
