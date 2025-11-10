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
	public RationalNumber DecayRate { get; set; } = RationalNumber.Zero;

	public IMeasure Minimum { get; set; }
    public IMeasure Maximum { get; set; }
    public IMeasure? FollowTarget { get; set; }

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

    public DecayMeasure(long pos, float decayRate, long minimum, long maximum, IMeasure? followTarget = null)
    {
        Position = pos;// new Measure(pos,  CertaintyLevel.Set);
        DecayRate = new RationalNumber(decayRate, 1000);
        Minimum = new Measure(minimum, CertaintyLevel.Set);
        Maximum = new Measure(maximum, CertaintyLevel.Set);
        FollowTarget = followTarget;
        if(FollowTarget != null)
		{
			FollowTarget.PositionChanged += OnUpdate;
		}

	}
    public DecayMeasure(long pos, RationalNumber decayRate, IMeasure minimum, IMeasure maximum)
    {
        Position = pos;
        DecayRate = decayRate;
        Minimum = minimum;
        Maximum = maximum;
    }
    public void FlipDecayRate()
    {
        DecayRate.Negate();
    }
    public void OnUpdate(IMeasure source)
    {
        var change = DecayRate.FloatValue;
        if(FollowTarget != null)
		{
			change = (FollowTarget.Position - Position) * DecayRate.FloatValue;
		}
		SetPosition((long)(change + Position));
        Trace.WriteLine(Position + " " + FollowTarget?.Position);
    }
    public void Dispose()
	{
		if (FollowTarget != null)
		{
			FollowTarget.PositionChanged -= OnUpdate;
		}
	}
    
}
