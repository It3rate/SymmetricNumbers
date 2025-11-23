using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;
using SymNum.Scraps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

// A MeasureRef is always inside a number, so things like polarity and min max are derived from the number, not inherent in the measure.
public class MeasureRef : MeasureRefBase
{
	public override IMeasure Ref => Number.GetMeasureBySlot(Slot);
	public INumber Number { get; }
	public MeasureSlot Slot { get; }
	public MeasureRef(int index, INumber num, float scale = 1f, long bias = 0) : base(scale, bias)
	{
		Number = num;
		Slot = (MeasureSlot)(index % 5);
		Ref.PositionChanged += OnUpdate;
	}
}

// There is a measure and an interpretation of that measure. A light sensor reads whatever it reads,
// but this can be scaled or shifted to otherwise use by anything that used the original measure.
// Only Refs can do this, because you can change the input signal without changing the mechanism the generates it
// (which can be external, or an intermediate value created deeper in the system).
public abstract class MeasureRefBase : MeasureBase, IMeasureRef
{
	public virtual IMeasure Ref { get; }
	public Ratio Scale { get; } // this scales the reference position and bias
	public virtual IMeasure Bias { get; set; } = IMeasure.Zero;
	public virtual void SetBias(IMeasure bias)
	{
		Bias = bias;
		//InvokePositionChanged(); // does this trigger a number change?
	}
	public override long Position  => (long)(Ref.Position * Scale.Value) + Bias.Position;
	public override void SetPosition(long position) // perhaps refs are readonly?
	{
		var val = (long)((position - Bias.Position) / Scale.RightValue);
		Ref.SetPositionAndInvoke(val);
	}
	//public override long Bias
	//{
	//	get  => (long)(Ref.Bias * Scale.FloatValue);
	//	set => Ref.SetBias((long)(value / Scale.FloatValue));
	//}
	protected MeasureRefBase(float scale = 1f, long bias = 0)
	{
		Scale = Ratio.HighResNumber(scale);
		if (bias != 0)
		{
			Bias = new Measure(bias);
		}
	}
	public MeasureRefBase(IMeasure reference, float scale = 1f, long bias = 0) : this(scale, bias)
	{
		Ref = reference;
		Ref.PositionChanged += OnUpdate;
	}
	public virtual void OnUpdate(IMeasure source)
	{
		InvokePositionChanged();
	}

	// the offset needs to be negated if the ref measure has an offset.
	// This suggests all measures need an offset. Makes sense as the zero point is arbitrary.
	// Q. Can you have to allow a number where all zero points are different?
	// Q. If you allow it, do you still allow a number only zero point too?
	// If there is no gain, perhaps zero point should be a number only thing.
	// There may be math reasons, reality reasons, or even just training reasons,
	// but it doesn't add complexity, so one of those need to be true.
	//public static MeasureRef MirroredRef(IMeasure measure) => new(measure, -1); 

}
