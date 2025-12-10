using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Experimental;

public interface Measure
{
	Op Operation { get; } // project, decompose, invert
	Type Type { get; }
	int Unit { get; }
	//int Repeat;
	//int Param; // the parameter is just a repeat count of the unit, a power is the repeat count of the op 
}
public interface Op
{
	// a segment can only add when projected as there is no unit. if it projects from the end of other it is added, from end of self it is diff.
	// the result will always be zero based, as there is no unit to anchor to.
	// a ratio (numerator/denominator) can only multiply when projected as there is a unit and things need to align.
	// to add ratios, you still need to align, but then just use the numerator, which is a segment and thus has to add.

	// a projection (~multiply) always lowers type counts and loses infomation (2x3=6 :: 6 can not recover 2 and 3)
	// a decomposition always increases the type count and invents information an area of 16 can be split into 4x4 or 2x8, you 'invent' which.
	//    - we prefer 4x4 because it is one Mearuse (4x with count 2, vs 2x and 8x)

	// perhaps the op is always 'project', the changes come in order of elements, and how they are lined up
	//    - two segments lined up by start and end points project addition. Lined up start to start projects difference.
	bool IsInverted { get; }
	int Repeats { get; }
	// compose is sticking multiple things together as a single thing with multiple combined types. LxW=Area m*m=m^2
	// If you run the calculation (Area), the type becomes m^2 (or m*m, they are the same type, m^2 just means 'm* count 2' and has 2 parameters).
	// if you don't run the calculation, they are still composed, the type is still m^2, just you retain the parameter information (6x*7m vs 42m^2)
	// alternately, if you collect elements together without composing them
	Measure Compose(params Measure[] measures);// if inverted decompose
}
public interface Type
{
	// A type is always 'one' thing, if part of it moves, all of it moves. When combined with a second thing, it becomes a new 'one' thing.
	// the combination can increase the type count (decompose), decrease it (compose), or keep it the same (+,-, invert) 

	// There can only be one op in a type, and perhaps it's always 'project' (or inverse project)
	// Ops are as simple as inverse-compose or compose. They can't be +- if they don't do type escallation/reduction.
	// because addition only occurs with one param (2m+2m ok, 9m/3s+4m/2s not ok, but 3(m/s) + 2(m/s) = 5(m/s) is ok because m/s is one unit)
	// if you want to multiply 3(m/s)*2(m/s), you have to convert that to (3m/1s)*(2m/1s) = 6m/1s

   	// Ops are always one or the other (LxW or m/s), never both (m*L/s)... *however*,
	//    each of the units are units themselves, so e.g. density is mass/volume, but volume is LxLxL, so density is mass/(LxLxL)
	Op Op { get; } 
	int[] Type { get; } // this is like speed/time
	int[] Calibrations { get; } // this is like meters/seconds
	int[] Counts { get; }
	// lookup table for meanings
}

/*
Order of elements

1PT
* point (no internal information)
* repeat a group of points in a single spot (which becomes a point due to information loss) There is a scalar count, but it is meaningless.
** from a 2PT perspective, there can be multiple seperable points at different locations on the line
** from a 2PT perspective, we can group points
** from a 2PT perspective, we can decompose a point into a group of points (inverse group, which invents location information)
2PT
** project one point to another, creates a 2 point type which has a scalar length. This is the 2PT space
** repeat this 2PT length. There can be many repeats, but the 2PT perspective can only see within itself.
** we can convert a segment into 2 points (which loses information because we can't track the point pair without a connection) (/)
*** from a 3PT perspective, we can group multiple segments, aligning with start or end points (repeat count)
** from a 3PT perspective, we can merge these groups of segments into a single 2PT segment that is longer (scalar) than others. (+)
** from a 3PT perspective, we can create a segment from one end point to another, making the diff (-)
*** from a 3PT perspective, we can decompose a segment into multiple segments (which invents location information) (*)
4PT
**** project one segment to another, creates a 2 seg type.

T
T,T->(T,T) (group) param 2T, 5T etc
(T,T)->T,T (inv group)
(T,T)->T (combine, sequential)+   // make one A+B=T
(T,T)->(T =T) (inv combine, parallel)- // make equal A+N=B // the diff is the adjustment to make the two indistinguishable, the result is T but we care about N
(T,T)->TT (compose) * T^2 etc
TT->(T,T) (inv compose) /

pt is a measure with no internal meaning (all ops don't change it)
m is a two points joined (mult)
3m is an 'm' unit repeated 3 times (add)
mxm is two 'm' units composed (mult) into mm, making it conceptually the same as m (a composed unit is still one unit)
3mx3mx3m is 27mmm - the unit is m^3 (composed), the count is all the units it creates added together (add)
the unit is always the multiply/divide op, the count is always the add op because you can only add in the current dimension, and must mult/div to escape/generate it
add is duplicate, and grouping those duplicates as one item is combine/project (mult) with compatible types
if the types are distinguishable (m and s, or even mH and mV) then you need to change levels up (mult) or down (div) to preserve the difference
scalars can be considered the same type (add) or different (multiply) - they have no type so you can choose?
 - they behave the same in (sc, unit), need to mult or div. It is (sc,sc) that should only add, but can multiply. hmm.
 - 'of the same type' is independent of the unit type, as meters can sqaure (m*m) or add (m+m) based on orthogonality, not type. But that requires a difference.
 - the difference doesn't have to be specified for the equation to work though, so it can be assumed I guess? It is implied in the result type, except in scalar mult.
 - perhaps there needs to be a marker, or the combine has an 'as the same type' indicator.
 - or a scalar*scalar results in the type scalars per scalar, or scalar^2. It might make sense to track that for interpretation.
Q. if constructed via duplication it has to add, if separate found/created things, it multiplies.
accelleration is m/s^2 - the seconds here are distinct as the 1st is the elapsed seconds and the 2nd is the measured second. It allows non zero measure points.

with compatible units, your options are (a+b), (a->b), (b->a), (a->0, b->0), these create 1, (1,1 len b), (1,1 len a) and (0,0) which is 0
this is (both,b,a,neither). This is add, subtract, subtract inverse, and add inverse. +,-,-,0
you can only do these additions with identical unit types, and they must be consolidated to one unit (an area, not a w*l)
if you have different types, you can group them but they don't merge (2 apples + 3 oranges = 2 apples and 3 oranges). 5 fruit is a new type.

numbers are 2 unconsolidated types, a scale (numerator) and unit (denominator). This allows calibration and keeping track of projection scales.
	- you can say 7 meters rather than 7 distance (which is true, but is uninterpretable without a unit). Like a non count noun, a long line is still a line.
complex numbers are 4 unconsolidated types. All the math is done without ever combining them.
 
Similar things are similar across levels. Like 2m and 2m are more similar than 3m and 2m, because the unit (L2) is the same in both, but the length (L1) is different in the latter.
L0 same position, L1 same lengths, L2 same ratio/areas. Assuming a pair of L2s, areas the same, but lw is different. Or lw same but position different.

 */