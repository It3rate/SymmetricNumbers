using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

public class DriftMeasure : Measure, IDisposable
{
	private float _accPosition;
	public IMeasure Ref { get; }
	public bool TrackRef { get; } = true;

	public ComplexNumber DriftRate { get; }

	public DriftMeasure(IMeasure reference, float driftRate, long resolution) : base(reference.Position)
	{
		Ref = reference;
		TrackRef = false;
		DriftRate = new ComplexNumber(driftRate, driftRate, resolution);
		Position = reference.Position;
		_accPosition = reference.Position;
		Ref.PositionChanged += OnUpdate;
	}
	public DriftMeasure(IMeasure reference, float ifLower, float ifHigher, long resolution) : base(reference.Position)
	{
		Ref = reference;
		DriftRate = new ComplexNumber(ifLower, ifHigher, resolution);
		Position = reference.Position;
		_accPosition = reference.Position;
		Ref.PositionChanged += OnUpdate;
	}
	public void OnUpdate(IMeasure source)
	{
		var change = 0f;
		if (TrackRef)
		{
			var dif = source.Position - Position;
			change = dif < 0 ? dif * DriftRate.LeftValue : dif * DriftRate.RightValue;
		}
		else
		{
			change = Position * DriftRate.RightValue;
		}
		_accPosition += change;
		SetPosition((long)_accPosition);
	}

	public void Dispose()
	{
		if (Ref != null)
		{
			Ref.PositionChanged -= OnUpdate;
		}
	}

	private const float _smallDrift = 0.001f;
	private const float _largeDrift = 0.2f;
	private const long _highResolution = 10000;
	public static DriftMeasure Min(IMeasure reference)
	{
		return new DriftMeasure(reference, _largeDrift, _smallDrift, _highResolution);
	}
	public static DriftMeasure Max(IMeasure reference)
	{
		return new DriftMeasure(reference, _smallDrift, _largeDrift, _highResolution);
	}
	public static DriftMeasure SlowFollow(IMeasure reference, float smallDrift = _smallDrift)
	{
		return new DriftMeasure(reference, smallDrift, smallDrift, _highResolution);
	}
	public static DriftMeasure FastFollow(IMeasure reference, float largeDrift = _largeDrift)
	{
		return new DriftMeasure(reference, largeDrift, largeDrift, _highResolution);
	}
	public static DriftMeasure SlowSink(IMeasure reference, float slowDrift = -_smallDrift)
	{
		return new DriftMeasure(reference, slowDrift, _highResolution);
	}
	public static DriftMeasure SlowRise(IMeasure reference, float slowDrift = _smallDrift)
	{
		return new DriftMeasure(reference, slowDrift, _highResolution);
	}
}
