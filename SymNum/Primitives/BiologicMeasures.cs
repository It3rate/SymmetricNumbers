using SymNum.Expressions;
using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

// Biologic numbers are dynamic, not fixed values. They have a natural decay rate and can be refreshed over time.
// They often follow an input moving according to the target value. At minimum it needs a clock for ticks, which could even be internal.
// The simplest is decay, where a value is reduced over time, like energy levels. The inverse is an accumlator, which increases.
// Reactive biologic numbers look at the input and react one way if it is larger than the current value, another way if it is smaller.
// This can result in a follower with different levels of damping based on how much it closes the gap each tick.
// If it chases it hard in the positive direction and very slowly in the negative, you get a max/min that will be aproximately valid over a chosen time span.
// with a few followers of different decay rates you can get a change estimate over time, with the ones that close the gap quicker being more recent.
// The 'gap closing' is also an operation. It can be a percent of the diff, or a fixed amount added. These amount can also be dynamic and accumulated.
// Cos and Sin can naturally fall out of an additive signal that accumulates it speed, adding or reducing it based on the direction of the center.

// Biologic numbers are still in the regular number system, so they can work on measures or Number values themselves.
// As measures, they can hook to any part of the number - zero point, denominator or numerator, real or imaginary.
// All the parameters in dynamic numbers are learnable just like regular numbers.
// Essentially they are also regular equations, but react to input.
// In biology or other continuous systems these numbers are more natural, as they expect continuous interaction with the environment.

public class BiologicMeasures
{
	// min, max, average, accumulate, follow, time series estimates, sin, cos, calculus.
	// These come from arrangements of Dynamic measures, and interpretations of their resulting numbers.
}
public class MinMaxMeasure : DynamicMeasure
{
	// may need to accommodate multiple references?
	// Real and imaginary side combine into minmax, but maybe imaginary is always min (even if it is greater than real)?
	public override long Position { get => Ref.Position; }
	public DynamicMeasure MinMeasure { get; }
	public DynamicMeasure MaxMeasure { get; }
	public MinMaxMeasure(IMeasure reference, float? centerForce = null, float? edgeForce = null):base(reference)
	{
		MinMeasure = Min(reference, centerForce, edgeForce);
		MaxMeasure = Max(reference, centerForce, edgeForce);
	}
	public override void SetPosition(long position)
	{
		Ref.SetPosition(position);
	}
	public override void SetPositionAndInvoke(long position)
	{
		Ref.SetPositionAndInvoke(position);
	}
}
public class MidpointMeasure : DynamicMeasure
{
	public IMeasure First => Ref;
	public IMeasure Second { get; }
	private bool _firstIsImaginary;
	public MidpointMeasure(IMeasure first, IMeasure second, bool firstIsImaginary = true) : base(first)
	{
		Second = second;
		_firstIsImaginary = firstIsImaginary;
		Second.PositionChanged += OnUpdate;
	}
	public override void OnUpdate(IMeasure source)
	{
		base.OnUpdate(source);
		var firstPosition = _firstIsImaginary ? -First.Position : First.Position;
		var pos = (Second.Position - firstPosition) / 2 + firstPosition;
		SetPosition(pos);
	}
	public override void Dispose()
	{
		base.Dispose();
		if (Second != null)
		{
			Second.PositionChanged -= OnUpdate;
		}
	}
}
public class AccumulateMeasure : DynamicMeasure // these are probably just RefMeasures
{
	public AccumulateMeasure(IMeasure reference):base(reference)
	{
	}
	public override void OnUpdate(IMeasure source)
	{
		SetPositionAndInvoke(Position + source.Position);
	}
}

/// <summary>
/// Creates a measure that follows the zero point, increasing speed towards it evenly at every update.
/// The Position is the accumulation of the speed for each tick, going to positive to negative and back each cycle.
/// The Speed is the current speed of the position, positive or negative.
/// The inherited Bias is how much speed is added per tick. If the rational an imaginary part aren't equal it will not be symmetric.
/// The inheritied DiffScale will slowly dampen (or strengthen) the sine wave.
/// </summary>
public class SinCosMeasure : DynamicMeasure
{
	public float Speed => Accumulator;
	public float Sin => throw new NotImplementedException();
	public float Cos => throw new NotImplementedException();
	
	public IMeasure SpeedMeasure { get; }
	public MinMaxMeasure MinMaxPosition { get; }
	public MinMaxMeasure MinMaxSpeed { get; }
	public SinCosMeasure(IMeasure reference) : base(reference)
	{
		MinMaxPosition = new MinMaxMeasure(this, centerForce: 0, edgeForce: 1);
		SpeedMeasure = new Measure(0);
		MinMaxSpeed = new MinMaxMeasure(SpeedMeasure, centerForce: 0, edgeForce: 1);
	}
	public static SinCosMeasure CreateWithSpeed(IMeasure reference, RawNumber bias, float speed)
	{
		var result = new SinCosMeasure(reference);
		result.Bias = bias;
		result.Accumulator = speed;
		result.SetPosition(0);
		return result;
	}
	public static SinCosMeasure CreateWithStartPosition(IMeasure reference, RawNumber bias, long startPosition)
	{
		var result = new SinCosMeasure(reference);
		result.Bias = bias;
		result.Accumulator = 0;
		result.SetPosition(startPosition);
		return result;
	}
	private float _runningPosition;
	protected override void UpdateState(float change)
	{
		Accumulator += change;
		_runningPosition += (long)Accumulator;
		SetPositionAndInvoke((long)_runningPosition);
		SpeedMeasure.SetPositionAndInvoke((long)Accumulator);
	}
}