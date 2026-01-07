namespace Forces.Experimental;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Ops
{
    // 0D Point. Can represent a complex structure and behave in certain ways, but is treated as a element with no visible internal information.
    //   A point knows nothing about it self. It is an unknown variable, where all information can be methodically grafted onto it, eventually making it a resolved variable.
    //   3x²+2x+5=77 then x==4 in 1D space (line). 3*4=x then x==12 in 2D space (area). 4abc+6bbb+2ab+3c=x then x is property with three 1D components that have correlations in 1,2,and 3 dimensions.
    //   In all these, x is a 'point' until more information is known about it. As information is revealed, it can participate in multiple dimensions to resolve reality.
    //   This is intuitive. Heat can affect an object in many ways, combining with time, motion, color, taste etc. Heat is a point until you know the value,
    //   but the object itself is a point until you know the equations the govern it.
    //   The Heat/taste affect is a point until you know the bounds of it, then it becomes some 2D construction. A measured heat and taste can be plugged into that area to create a 2D position.

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

    public Measure[] Duplicate(Measure[] source) { throw new NotImplementedException(); }
    public Measure[] Mirror(Measure[] source) { throw new NotImplementedException(); }
    public Measure[] Negate(Measure[] source) { throw new NotImplementedException(); }
    public Measure[] Center(Measure[] source) { throw new NotImplementedException(); }
    public Measure[] UnitDerived(Measure unit, Measure[] source) { throw new NotImplementedException(); }
    public Measure[] Repeat(Expr source) { throw new NotImplementedException(); }
    public Measure[] Reference(Measure[] source) { throw new NotImplementedException(); }
    public Measure[] Merge(Measure[] source) { throw new NotImplementedException(); } // addition if same type, multiply if different (+, *)
    public Measure[] SequenceMerge(Measure[] source) { throw new NotImplementedException(); } // keep separate if different types, multiply if same type (power)
    public Measure[] Boolean(BoolOp boolOp, Measure[] source) { throw new NotImplementedException(); }
    public Measure[] Compare(CompareKind compareOp, Measure[] source) { throw new NotImplementedException(); }



}

public enum CompareKind
{
    None,
    Equal,
    NotEqual,
    GreaterThanOrEqual,
    LessThanOrEqual,
    GreaterThan,
    LessThan,
    Mod,
}

//  0000	Never			0			FALSE
//  0001	Both            A ^ B       AND
//  0010	Only A          A ^ !B      A AND NOT B
//  0011	A Maybe B       A           A
//  0100	Only B			!A ^ B      NOT A AND B
//  0101	B Maybe A       B           B
//  0110	One of          A xor B     XOR
//  0111	At least one    A v B       OR
//  1000	No one          A nor B     NOR
//  1001	Both or no one  A XNOR B    XNOR
//  1010	A or no one		!B          NOT B
//  1011	Not B alone     A v !B      A OR NOT B
//  1100	B or no one		!A          NOT A
//  1101	Not A alone		!A v B      NOT A OR B
//  1110	Not both        A nand B    NAND
//  1111	Always			1			TRUE

public enum BoolOp
{
    Never = 0x0000,
    Both = 0x0001,
    OnlyA = 0x0010,
    AMaybeB = 0x0011,
    OnlyB = 0x0100,
    BMaybeA = 0x0101,
    OneOf = 0x0110,
    AtLeastOne = 0x0111,
    NoOne = 0x1000,
    BothOrNoOne = 0x1001,
    AOrNoOne = 0x1010,
    NotBAlone = 0x1011,
    BOrNoOne = 0x1100,
    NotAAlone = 0x1101,
    NotBoth = 0x1110,
    Always = 0x1111,
}
public class BooleanOperations()
{
    public long Never(long a) { return 0; } // Null
    public long Transfer(long a) { return a; }
    public long UnaryNot(long a) { return ~a; }
    public long Identity(long a) { return -1; }

    public long Never(long a, long b) { return 0; } // Null
    public long And(long a, long b) { return a & b; }
    public long B_Inhibits_A(long a, long b) { return a & ~b; } // inhibition, div a/b. ab`
    public long Transfer_A(long a, long b) { return a; }                   // transfer
    public long A_Inhibits_B(long a, long b) { return ~a & b; } // inhibition, div b/a. a`b
    public long Transfer_B(long a, long b) { return b; }                   // transfer
    public long Xor(long a, long b) { return a ^ b; }
    public long Or(long a, long b) { return a | b; }
    public long Nor(long a, long b) { return ~(a | b); }
    public long Xnor(long a, long b) { return ~(a ^ b); } // equivalence, ==. (xy)`
    public long Not_B(long a, long b) { return ~b; } // complement
    public long B_Implies_A(long a, long b) { return a | ~b; } // implication (Not B alone)
    public long Not_A(long a, long b) { return ~a; } // complement
    public long A_Implies_B(long a, long b) { return b | ~a; } // implication (Not A alone)
    public long Nand(long a, long b) { return ~(a & b); }
    public long Always(long a, long b) { return -1; }
}

/*

level of dimensionality:
0 no new point
(pt)  duplicate of existing internal point (equal values)
(pt)  point created by higher orders:
2 points
(seg) mirror/negation of existing internal point
(seg) midpoint/aproximation (internal downcast) of multiple points
(seg) repeat of a created point with the same pattern (internal upcast)
(seg) separate sibling internal point
External
(seg) external point with relation to internal point (new dimension but corrolated)
// all the same seg points, but with external ref. eg ext point to seg midpoint
(area) point to higher dimension point (midpoint to area center)
Unit based:
point created by scalar from unit




*/