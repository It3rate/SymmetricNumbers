using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers
{
	/// <summary>
	/// An anchor number can only have an imaginary or rational segment, not both.
	/// So 0i+5 or 5i+0 are both valid, while 5i+5 is not. 
	/// You set it by setting a value, positive or negative, and it will set the correct polarity.
	/// </summary>
	public class AnchorNumber : RawNumber, IDisposable
	{
		public AnchorNumber(float value, long resolution) : this((long)(value * resolution), resolution) { }
		public AnchorNumber(long position, long resolution, Polarity polarityContext = Polarity.FacesRight)
		{ 
			LeftUnit = Measure.CreateUnit(resolution);
			RightUnit = Measure.CreateUnit(resolution);
			LeftMeasure = new Measure(0, CertaintyLevel.Set, false);
			RightMeasure = new Measure(0, CertaintyLevel.Set, false);
			UpdateMeasures(position, polarityContext);

			LeftMeasure.PositionChanged += OnLeftMeasureChanged;
			RightMeasure.PositionChanged += OnRightMeasureChanged;
		}

		private void OnLeftMeasureChanged(IMeasure obj)
		{
			UpdateMeasures(obj.ProposedPosition, Polarity.FacesLeft);
		}
		private void OnRightMeasureChanged(IMeasure obj)
		{
			UpdateMeasures(obj.ProposedPosition, Polarity.FacesLeft);
		}
		private void UpdateMeasures(long position, Polarity polarityContext)
		{
			bool isRight = polarityContext == Polarity.FacesRight;
			var rightPos = isRight && position > 0 ? position : !isRight && position < 0 ? -position : 0;
			var leftPos = !isRight && position > 0 ? position : isRight && position < 0 ? -position : 0;
			LeftMeasure.SetPosition(leftPos);
			RightMeasure.SetPosition(rightPos);
		}
		public void Dispose()
		{
			LeftMeasure.PositionChanged -= OnLeftMeasureChanged;
			RightMeasure.PositionChanged -= OnRightMeasureChanged;
		}
	}
}
