using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

public class MeasureRef : MeasureBase
{
	public IMeasure Ref { get; }
	public RationalNumber Scale { get; }
	public IMeasure Bias { get; }

	public override long Position
	{
		get => (long)(Ref.Position * Scale.FloatValue) + Bias.Position;
		protected set
		{
			var val = (long)((value - Bias.Position) / Scale.FloatValue);
			Ref.SetPosition(val);
		}
	}

	public MeasureRef(IMeasure measure, float scale = 1f, float offset = 0)
	{
		Ref = measure;
		Scale = RationalNumber.HighResNumber(scale);
		Bias = new Measure((long)offset);
	}

	// the offset needs to be negated if the ref measure has an offset.
	// This suggests all measures need an offset. Makes sense as the zero point is arbitrary.
	public static MeasureRef MirroredRef(IMeasure measure) => new(measure, -1, 0); 

}
