namespace SymNum.Primitives;

using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class DecayMeasure : MeasureBase, IVolatile, IDisposable
{
    public IMeasure DecayRate { get; set; }
    public IMeasure Minimum { get; set; }
    public IMeasure Maximum { get; set; }
    public IMeasure? FollowTarget { get; set; }

    public override bool IsFullDefined => true;

    public override bool Inclusive => true;
    public override bool IsCertain => true;
    public override bool IsMax => Position >= Maximum.Position;
    public override bool IsMin => Position <= Minimum.Position;

    private IMeasure _position;
    public override long Position
    {
        get => _position.Position;
        protected set => _position.SetPosition(long.Clamp(value, Minimum.Position, Maximum.Position));
    }

    public DecayMeasure(long pos, long decayRate, long minimum, long maximum, IMeasure? followTarget = null)
    {
        _position = new Measure(pos, true, CertaintyLevel.Set);
        DecayRate = new Measure(decayRate, true, CertaintyLevel.Set);
        Minimum = new Measure(minimum, true, CertaintyLevel.Set);
        Maximum = new Measure(maximum, true, CertaintyLevel.Set);
        FollowTarget = followTarget;
        if(FollowTarget != null)
		{
			FollowTarget.PositionChanged += OnUpdate;
		}

	}
    public DecayMeasure(IMeasure pos, IMeasure decayRate, IMeasure minimum, IMeasure maximum)
    {
        _position = pos;
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
        var change = DecayRate.Position;
        if(FollowTarget != null)
		{
			change = FollowTarget.Position >= Position ? DecayRate.Position : -DecayRate.Position;
		}
		SetPosition((long)(change + Position));
    }
    public void Dispose()
	{
		if (FollowTarget != null)
		{
			FollowTarget.PositionChanged -= OnUpdate;
		}
	}
    
}
