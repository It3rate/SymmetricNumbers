using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

// A MeasureRef is always inside a number, so things like polarity and min max are derived from the number, not inherent in the measure.
public class RefMeasure : Measure, IRefMeasure<long>
{
	public INumber NumberRef { get; }
	public MeasureSlot Slot { get; }
	public IMeasure<long> Ref => NumberRef.GetMeasureBySlot(Slot);
	private Measure _refm =>(Measure)Ref;
	public override long Position => Ref.Position;
	override public void SetPosition(long position) => _refm.SetPosition(position);
	override public void SetPositionAndInvoke(long position) => _refm.SetPositionAndInvoke(position);

	public CertaintyLevel Certainty => NumberRef.GetCertaintyBySlot(Slot);
	public bool IsInput => Certainty.IsInput();
	public Polarity Polarity => NumberRef.GetPolarityBySlot(Slot);
	public IMeasure<long> Bias => NumberRef.Bias;

	public RefMeasure(MeasureSlot slot, INumber num) : base(0)
	{
		NumberRef = num;
		Slot = slot;
		SetPosition(Ref.Position);
		NumberRef.MeasureChange += OnMeasureChange;
	}

	public long PositionAs(Polarity polarity) => polarity == Polarity ? Position : -Position;

	private void OnMeasureChange(Measure from, Measure to, MeasureSlot slot)
	{
		if(slot == Slot )
		{
			from.PositionChanged -= OnExternalUpdate;
			to.PositionChanged += OnExternalUpdate;
		}
	}

	public virtual void OnExternalUpdate(IMeasure<long> source)
	{
	}
	public virtual void Dispose()
	{
		if (Ref != null)
		{
			Ref.PositionChanged -= OnExternalUpdate;
		}
	}

}


/*
// There is a measure and an interpretation of that measure. A light sensor reads whatever it reads,
// but this can be scaled or shifted to otherwise use by anything that used the original measure.
// Only Refs can do this, because you can change the input signal without changing the mechanism the generates it
// (which can be external, or an intermediate value created deeper in the system).
public abstract class MeasureRefBase : IRefMeasure
{
	public virtual Measure Ref { get; }
	public Ratio Scale { get; } // this scales the reference position and bias
	public virtual Measure Bias { get; set; } = Measure.Zero;
	public virtual void SetBias(Measure bias)
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
	public MeasureRefBase(Measure reference, float scale = 1f, long bias = 0) : this(scale, bias)
	{
		Ref = reference;
		Ref.PositionChanged += OnUpdate;
	}
	public virtual void OnUpdate(Measure source)
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
	//public static MeasureRef MirroredRef(Measure measure) => new(measure, -1); 

}
*/