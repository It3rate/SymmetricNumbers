namespace SymNum.Scraps;

using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class DecayMeasure : DynamicMeasure
{
	// These types of classes are meant to model input and biological functions.

	// There is both the singal and the interpretation. The signal is always unaltered,
	// but the interpretation is usally conditioned - at least with scale and bias, but other ways too.
	// Not sure if this interpretation is always an equation, or if scale and bias are so common they are built in?

	// An important operation is inverting, where the lighter it gets (positive) the slower you go (negative).
	// the two ways to solve this are
	// 1) use the imaginary side of complex number points (where real and imaginary are the same measure)
	// thus getting height and depth with the same value. This needs a max value if everything is to remain positive.
	// 2) have a clamped min/max on the measure, and the inverse is max-value.

	#region MinMax
	public Measure Minimum { get; set; }
	public Measure Maximum { get; set; }
	public long DistanceFromMinimum { get; }
	public long DistanceFromMaximum { get; }
	public long MinMaxLength { get; }
    public bool IsMax => Position >= Maximum.Position;
    public bool IsMin => Position <= Minimum.Position;
	#endregion

	//public RationalNumber Scale { get; set; } = RationalNumber.One;
	//   private Measure _position;
	//   public override long Position
	//   {
	//       get => (long)(_position.Position * Scale.FloatValue) + Bias;
	//       protected set
	//       {
	//           var val = (long)((value - Bias) / Scale.FloatValue);
	//           _position.SetPosition(long.Clamp(val, Minimum.Position, Maximum.Position));
	//       }
	//   }

	public DecayMeasure(MeasureSlot slot, INumber num, INumber decayRate) : base(slot, num, decayRate, null)
    {   
		Decay = DynamicMeasure.FastFollow(slot, num, decayRate);
		Minimum = DynamicMeasure.Min(slot, num);
		Maximum = DynamicMeasure.Max(slot, num);
		Ref.PositionChanged += OnUpdate;
	}
	public DecayMeasure(Measure reference, float decayRate, Measure minimum, Measure maximum)
	{
		Ref = reference;
		SetPosition(reference.Position);// new Measure(pos,  CertaintyLevel.Set);
        Decay = DynamicMeasure.SlowFollow(reference, decayRate);
        Minimum = minimum;
        Maximum = maximum;
		Ref.PositionChanged += OnUpdate;
	}
	public float Norm() => (float)(Position - Minimum.Position) / (Maximum.Position - Minimum.Position);
	public float InvNorm => 1f - Norm();
    public void OnUpdate(Measure source)
	{
		// all sub refs have now updated
		SetPositionAndInvoke(long.Clamp(Decay.Position, Minimum.Position, Maximum.Position));
		MinMaxLength.SetPositionAndInvoke(Maximum.Position - Minimum.Position);
		DistanceFromMinimum.SetPositionAndInvoke(Position - Minimum.Position);
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
