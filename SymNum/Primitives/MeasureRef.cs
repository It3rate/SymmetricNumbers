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
	public RationalNumber Scale { get; } // this scales the reference position and bias

	public override long Position
	{
		get => (long)(Ref.Position * Scale.FloatValue) + Bias;
		protected set
		{
			var val = (long)((value - Bias) / Scale.FloatValue);
			Ref.SetPosition(val);
		}
	}
	public override long Bias
	{
		get  => (long)(Ref.Bias * Scale.FloatValue);
		set => Ref.SetBias((long)(value / Scale.FloatValue));
	}
	public MeasureRef(IMeasure measure, float scale = 1f)
	{
		Ref = measure;
		Scale = RationalNumber.HighResNumber(scale);
	}

	// the offset needs to be negated if the ref measure has an offset.
	// This suggests all measures need an offset. Makes sense as the zero point is arbitrary.
	public static MeasureRef MirroredRef(IMeasure measure) => new(measure, -1); 

}
