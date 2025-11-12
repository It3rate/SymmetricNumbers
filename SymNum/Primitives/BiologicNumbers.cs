using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

// Biologic numbers are dynamic, not fixed values. They have a natural decay rate and can be refreshed over time.
// They often follow an input moving according to the target value. At minimum it needs a clock for ticks, which could even be internal.
// The simplest is decay, where a value is reduced over time, like energy levels. The inverse is an accumlator, which increases.
// Reactive biologic numbers look at the input and react one way if it is larger than the current value, another way if it is smaller.
// This can result in a follower with different levels of damping based on how much it closes the gap each tick.
// If it chases it hard in the positive direction and very slowly in the negative, you get a max/min that will be aproximately valid over a chosen time span.
// with a few followers of different decay rates you can get a change estimate over time, with the ones that close the gap quicker being more recent.
// The 'gap closing' is also an operation. It can be a percent of the diff, or a fixed amount added. These amount can also be dynamic and accumulated.
// Cos and Sin can naturally fall out of an additive signal that accumulates it speed, adding or reducing it based on the direction of the center.

// Biologic numbers are still in the regular number system, so they can work on measures or Number values themselves.
// As measures, they can hook to any part of the number - zero point, denominator or numerator, real or imaginary.
// All the parameters in dynamic numbers are learnable just like regular numbers.
// Essentially they are also regular equations, but react to input.
// In biology or other continuous systems these numbers are more natural, as they expect continuous interaction with the environment.

public class BiologicNumbers
{
	// min, max, average, accumulate, follow, time series estimates, sin, cos, calculus.
}
