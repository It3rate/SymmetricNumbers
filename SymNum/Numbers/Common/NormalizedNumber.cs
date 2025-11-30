using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers.Common;

public class NormalizedNumber : Number, IDisposable
{
	public IMeasure Ref => Measures[0];
	public IMeasure Minimum => Measures[1];
	public IMeasure Maximum => Measures[2];
	public IMeasure LeftRef => Measures[3];

	// these are calculated measures that can be used externally in other numbers
	public IMeasure DistanceFromMinimum { get; } = new Measure(0);
	public IMeasure DistanceFromMaximum { get; } = new Measure(0);
	public IMeasure MinMaxLength { get; } = new Measure(0);

	public bool IsMax => Ref.Position >= Maximum.Position;
	public bool IsMin => Ref.Position <= Minimum.Position;

	private static ComplexNumber _minTracking = new ComplexNumber(10, 0, 10);
	private static ComplexNumber _maxTracking = new ComplexNumber(0, 10, 10);
	private static ComplexNumber _centerTracking = new ComplexNumber(1, 1, 100);
	public NormalizedNumber(IMeasure reference, 
		INumber? minTracking = null, INumber? maxTracking = null, INumber? centerTracking = null)
	{
		Measures[0] = new DynamicMeasure(0, this, centerTracking ?? _centerTracking, null);

		Measures[1] = reference;
		Measures[2] = new DynamicMeasure(0, this, minTracking ?? _minTracking, null);
		Measures[3] = new DynamicMeasure(0, this, maxTracking ?? _maxTracking, null);
		Measures[4] = reference;

		reference.PositionChanged += OnReferenceUpdate;
	}
	public NormalizedNumber(IMeasure reference, IMeasure center, IMeasure minimum, IMeasure maximum)
	{
		Measures[0] = center;
		Measures[1] = reference;
		Measures[2] = minimum;
		Measures[3] = maximum;
		Measures[4] = reference;

		reference.PositionChanged += OnReferenceUpdate;
	}

	private void OnReferenceUpdate(IMeasure obj)
	{
		DistanceFromMinimum.SetPosition(Ref.Position - Minimum.Position);
		DistanceFromMaximum.SetPosition(Maximum.Position - Ref.Position);
		MinMaxLength.SetPosition(Maximum.Position - Minimum.Position);
	}

	public void Dispose()
	{
		Ref.PositionChanged -= OnReferenceUpdate;
	}
}
