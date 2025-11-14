using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

public class DynamicMeasure : Measure, IRef, IDisposable 
{
	public IMeasure Ref { get; }
	public INumber Scale { get; protected set; }
	public INumber Bias { get; protected set; }

	public virtual float Accumulator { get; protected set; }

	protected DynamicMeasure(IMeasure reference) :base(reference.Position) { }
	public DynamicMeasure(IMeasure reference, RawNumber diffScale, RawNumber bias) : base(reference.Position)
	{
		Ref = reference;
		Scale = diffScale;
		Bias = bias;
		Position = reference.Position;
		Accumulator = reference.Position;
		Ref.PositionChanged += OnUpdate;
	}
	public static DynamicMeasure CreateFollow(IMeasure reference, float diffScale, long resolution) =>
		new DynamicMeasure(reference, new ComplexNumber(diffScale, diffScale, resolution), RationalNumber.Zero);
	public static DynamicMeasure CreateFollow(IMeasure reference, float ifLowerScale, float ifHigherScale, long resolution) =>
		new DynamicMeasure(reference, new ComplexNumber(ifLowerScale, ifHigherScale, resolution), RationalNumber.Zero);
	public static DynamicMeasure CreateDrift(IMeasure reference, float bias, long resolution) =>
		new DynamicMeasure(reference, new ComplexNumber(0, 0, resolution), new ComplexNumber(bias, bias, resolution));
	public static DynamicMeasure CreateDrift(IMeasure reference, float ifLowerBias, float ifHigherBias, long resolution) =>
		new DynamicMeasure(reference, new ComplexNumber(0, 0, resolution), new ComplexNumber(ifLowerBias, ifHigherBias, resolution));

	public virtual void OnUpdate(IMeasure source)
	{
		var change = CalculateChange();
		UpdateState(change);
	}
	protected virtual float CalculateChange()
	{
		var dif = Ref.Position - Position;
		var change = 0f;
		if (dif < 0)
		{
			change = dif * Scale.LeftValue;
			change += Bias.LeftValue;
		}
		else
		{
			change = dif * Scale.RightValue;
			change += Bias.RightValue;
		}
		return change;
	}
	protected virtual void UpdateState(float change)
	{
		Accumulator += change;
		SetPosition((long)Accumulator);
	}
	public void Dispose()
	{
		if (Ref != null)
		{
			Ref.PositionChanged -= OnUpdate;
		}
	}

	private const float _smallDrift = 0.0005f;
	private const float _largeDrift = 0.1f;
	private const long _highResolution = 10000;
	// Eventually these rates will be learned and updated accordingly, for now use engineered values.
	public static DynamicMeasure Min(IMeasure reference, float? small=null, float? large = null) =>
		CreateFollow(reference, large ?? _largeDrift, small ?? _smallDrift, _highResolution);

	public static DynamicMeasure Max(IMeasure reference, float? small = null, float? large = null) =>
		CreateFollow(reference, small ?? _smallDrift, large ?? _largeDrift, _highResolution);
	
	public static DynamicMeasure SlowFollow(IMeasure reference, float? smallDrift = null) =>
		CreateDrift(reference, smallDrift ?? _smallDrift, _highResolution);
	
	public static DynamicMeasure FastFollow(IMeasure reference, float? largeDrift = null) =>
		CreateDrift(reference, largeDrift ?? _largeDrift, _highResolution);
	
	public static DynamicMeasure SlowSink(IMeasure reference, float? slowDrift = null) =>
		CreateDrift(reference, -slowDrift ?? _smallDrift, _highResolution);
	public static DynamicMeasure SlowRise(IMeasure reference, float? slowDrift = null) =>
		CreateDrift(reference, slowDrift ?? _smallDrift, _highResolution);

}
