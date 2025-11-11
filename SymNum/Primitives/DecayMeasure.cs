namespace SymNum.Primitives;

using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class DecayMeasure : MeasureBase, IDisposable
{
    // There is both the singal and the interpretation. The signal is always unaltered,
    // but the interpretation is usally conditioned - at least with scale and bias, but other ways too.
    // Not sure if this interpretation is always an equation, or if scale and bias are so common they are built in?

    // An important operation is inverting, where the lighter it gets (positive) the slower you go (negative).
    // the two ways to solve this are
    // 1) use the imaginary side of complex number points (where real and imaginary are the same measure)
    // thus getting height and depth with the same value. This needs a max value if everything is to remain positive.
    // 2) have a clamped min/max on the measure, and the inverse is max-value. 
    public IMeasure Ref { get; set; }
	public DriftMeasure Decay { get; set; }

	public IMeasure Minimum { get; set; }
    public IMeasure Maximum { get; set; }

    public override bool IsFullDefined => true;
    public override bool IsCertain => true;
    public override bool IsMax => Position >= Maximum.Position;
    public override bool IsMin => Position <= Minimum.Position;

	//public RationalNumber Scale { get; set; } = RationalNumber.One;
 //   private IMeasure _position;
 //   public override long Position
 //   {
 //       get => (long)(_position.Position * Scale.FloatValue) + Bias;
 //       protected set
 //       {
 //           var val = (long)((value - Bias) / Scale.FloatValue);
 //           _position.SetPosition(long.Clamp(val, Minimum.Position, Maximum.Position));
 //       }
 //   }

    public DecayMeasure(IMeasure reference, float decayRate)
    {   
        Ref = reference;
		Decay = DriftMeasure.FastFollow(reference, .9f);
		Minimum = DriftMeasure.Min(reference);
		Maximum = DriftMeasure.Max(reference);
		Ref.PositionChanged += OnUpdate;
	}
	public float Norm() => (float)(Position - Minimum.Position) / (Maximum.Position - Minimum.Position);
	public float InvNorm => 1f - Norm();
	public override void SetPosition(long position)
	{
		Position = position;
	}
	public DecayMeasure(IMeasure reference, float decayRate, IMeasure minimum, IMeasure maximum)
	{
		Ref = reference;
		Position = reference.Position;// new Measure(pos,  CertaintyLevel.Set);
        Decay = DriftMeasure.SlowFollow(reference, decayRate);
        Minimum = minimum;
        Maximum = maximum;
        Ref.PositionChanged += OnUpdate;
	}
    public void OnUpdate(IMeasure source)
	{
		// all sub refs have now updated
		SetPosition(long.Clamp(Decay.Position, Minimum.Position, Maximum.Position));
        //Trace.WriteLine(Position + " " + Ref?.Position);
    }
    public void Dispose()
	{
		if (Ref != null)
		{
			Ref.PositionChanged -= OnUpdate;
		}
	}
    
}
