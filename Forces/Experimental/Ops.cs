namespace Forces.Experimental;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Ops
{
    // 0D Point. Can represent a complex structure and behave in certain ways, but is treated as a element with no visible internal information.
    //   a point can hold any measure, but it has to be calibrated against an external (higher or lower) space. It can have a position or population metadata,
    //   but those are either measured in the space it occupies, or inherent in itself but not directly caluculable with the evidence it presents.
    //   This metadata doesn't make it multidimensional, it is a group of properties represented by a point. It takes merging with a different type to move up a dimension.
    //   This merger can be two types, or the same type repeated in time, like m/s or m^2. Just m*m or m+m is still the same dimension as the two are merged into one of the same type.
    //   Internally it can have many multidimensional properties, those create a composite type, and one of these types is a 0D object ('point').
    //   A group of 10 points unmerged is still 0D, there is no length, area etc. Like a wire frame cube is only 1D because it only has lengths, not areas or volumes.
    //   That said, if you have 10 points in a group, and two are in the same position, merging on position (2D) will give you a group of 9 points (in 2D space).
    //   Merging will relate objects along specified properties. Those properties can be internal or external to the object. A 0D point has no properties of its own.

    // 1D MultiPoint. 2+ merged points always have a path, but it isn't convertible to a length value without a unit relation.
    //   The two points are displaced in time or some other measure, they are start point and end point. If they are both 'start points', they are 0D.
    //   A path has two directions, start to end, or end to start. There is no way of knowing which is 'right' if just given two points, but both exist.
    //   Two lines can merge into one, (path + path = path), just like two points can merge into one point.
    //   Two lines can be compared to each other, with will give a ratio (length), a direction (positive or negative), and a polarity (the positive direction).
    //   Two lines can be created from three points, 0->a and 0->b, which is technically 4 points - two points can merge to a single point, and a single point can unmerge to two points.
}
