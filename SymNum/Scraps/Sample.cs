using SymNum.Concepts;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Scraps;

/// <summary>
/// Represents a single sample of some aspect. 
/// If there was a giant database of all the colors you've ever seen, annotated by the object it was on and how much you cared,
/// this would be a single entry. It is the instance at a moment, and is probably very short lived in our heads.
/// </summary>
public class Sample
{
	public Extent Value { get; set; }
	public PropertyUnit SourceUnit { get; set; }
	/// <summary>
	/// Might be emotional context, desireabiliy, frequency etc
	/// We see a lot of things, some leave lasting impressions, some don't. 
	/// This is used to calibrate what to do with the information, and affects our default expectations, what surprises us etc.
	/// There is some way to tell one from the other, this represents the way, whatever it is.
	/// </summary>
	public object Significance { get; set; }
}
