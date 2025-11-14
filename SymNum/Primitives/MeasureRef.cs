using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

// There is a measure and an interpretation of that measure. A light sensor reads whatever it reads,
// but this can be scaled or shifted to otherwise use by anything that used the original measure.
// Only Refs can do this, because you can change the input signal without changing the mechanism the generates it
// (which can be external, or an intermediate value created deeper in the system).
public class MeasureRef : MeasureBase, IRef
{
	public IMeasure Ref { get; }
	public INumber Scale { get; } // this scales the reference position and bias
	public virtual long Bias { get; set; } = 0;
	public virtual void SetBias(long bias)
	{
		Bias = bias;
		//PositionChanged?.Invoke(this); // does this trigger a number change?
	}
	public override long Position
	{
		get => (long)(Ref.Position * Scale.RightValue) + Bias;
		protected set
		{
			var val = (long)((value - Bias) / Scale.RightValue);
			Ref.SetPosition(val);
		}
	}
	//public override long Bias
	//{
	//	get  => (long)(Ref.Bias * Scale.FloatValue);
	//	set => Ref.SetBias((long)(value / Scale.FloatValue));
	//}
	public MeasureRef(IMeasure measure, float scale = 1f)
	{
		Ref = measure;
		Scale = RationalNumber.HighResNumber(scale);
	}
	public virtual void OnUpdate(IMeasure source)
	{
	}

	// the offset needs to be negated if the ref measure has an offset.
	// This suggests all measures need an offset. Makes sense as the zero point is arbitrary.
	// Q. Can you have to allow a number where all zero points are different?
	// Q. If you allow it, do you still allow a number only zero point too?
	// If there is no gain, perhaps zero point should be a number only thing.
	// There may be math reasons, reality reasons, or even just training reasons,
	// but it doesn't add complexity, so one of those need to be true.
	public static MeasureRef MirroredRef(IMeasure measure) => new(measure, -1); 

}
