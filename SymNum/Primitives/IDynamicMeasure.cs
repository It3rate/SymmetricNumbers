using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

public interface IDynamicMeasure : IRefMeasure
{
	long ProposedPosition { get; }
	/// <summary>
	/// Used to store the float remainder of a value. 
	/// Think a synapse's firing threshold as it's resolution, but charge below this resolution builds and decays.
	/// </summary>
	float Accumulator { get; }

	//float CalculateChange(IMeasure source);
	//void UpdateState(float change);

	public static IMeasure Min(IMeasure a, IMeasure b) => a.Position < b.Position ? a : b;
	public static IMeasure Max(IMeasure a, IMeasure b) => a.Position >= b.Position ? a : b;
	public static long MinPosition(IMeasure a, IMeasure b) => Math.Min(a.Position, b.Position);
	public static long MaxPosition(IMeasure a, IMeasure b) => Math.Max(a.Position, b.Position);
}
