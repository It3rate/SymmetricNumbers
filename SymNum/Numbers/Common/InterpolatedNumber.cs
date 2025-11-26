
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers.Common;

public class InterpolatedNumber : RawNumber, IDisposable
{
	public IMeasure LeftWeight => Measures[1];
	public IMeasure RightWeight => Measures[2];

	// these are calculated measures that can be used externally in other numbers
	public IMeasure InterpolatedRational { get; } = new Measure(0);
	public IMeasure InterpolatedImaginary { get; } = new Measure(0);

	public InterpolatedNumber(IMeasure rightMeasure, IMeasure leftWeight, IMeasure rightWeight, IMeasure leftMeasure) :
		base(rightMeasure, leftWeight, rightWeight, leftMeasure)
	{
		rightMeasure.PositionChanged += OnReferenceUpdate;
		leftMeasure.PositionChanged += OnReferenceUpdate;
	}
	public InterpolatedNumber(IMeasure rightMeasure, long leftWeight, long rightWeight, IMeasure leftMeasure) :
		base(rightMeasure, new Measure(leftWeight), new Measure(rightWeight), leftMeasure)
	{
		rightMeasure.PositionChanged += OnReferenceUpdate;
		leftMeasure.PositionChanged += OnReferenceUpdate;
	}

	private void OnReferenceUpdate(IMeasure measure)
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
