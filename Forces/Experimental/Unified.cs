using SpiceSharp.Components.NoiseSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Experimental;

public interface Type
{
	int[] Types { get; }
	// Has lookup table for calibrated types, as in Area2 and Area3 can be types, and lookup to m^2 and feet^2
	// uncalibrated types are like distVertical and distHorizontal, which are convertable to the same type/subtype
	// perhaps they all use the same unit, and there is a conversion type, which is a scalar multiply and add.
}
public interface Element // these never merge
{
	bool IsComposing { get; } // merge or split apart
	int Length { get; } // This is the merged 'length'. Like 5 separate lines become a long line, this is the length.
	Type Type { get; }
}

public interface MergbleBlock : Element // these always merge
{
	// The calculation of a block is always automatic, as it knows multiply or add based on the types, and will sort the types appropriately.
	// The order of operations is the order they appear in the list.
	// This can be recursive, so a block can contain other blocks.
	// A MergbleBlock is by definition unmerged. If it is merged, it becomes an Element.
	List<Element> Block { get; } // this is the premerged count of things. Like 5 separate lines, before they are a long line.
								 
	// merges the block into a single element with the merged type.
	Element Merge();
	// if element is null, splits in the simplest way e.g. sqrt
	// if element has no common type, returns [null, original]
	MergbleBlock Unmerge(Element? element); 
}
public record Parameter(int Quantity, MergbleBlock Block);
public interface Expr
{
	// these blocks will not be merged, they are algebraic expressions, and therefore number systems.
	List<Parameter> Expression { get; }
}
