using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Patterns;

/// <summary>
/// A Real Part number is the right side of a complex number (or split number).
/// This is essentially our normal numbers, but with a resolution component (the ticks in the denominator).
/// It has a unit (bottom right) and a measure (top right).
/// The unit of the imaginary side has to match the real unit, but points in the opposite direction.
/// When combining with an imaginary part into a SplitNumber, the unit of the imaginary part is used.
/// When combining to a complex number (identical units) the min resolution is used
/// </summary>
public class RealPartNumber : UnconstrainedNumber
{
	// the only constraints are that left unit is always UnitPosition pointing left, and left measure is always 0.
	// All operations on complex numbers should apply? (and imaginary part will always be zero)
	public long MeasurePosition
	{
		get => RightMeasure.Position;
		set { RightMeasure.SetPosition(value); LeftMeasure.SetPosition(0); }
	}
	public long UnitPosition
	{
		get => RightUnit.Position;
		set { RightUnit.SetPosition(value); LeftUnit.SetPosition(-value); }
	}
	public long TickCount => MeasurePosition;
	public long Resolution
	{
		get => UnitPosition;
		protected set
		{
			MeasurePosition = (long)(FloatValue * value);
			UnitPosition = value;
		}
	}
	public float FloatValue
	{
		get => RightValue;
		protected set => RightValue = value;
	}
	/// <summary>
	/// A RationalNumber is a complex number with constraints on the left side.
	/// </summary>
	public RealPartNumber(long resolution, long value)
	{
		RightMeasure = new Measure(value, Direction.FacesRight, true);
		RightUnit = new Measure(resolution, Direction.FacesRight, true);
		LeftUnit = new Measure(resolution, Direction.FacesLeft, true);
		LeftMeasure = new Measure(0, Direction.FacesLeft, true);
	}
	public RealPartNumber(long resolution, float value) : this(resolution, (long)(value * resolution)) { }

	// ScaleAll - multiply both measure and unit by the same factor, increasing resolution but not changing the value.
	// Value - measure / unit, what we normally consider the number to be.
	// Reciprocal - unit/value.
	// Shift - this is akin to add and subtract, but there is no added value. Essentially it is shifting the zero point. Add requires two things.

	// Add, multiply etc all require a second rational number.

}
