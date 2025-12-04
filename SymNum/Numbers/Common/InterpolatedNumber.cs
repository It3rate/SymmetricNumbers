
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers.Common;

public class InterpolatedNumber : Number, IDisposable
{
	public Measure LeftWeight => Measures[1];
	public Measure RightWeight => Measures[2];

	// these are calculated measures that can be used externally in other numbers
	public Measure InterpolatedRational { get; } = new Measure(0);
	public Measure InterpolatedImaginary { get; } = new Measure(0);

	public InterpolatedNumber(Measure rightMeasure, Measure leftWeight, Measure rightWeight, Measure leftMeasure) :
		base(rightMeasure, leftWeight, rightWeight, leftMeasure)
	{
		rightMeasure.PositionChanged += OnReferenceUpdate;
		leftMeasure.PositionChanged += OnReferenceUpdate;
	}
	public InterpolatedNumber(Measure rightMeasure, long leftWeight, long rightWeight, Measure leftMeasure) :
		base(rightMeasure, new Measure(leftWeight), new Measure(rightWeight), leftMeasure)
	{
		rightMeasure.PositionChanged += OnReferenceUpdate;
		leftMeasure.PositionChanged += OnReferenceUpdate;
	}

	private void OnReferenceUpdate(IMeasure<long> measure)
	{
		var wlen = (-LeftWeight.Position + RightWeight.Position);
		var ratio = -LeftWeight.Position / (float)wlen;
		var len = (-LeftMeasure.Position + RightMeasure.Position);
		var rpos = LeftMeasure.Position + (long)(ratio * len);
		InterpolatedRational.SetPosition(rpos);
		InterpolatedImaginary.SetPosition(-rpos);
	}
	public void Dispose()
	{
		RightMeasure.PositionChanged -= OnReferenceUpdate;
		LeftMeasure.PositionChanged -= OnReferenceUpdate;
	}
}
