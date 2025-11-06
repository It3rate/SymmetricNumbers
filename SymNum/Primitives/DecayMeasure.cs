namespace SymNum.Primitives;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class DecayMeasure : MeasureBase, IVolatile
{
    public IMeasure DecayRate { get; set; }
    public IMeasure Minimum { get; set; }
    public IMeasure Maximum { get; set; }

    public override Direction Direction => DecayRate.Direction;
    public override bool IsFullDefined => true;

    public override bool Inclusive => true;
    public override bool IsCertain => true;
    public override bool IsMax => Position >= Maximum.Position;
    public override bool IsMin => Position <= Minimum.Position;

    private long _position;
    public override long Position
    {
        get => _position;
        protected set => _position = long.Clamp(value, Minimum.Position, Maximum.Position);
    }

    public DecayMeasure(long decayRate, long minimum, long maximum, Direction direction)
    {
        DecayRate = new Measure(decayRate, direction, true, CertaintyLevel.Set);
        Minimum = new Measure(minimum, direction, true, CertaintyLevel.Set);
        Maximum = new Measure(maximum, direction, true, CertaintyLevel.Set);
    }
    public DecayMeasure(IMeasure decayRate, IMeasure minimum, IMeasure maximum)
    {
        DecayRate = decayRate;
        Minimum = minimum;
        Maximum = maximum;
    }
    public void FlipDecayRate()
    {
        DecayRate.Negate();
    }
    private bool _isFirst = true;
    public void OnTargetChange(IMeasure trackingTarget)
    {
        if (_isFirst)
        {
            _isFirst = false;
            Trace.WriteLine("****************");
            SetPosition(128);// trackingTarget.Position);
        }
        else
        {
            //var change = trackingTarget.Position >= Position ? 1.01f : -1.01f;// DecayRate.Position : -DecayRate.Position;
            SetPosition((long)((trackingTarget.Position - Position)* 0.06f + Position));
        }
    }
}
