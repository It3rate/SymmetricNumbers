using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

// These are measures that change over time. There are different ways this can happen, and should all be covered in here eventaully.
// 1. Follow - Move towards the reference measure at a rate proportional to the distance.
// 2. Drift - Move in one direction at a constant rate, regardless of the reference measure.
// 3. Accumulate - Move up and down based on inputs, like energy store going up on eating and down on moving.
// 4. Oscillate - Move back and forth around the reference measure, like a pendulum or sine wave.
// 5. Random Walk - Move randomly, but influenced by the reference measure.

// The change can be additive, multiplicative or multiplicative based on the difference in goal and state.
// It can be triggered by tracking external property changes, or self triggered on a timer/tick basis.
// It can have different behaviors based on direction of change, and magnitude of change.
// It can turn on and off.

public class DynamicMeasure : RefMeasure, IDynamicMeasure
{
	public INumber DriftScale { get; protected set; }
	public INumber DriftOffset { get; protected set; }
	public long ProposedPosition { get; }

	public virtual float Accumulator { get; protected set; }

	public DynamicMeasure(MeasureSlot slot, INumber num, INumber? driftScale, INumber? driftOffset) : base(slot, num)
	{
		DriftScale = driftScale ?? RawNumber.One;
		DriftOffset = driftOffset ?? RawNumber.Zero;

		Accumulator = Ref.Position;
	}
	public override void OnExternalUpdate(IMeasure source)
	{
		var change = CalculateChange(source);
		UpdateState(change);
	}
	protected override float CalculateChange(IMeasure source)
	{
		var dif = source.Position - Position;
		var change = 0f;
		if (dif < 0)
		{
			change += dif * DriftScale.LeftValue;
			change += DriftOffset.LeftValue;
		}
		else
		{
			change += dif * DriftScale.RightValue;
			change += DriftOffset.RightValue;
		}
		return change;
	}
	protected override void UpdateState(float change)
	{
		Accumulator += change;
		SetPositionAndInvoke((long)Accumulator);
	}



	public static DynamicMeasure CreateFollow(MeasureSlot slot, INumber num, float diffScale, long resolution) =>
		new DynamicMeasure(slot, num, new ComplexNumber(diffScale, diffScale, resolution), ComplexNumber.Zero);
	public static DynamicMeasure CreateFollow(MeasureSlot slot, INumber num, float ifLowerScale, float ifHigherScale, long resolution) =>
		new DynamicMeasure(slot, num, new ComplexNumber(ifLowerScale, ifHigherScale, resolution), ComplexNumber.Zero);
	public static DynamicMeasure CreateDrift(MeasureSlot slot, INumber num, float bias, long resolution) =>
		new DynamicMeasure(slot, num, new ComplexNumber(0, 0, resolution), new ComplexNumber(bias, bias, resolution));
	public static DynamicMeasure CreateDrift(MeasureSlot slot, INumber num, float ifLowerBias, float ifHigherBias, long resolution) =>
		new DynamicMeasure(slot, num, new ComplexNumber(0, 0, resolution), new ComplexNumber(ifLowerBias, ifHigherBias, resolution));

	private const float _smallDrift = 0.0005f;
	private const float _largeDrift = 0.1f;
	private const long _highResolution = 10000;
	// Eventually these rates will be learned and updated accordingly, for now use engineered values.
	public static DynamicMeasure Min(MeasureSlot slot, INumber num, float? isLessForce=null, float? isMoreForce = null) =>
		CreateFollow(slot, num, isMoreForce ?? _largeDrift, isLessForce ?? _smallDrift, _highResolution);

	public static DynamicMeasure Max(MeasureSlot slot, INumber num, float? isLessForce = null, float? isMoreForce = null) =>
		CreateFollow(slot, num, isLessForce ?? _smallDrift, isMoreForce ?? _largeDrift, _highResolution);
	
	public static DynamicMeasure SlowFollow(MeasureSlot slot, INumber num, float? smallDrift = null) =>
		CreateFollow(slot, num, smallDrift ?? _smallDrift, _highResolution);
	
	public static DynamicMeasure FastFollow(MeasureSlot slot, INumber num, float? largeDrift = null) =>
		CreateFollow(slot, num, largeDrift ?? _largeDrift, _highResolution);
	
	public static DynamicMeasure SlowSink(MeasureSlot slot, INumber num, float? slowDrift = null) =>
		CreateDrift(slot, num, -slowDrift ?? _smallDrift, _highResolution);
	public static DynamicMeasure SlowRise(MeasureSlot slot, INumber num, float? slowDrift = null) =>
		CreateDrift(slot, num, slowDrift ?? _smallDrift, _highResolution);

}
