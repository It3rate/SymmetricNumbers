using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Experimental;

public enum Certainty
{
    Unknown,
    Partial,
    Certain,
}
public enum Relation
{
    Unknown     = 0,
    Duplicate   = 1,
    Mirror      = 2,   // doubles size (1i+1)
    Negation    = 4,   // doubles size, avg 0 (-1+1)
    Center    = 8,   // internal downcast (2pts->1pt)
    Repeated    = 16,  // internal upcast (1pt->2pts), like doubled at simplest
    UnitDerived = 32,  // derived from scalar by unit
    Sibling     = 64,  // an unrelated point in the shared dimension
    External    = 128, // point from external dimension
    Downcast    = 256, // point from a higher dimension downcast (eg center of volume)
}
public interface Measure
{
    Relation Relation { get; set; } // default Sibling
    Certainty Certainty { get; set; }
    long StartRange { get; }
    long EndRange { get; }
}
public interface Num
{
    // A measure can be unknown, or partially known (as in an algebraic expression). An uknown measure is at least 1D, as a point has no size or position
    Op Operation { get; } // project, decompose, invert
	TypeInfo Type { get; }
    int Dimensions { get; } // calculate from type info
    float Size { get; } // length, area, volume
    float[] Positions { get; } // center point for length/area/volume etc
    Measure[] Measures { get; }
	//int Repeat;
	//int Param; // the parameter is just a repeat count of the unit, a power is the repeat count of the op 
}
public interface Op
{
    //Measure Input { get; }
    //bool IsInverted { get; }
    //bool IsNegated { get; }
    Measure[] Eval(Measure[] inputs); // ops may cast to other dimensions

    // a segment can only add when projected as there is no unit. if it projects from the end of other it is added, from end of self it is diff.
    // the result will always be zero based, as there is no unit to anchor to.
    // a ratio (numerator/denominator) can only multiply when projected as there is a unit and things need to align.
    // to add ratios, you still need to align, but then just use the numerator, which is a segment and thus has to add.

    // a projection (~multiply) always lowers type counts and loses infomation (2x3=6 :: 6 can not recover 2 and 3)
    // a decomposition always increases the type count and invents information an area of 16 can be split into 4x4 or 2x8, you 'invent' which.
    //    - we prefer 4x4 because it is one Mearuse (4x with count 2, vs 2x and 8x)

    // perhaps the op is always 'project', the changes come in order of elements, and how they are lined up
    //    - two segments lined up by start and end points project addition. Lined up start to start projects difference.
    // compose is sticking multiple things together as a single thing with multiple combined types. LxW=Area m*m=m^2
    // If you run the calculation (Area), the type becomes m^2 (or m*m, they are the same type, m^2 just means 'm* count 2' and has 2 parameters).
    // if you don't run the calculation, they are still composed, the type is still m^2, just you retain the parameter information (6x*7m vs 42m^2)
    // alternately, if you collect elements together without composing them
    Measure Compose(params Measure[] measures);// if inverted decompose
}
public interface TypeInfo
{
	// A type is always 'one' thing, if part of it moves, all of it moves. When combined with a second thing, it becomes a new 'one' thing.
	// the combination can increase the type count (decompose), decrease it (compose), or keep it the same (+,-, invert) 

	// There can only be one op in a type, and perhaps it's always 'project' (or inverse project)
	// Ops are as simple as inverse-compose or compose. They can't be +- if they don't do type escallation/reduction.
	// because addition only occurs with one param (2m+2m ok, 9m/3s+4m/2s not ok, but 3(m/s) + 2(m/s) = 5(m/s) is ok because m/s is one unit)
	// if you want to multiply 3(m/s)*2(m/s), you have to convert that to (3m/1s)*(2m/1s) = 6m/1s

   	// Ops are always one or the other (LxW or m/s), never both (m*L/s)... *however*,
	//    each of the units are units themselves, so e.g. density is mass/volume, but volume is LxLxL, so density is mass/(LxLxL)
	//Op Op { get; } 
	int[] Type { get; } // this is like speed/time
    long[] Calibrations { get; } // this is like meters/seconds vs inches/minutes
    Measure Range { get; } // min max of this type
	// lookup table for meanings
}
public interface Expr
{
    // a complex number [n, di, d, ni] needs to be [ni,di,n,d] [/+/] if ops are to be in order. Is this a stack?
    // or segments are 2 points, numbers are 2 lines, complex numbers are 2 numbers, areas are 2 complex numbers, volumes are 2 areas... all pair ops.
    // or numbers are numbers because they aren't evaluated? The operations only happen when combined, and that is peice wise?
    // (a+ib)(c+id)=(ac−bd)+i(ad+bc) [na, dib, da, nib] [nc, did, dc, nid] = (),(nid/2+did/2), ()  ,(na/da*nc/dc)-(nib/dib*nid/did)  
    Measure[] Inputs { get; }
    Op[] Operations { get; } 
    Measure[] Eval(int repeatCount);// run the ops by pulling inputs off the stack as needed, halt when complete. Output->input per repeat count
    Expr[] OutExpr { get; } // used for stops (empty), segmenting (one), as well as branches (multiple).
}
public interface Repeats
{

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
 - you can only multiply a scalar with a unit, not add. So maybe multiplying 2 scalars is just addition to a power (repeated) vs multiply
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

Any algebraic equation is a numbering system. The rule is it can't be further broken down or added to without becoming something else.
A numbering system is 'at least' one free variable, and a new result whenever it changes. 
Rational numbers are A/B, both can change, they can be squared etc, but the equation doesn't change.
Complex numbers are Ai/B + Cr/B (Q. is this the same numbers as quadrant math? Should be, just i is vertical and areas have polarity, A. yes, but no subtract in the multiply)

Like properties can multiply, but they must be distinguishable. This is often a spatial or temporal distinction. But there are others.
 - m^2 is spatial (directional)
 - ms^2 is temporal (a sequence, one calculation based on the previous calculation)
 - countable objects can not be merged, so you can imagine 3 apples times 4 apples being a grid of 12 apples (this is actually repeated addition though)
 - ranges pulled from a higher dimension are the same 'stuff' but can be multiplied. 
		A color space is 3D, but a color range is 1D, so you can multiply ranges to get areas. E.g. a blend of blue to white can mix with
		a blend of red to yellow, and create an area of possible color.The ranges are 1D paths through the 3D color space. They are the same type,
		with different paths. Perhaps this could be considered different locations, but ultimately location diff is always sub parts of a higher dimensional space.
		Q. So perhaps the only way two 'like' types can multiply is if it is done by the higher space?

Properties can always be merged, but in the countable case, the definition relies on properties being in certain ranges, so it will destroy the definition.
1+1 apples does not make a larger apple, but it could. If you smush them together as applesauce, you get more applesauce with each apple you add.
Merging is always 1<->1=1, and splitting is always 1=1<->1. We define objects by ranges of their unmerged properties.
Apples are red on the outside and white on the inside. You can merge those in the full range of the skin to core space, but you'll no longer have an apple.

Addition is 1+1=1, multiplication is 1x1=1, subtraction is 1=1+1, division is 1=1*1. lines merged with lines create a single thing, whether an area or a longer line.
These are all the same op, and only with units (denominators) can you get values. 
7m/1m is scalar 7, but 'seven meters' as we think of it is the uncomposed 7m/1m. The 1m is essential to determine the 7ness of the compared length.
That's why measures need to be rational numbers, they always have a numerator and denominator of the same type, and they can't be composed.
Number systems need to be algebraic expressions that are not composable, otherwise they lose their meaning.
The denominator type has to be in the numerator type somewhere, otherwise the result it null. 
Dividing a type by a scalar is null, only scalar/scalar is ok.


A case to consider, two perpendicular lines can be an 'L' path or an area, depending if they are added or multiplied. 
For paths, they are tip to tail, for areas they are connected at the start point.
In this system, the path version would have to be the same type, therefore only 'addable'. Areas would be different types.
Addable lines would potentially need to move to align, whereas areas would need to both move and rotate.
If the lines were curved, they would probably also need to be straightend for area. 
This is because area is combining lengths, it isn't a geometry thing. Multiplication combines two continuous properties, so it isn't rotating etc, it is just extracting a property..
Addition combines two discrete objects, so there is no transform. Really you are adding lengths as well (the reason a line isn't two points is it defines a length, not points).
So the idea that the lines are curved means they are viewed and encoded in a higher dimensional space. 
If they are just lengths defined by two points, they don't know about their curvature, and can only interpret themselves as lengths.
Any adding or multiplying is done in the space they exist in, which is length only.
To say a connection of two points is anything more than a length is just an error, as that requires more than two points.
There are number systems that handle curves (with more points) and they have addition and multiplication rules of their own.
(Q. symmetric numbers feel like the fit here, but can those work intuitively as areas too? 
Maybe human intuition requires a crude transformation to familiar spaces to be useful.)

If you join orthogonal points tip to tail (add) for each, you get a rectangle (area) when attaching the zero points.
You get a volume by attaching two mirrored rects at the zero point if one is rotated 90 degrees.
These both allow the start point and end point to be in mirrored corrdinate systems.
This seems right, as every space needs a start and end to exist in it's wrapper space, regardless of it's complexity.

Area defines the space, but to create shapes in it you need multiple segments in it. 
These segments only create an enclosed area when satisfy certain constraints. 
Q How can these be defined to always have a valid area? Or does this matter? Like total vertical and horizontal motion must equal zero?

Also, how can these measures be 'smarter'? A good start might be to always heal into an enclosed area.

What if.
When you create a space, you combine to elements in an existing space, but use variables instead of merging them. (Ai+B)
* This isn't something that creates a line/area/volume -- it allows you to define points in this new space.
* These points you define are ordered, and they have new levels of metadata.
So two points become a line space. This allows positioning a point on the line, and the point is 'weighted' base on how much line is on each side.
Segments in a complex line are positioned at their midpoint, and the length determines their 'strength' or weight, or certainty etc.
Areas in 2D complex grids are diamond shaped, but again, this is the 'weight' of the center point, not the area. Same with volumes.
The advantage of this is there are now many ways to define the same point. 
This makes 'smart' points possible, because they can make choices without changing locations, and their values are flexible based on other input.
** You can not have a smart point if it has only one possible configuration in its required position.
This could also allow bezier style acceleration and curves, based on the weight of the points.
This could make shapes always be valid, as the shape is defined as the enclosing of the points, 
and that happens automatically when point count >- shape dimension, because the points are ordered so there is no abiguity.

todo: Check if contraditions in spaces map to contratictions in symmetric numbers causing new dimensions.
e.g. tip to tail is addition, but a line that is defined with two start points (impossible), or start and end in the same spot (can be forbidden?)
would mean it breaks into a new dimension. Like two resaturants in the same location on a map suggests a multi story building.
in complex numbers this could be when e.g. both units are positive. Maybe the symmetric breaking cases need to look at polarity of units for the break.

You don't use the complex plane to create rectangles. Those rectangles help isolate a location with metadata, but it is still a location.
A location in 3D always has a volume, in 2D always has an area, and 1D always has a length. It can be small, but not zero (or it would be a lower dimension)

When creating points in a space by statistical centers (midpoint, center of area, 4 point volumes, etc), the excess data gives
flexibility and choice to the layer's object. 
Intelligence needs redundancy to give it room to move.
This metadata can also come from above or below, like an area space trying to align 3 shapes.
The negotiation of properties from all these forces will adjust the element, and then the new pressures are calculated and iterated.
* You can precisely precalculate things, or you can loosely iterate with intelligence, but you can't have both.

Every layer space can create/record elements in its space, but it can also have opinions about the pattern those should be in.
A line can create segments, but it can also want those segments to be evenly spaced.
This pattern ultimately comes from experience, as it tries to map its elements into patterns that give it stability.
The nudges to a space can also come from above or below, like a volume space trying to align 4 areas.
The volume can communicate with the area or line space by collapsing volumes to those spaces, and appending those desires to the local computations.
Collapsing and expanding always has 'rules', as information is going to be lost or invented, forcing choices.
Rules to expand or collapse are more mechanical though, and have normalish defaults (collapse an area to its center point etc).
There isn't necessarily a conversion that happens between layers. An area looks like a line to the layer below, and a line looks like a thin area to the area layer.
A point layer will see a volume as a point, as that is all it can see. Like a city on a map is a point. There is a collapse, but it is mechanical.
Layers can only interpret elements from their own perspective, but it doesn't mean other layers are invisible.
Patterns however are varied, and dependent on the domain they represent. Symmetry is good in a balancing system, but bad in a escalation.
These patterns ultimately are represented by equations of some sort, which are in reality just numbering systems of their own.
Each layer can have its own patterns, and the directionaly is two way. The same pattern can be interpreted by each layer in its own way.
The only rule is the layer can only have patterns and knowledges of its own layer, it can only broadcast its desires to others.


if a space can encode place, size and direction, there needs to be multiple ways create those properties in the space. 
Like if the position of an area is the center, there are a lot of ways to create that position 
(with different sized/shaped areas can have the same center). 
There is also lot of ways to represent a size (at any location, any shape). 
You can't expect a layer to be 'smart' without giving them options on how to accommodate requests, 
so this redundancy is actually a key feature. 
Like using XYZ to create a 3D point gives no options, but if the location is the center of a box in 3D space, then there are choices.
Layers have to be over-specified to have agency. 

In geometric terms, each complexity layer (points->lines->area->volume) can only create and see elements of it's type, 
but it sees them as weighted positions in its space (size and place) with direction (start and end). 
Two points that are separable (impossible in the point space) create a line, 
two identical lines of different length (also impossible) create an area space (rotating off the line changes the length),
two identical areas of different size must be orthogonal to create a volume space etc.
In volume, you now have 4 points to create volumes in this space (and 4 more to move the center point around), rather than 3. 
This means you can create and encode positions in 3D space in many ways, and they not only have different behavior,
but they affect the adjacent complexity spaces differently. 
You can imagine using 'area points' in 2D to create area splines (e.g. smaller area means sharper corners)
no matter what you do you will always have an area in this space because your 'points' are areas.
Even an area of zero volume is an area, as it still has the concept of volume - 
just it is more trivial to convert to a zero length line, or point. 
1D spaces see area as a line (point and a length), and as they push around elements in the line space,
it affects whatever 3D space the lines are related to.
Volumes in the 3D layer have no idea if they were made of polylines, circles or rectangles, as they only have a place and volume. 
This makes it easy to treat that complex layer as a simple point layer, and you can start from there. 
So there is upward pressure, downward pressure, and any level can be considered the lowest 'point' 0D level.

The trick is the position of something ultimately depends on all of these layers at once. 
An area may want to be larger, and that puts pressure on the lines that made it, 
and if a line wants to be longer it puts pressure on the points below and shape above. 
The 'want' here is to satisfy a pattern. 
E.g. 'evenly spaced' or 'ordered by size', or 'symmetrical around the center' etc.
These patterns are able to be expressed in each layer in their own way 
(maybe akin the the bioelectric signals being able to work across many layers of complexity). 
And these patterns are independent of layers, they come from logic, order, memory, and ultimately experience in the environment. 
Like, when things are evenly spaced, the element lasts longer, so evenly spaced things dominate that environment. 
Or when contract and expand happens in a certain pattern based on light input, an organism moves toward food. 
Or when bolt elements are distributed in a circular pattern, the stress on them decreases and their volume pressures subside,
which in turn causes their cost to decrease. Across many layers of interest.


In 3D, the two points that form the corner of the cube are center points of the areas. 
The areas are constructed with two midpoints points of the line segments. The lines are centers of the two points (eg the points).
The layer above only sees the layer below as points.
Q. Does it make sense to use the zero points, and allow offset sizes? 
The size would be created and moved in separate steps this way, and the moved location is the 'point' the above area sees.
I guess the layer above could query size, location, center of mass etc?

Motion is interpreted within a range. A fly on a train ignores the train, earth, sun motion, as well as the molecules, swatter kinetics etc.
This allows a linear computable range to work in, and ignoring things that are too distant, complex (cancelling) or scaled to be relevant.
We model many things at the levels we care about, speed, acceleration, predictive curves, timing, forces, the fly's mind etc.

Acceleration or area indicates one value affecting another. This is a product. You can have multiple products at once (eg volume)
A branch is an additive combination. There are multiple values that combine, but they don't affect each other.
Logic is boolean combination. These are non-linearities in equations, and can move up or down levels. (2 segments -> 0, 1 or 2 segment result)
The last op is random/unknown/don't care, and most calcualtions have some portion of this. 

Q. Does the line level or the area level determine the shape?
- The line level does not have to be linear, just a path. So a polyline that eventually connects to itself has some say (eg number of sides)
- Only the area level can see the overall size and shape.
- We ultimalely only model changes. An equation will continue to be calculated until there is some change, internal, external, or energy.
- To turn a rombus into a square, the line level needs to make the sides equal lengths, and the area level needs to make the angles the same?
- The line level could add two ghost diagonals and try to equalize them, but that makes it higher dimension (in addition at least)
- Think of making the letter 'A', there would be background energy around the top, bottom and crossbar areas, and every level would push toward those.
- An N turns to an H by the endpoints emmiting a repel field into the background energy, causing the crossbar to drift.
- A layer can try to 'convince' layer siblings, and pressure layers above and below. If the pressure causes change, that too can be contagious.


Types of computational objects:
Grouped {...} Anything inside are considered focused,bt not merged, {3,3,3} is three separate elements in order, not 9. Can contain nulls. Can be reordered? Or at least reversed with expr. 
        - Think of as 0D elements, points. They may have structure internally (like they may be expressions with many dimensions), but from the outside they are just points in a listed order.
Merged  - elements in [] are always sequenced merges. [Numerator,denominator] is always a scalar, [n,n] is addition, [nd,n] is multiplication, [nd,nd] is *power, [d,nd] is +power. [dd]=[1]
        - Sequence is always in order, so 3x^2 needs to be [x/1,2/1,3]
        - Types affect merge. AdBd becomes area, AdAd is accel. AA is two lines into one, AB is tip to tail but stays separate lines.
Bool    - bool ops are two inputs with one of 16 bool ops. Needs to also support lower dim ops, like >, <, ==, != etc. And higer dimension ops? A form of expr with an auto eval?
        - Perhaps all ops are derived from their dimesnions, and bools are 2D. GT, LT, addition would be 1D, mult would be 3D [ndn] due to needing a unit to stretch, and powers 4D [ndnd]
Eval    - an expression is always evaluated, but if it contains variables it can't be. feeding a group into an expression maps the input elements to variables.
Cast    - moving to higher or lower dimensions. This can happen naturally on eval. Merging two types goes up a dimension, and a unit with a lower dim type goes down (mss/s = ms)


Compared
 



The reason 'two areas make a volume' has redundant values is because it is very possible there is no overlap where the two areas are trying to intersect.
 - Imagine two areas with rectangles in them. As they are brought orthogonal to each other, they project the rectangles into each other's spaces. There may not be intersection there.
 - If one area is color and temperature, the other is melting point and density, it is very possible there is no intersection at all for certain combinations.
 - This doesn't mean there is no coorlation and the values have no reason to be merged, just that intersection may be empty.
 - With traditional xyz, that isn't possible unless one value is zero. But logically it is possible that no values are zero, and yet they don't correlate at those values.
 - As you add more dimensions, that gets more likely in fact.


Sequence of twos:
2 pts -> line
2 lines -> rational
2 rationals -> complex (segment) (4 pt units, 4 pt measures)
2 segments -> area
2 areas -> volume (may be null)
2 volumes - 4D (motion intersect, etc) (64 measures - 32 units, 32 values)

a volume from 2 areas only guarentees an intersection if the two areas share an axis (as we traditionally imagine it ish)
we think of XYZ as three axis, but really it is the XZ, and YZ planes intersecting (or any combination of two, as long as there is a shared axis)
Q. Can the shared axis be thought of as the unit? A complex number has two mirrored (orthogonal) units, that are the same size. This seems 2D akin to two areas with a common Z?


Q. When creating an expression, should the order of the inputs be the order in the expression, or should it pick and rearrange?
- an expression stack always has a set order, are there examples where that makes certain desired expressions impossible?
- Assume these expressions are nested, so an area is made of two complex numbers (A1 AND A2), which are made of two numbers (Ai+B), which are made of two segments (N/D)
- Here you have the order choice when you construct the value, maybe this is enough.
- Q. Are there cases in one expression where you want to reuse a value in a new way? Like A*B+A? Perhaps these are two expressions? 
-    5A²+3A+4 is common, but it is multiple levels of a value interacting, so this is expected.

Q. Perhaps moving a point is always an operation on 0D, but enacted from a higher dimension. Like moving it along a line is from 1D, moving in an area from 2D etc.
you can also move a line in an area space, or an area (face) in a volume. All operations are acting on one higher space on a lower space.
Q. Does this allow a lower space to also move an object in higher space? In a sense, in that if a line moves a point, and a point is part of an area, the area is affected.
   but it is hard for a point in its own space to tell an area how to move, because it doesn't know about area mechanics.


Each level is based on points, paths, areas, volumes etc. This instance is the bounds that can take samples.
The actual values are always from samples, not bounds points themselves. 
So if you have an area, area to point is a long, perhaps in the center.
Area to line is two points sampled from the area, making a line.
Line to point is a point on the circumference path of the area (a long)
Line to line is two points on the circumference of the area joined.
* You can't get a value without a sample. The sample can be the end point etc, but it is still a sample.
An op merges two spaces, which can increase dimensionality (*), decrease (/), or keep it the same (+,-).
Two areas can increase size without knowing the shape. Two lines change length without knowing position.
The shape or position can be negotiated with the other layers, even if the size/length is fixed.

Q. can you treat + as adding a branch, and * as appending length to the same value?
The length of 2+3 is still 5, but conceptually it would be two separate lines, joined at a point. 2+3+4 would be a fan out.
A. Maybe they join like this, but not as branches? and only become one unit with a merge command (*1?)
   Probably isn't correct always, but adding unlike types does give a total length without merging (2 apples + 3 oranges = 5 total)
   The command for a branch requires a more explicit symmetry breaking.

 */