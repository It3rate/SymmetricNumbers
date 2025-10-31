namespace SymNum.Primitives;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DecayMeasure : MeasureBase, IVolatile
{
    public IMeasure DecayRate { get; set; }
    public IMeasure Minimum { get; set; }
    public IMeasure Maximum { get; set; }
    public IMeasure? Boost { get; set; }
    public override CertaintyLevel Certainty => CertaintyLevel.Perceived;

    public override bool Inclusive => true;
    public override bool IsCertain => true;
    public override bool IsMax => Position >= Maximum.Position;
    public override bool IsMin => Position <= Minimum.Position;

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
    public void Update(float deltaTime)
    {
        var change = DecayRate.Position + (Boost == null ? 0 : Boost.Position);
        SetPosition(Position - change);
    }
    public override void SetPosition(long position)
    {
        if (Position <= Minimum.Position)
        {
            Position = Minimum.Position;
        }
        else if (Position >= Maximum.Position)
        {
            Position = Maximum.Position;
        }
        else
        {
            Position = position;
        }
    }
}
