using SymNum.Numbers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Scraps
{
    /// <summary>
    /// An anchor number can only have an imaginary or rational segment, not both.
    /// So 0i+5 or 5i+0 are both valid, while 5i+5 is not. 
    /// You set it by setting a value, positive or negative, and it will set the correct polarity.
    /// </summary>
    public class AnchorNumber : RawNumber, IDisposable
    {
        private Ratio _ratioRef;
        private long _position => _ratioRef.Numerator.ProposedPosition;
        private long _resolution => _ratioRef.Denominator.Position;
        private Polarity _polarity => _ratioRef.Polarity;
        public AnchorNumber(Polarity polarity, long resolution) : this(new Ratio(0, resolution, polarity)) { }
        public AnchorNumber(Ratio ratio)
        {
            _ratioRef = ratio;

            LeftUnit = Measure.CreateUnit(_resolution);
            RightUnit = Measure.CreateUnit(_resolution);
            LeftMeasure = new Measure(0, CertaintyLevel.Set, false);
            RightMeasure = new Measure(0, CertaintyLevel.Set, false);
            OnMeasureChanged(_ratioRef.Numerator);

            _ratioRef.Numerator.PositionChanged += OnMeasureChanged;
        }
        public override void Validate()
        {
        }
        private void OnMeasureChanged(Measure obj)
        {
            bool isRight = _polarity == Polarity.FacesRight;
            var rightPos = isRight && _position > 0 ? _position : !isRight && _position < 0 ? -_position : 0;
            var leftPos = !isRight && _position > 0 ? _position : isRight && _position < 0 ? -_position : 0;
            LeftMeasure.SetPosition(leftPos);
            RightMeasure.SetPosition(rightPos);
        }
        public void Dispose()
        {
            _ratioRef.Numerator.PositionChanged -= OnMeasureChanged;
        }
    }
}
