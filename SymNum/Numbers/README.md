# Numbers for Concepts
Any number has a calibrated start state and a calibrated end state. Calibrated state means it is a measure with a unit reference. Therefore a number requires four measures.

## Number Mechanics
Our numbers always assume a start state of zero, with the start unit being a mirror of the regular unit in the opposite direction. The regular unit is always considered to be '1'. Therefore our numbers only require one measure to be specified.

A fifth unit is possible, which is the zero point. This always has the value of zero, but it can be anywhere, and does not need to be derived from any of the four measures. Changing the zero point does not change the thing being measured, but it can change resulting number. Adjusting the zero point on a segment will change the start and end values, but it will not change the segment itself, nor the resolution.
## Structure
All numbers are raw numbers, but when certain constraints are met they can be interpreted as specific types of numbers.
- Symmetric numbers: any values are ok, interpreted as only partially constrained to the line.
- Unbalanced numbers: the units must point in the positive and negative directions.
- Complex numbers: the units must be the same length in opposite directions, and the measures can be any value.
- Rational numbers: the right unit must be positive, the left measure and unit must be ignored.
- Imaginary numbers: the left unit must be negative, the right measure and unit must be ignored.
- Ratio: is the real or imaginary number part of a number, specifying which.
- Biologic numbers: are numbers that are volitile, changing over time according to some internal or external process.

## Number Types
The order of measures in number encoding is as follows:
> B | A  
> D | C  
> (E is the bias or zero point)

The ABCD choices are so our rational numbers can be read as fractions on the right (A/C), and the whole mechanism can be rotated 180 to invert the perspective. Think home team and away team in football, there are two directions in this number system depending who has the ball. The 30 yard line changes physical location when the other team goes on offence.

- **Raw Number**: four measures and a zero point, there is no inherent meaning for raw numbers, but ABCDE positions are important in calibrated numbers. 
- **Ratio**: Any to measures with the first divided by the second.
- **Rational Number**: Our numbers, positive right, expressed as a nuerator(A)/denominator(C) fraction with the denominator denoting resolution. The left denominator mirrors this one, and left numerator is 0.
- **Imaginary Number**: The postive left side of a number, expressed as a nuerator(D)/denominator(B) fraction. The right denominator mirrors this one, and right numerator is 0.
- **Complex Number**: The rational (end point) and imaginary (start point) combined, forming a segment. The denominators are mirrored, having the same resolution. 
- **Unbalanced Number**: A complex number where the denominators do not have to match resolution.
- **Symmetric Number**: An unbalanced number where transforms act as forces when multiplying, allowing a symmetries to be expressed and linking to higher dimensions. *This is probably just extra information for unbalanced numbers, but haven't yet unified complex and symmeteric calculations (maybe this already correct, in which case symmetric numbers will just replace unbalanced numbers).*
- **Equations**: Equations are just numbers with more complex internals that 12/1. A linear segment is a primitive number, but there are other internal relations, powers, mods, accelerations, sin/cos, intermediate refs etc. These all result in segments that are non linear, but still continuous at some resolution.
- **Probabilities**: Number segments and paths also do not have to be crisply delinieated. Prediction requires dealing with uncertainty, some of this can be handled with resolution, but with more data, probablity flows naturally out of trying to reduce data to equations. Reality comes from things that are both ordered and random/unknown/ignored, and probabilities are how we encode the second part.

## Constraints
All measures have a *confidence* level. If the value is **set**, we are pretty sure it is right (m=10). If it is **derived** from a calculation that is fully defined we are also pretty sure (m=a/b). If it is unknown, the certaintly propogates through equations (m*?=100). This results in a constraint - we don't know the value of m, but we know 1) it is important because it was stated, and 2) it is constrained by whatever the value of ? is, to have product of 100. This constrained equation is a valid number, it just a larger constraint. If m is a segment from 0 to 12 (12/1, a rational number), or from 4 to 12 (-4i+12, a complex number), or a segment from 4 to max (>4, an expression), these are all just constrained segments. The expression m*?=100 is constrained to a hyperbola (in 2D viewing), which is no different than 12/1 as far as this system cares. Lastly, points are just a special case of complex numbers, a segment from 8 to 8 (-8i+8), so aren't mechanically any different. Te expression >=8 may or may not be a point, but the point isn't given special 'type' consideration.

Generally we overcompute things in our math, reducing equations to results, destroying information in the process. An unsolveable equation is a valid constraint on a system, so it is very useful in defining a concept.

## Resolution

Resolution is determined by the denonminator, so 2/4 is a higher resolution version of 1/2. Unbalanced and symmetric numbers can have different resolutions internally. Operations on mismatched resolutions need to have rules to convert. Sometimes the lowest resolution wins (about a mile plus 1.03 inches is about a mile), and sometimes the resolution increases. A running average just adds numerators and denominators. If you ask different classrooms how many students like pizza, one class may be 18/21 and another 28/32. The average would be 46/53. No division needed, the answer is 46/53, not 0.8679.

Generally we overcompute things in our math, simplfying equations to make them easier to calculate and communicate (which is good), destroying information in the process (which is bad). Reasons to reduce resolution are not caring, not being able to measure accurately, or not wanting to encode very large values. Note that the best way to reduce storage and complexity without losing resolution is to encode a relation as an equation rather than values. If you know area is length X width, you don't need to store the area at any resolution.

## Bools and Queries

It is desirable to scale numbers from bools all the way to probabilities without changing the substrate. Language requires this to encode reality, and math will too. The simplest test is to compare two Numbers (which are some sort of segment path). There are two parts to this - what they will be compared for, and the interpretation of the result. In the simple case we can ask if two Numbers hold exactly the same values, and answer yes or no. There is only one answer.

A more complex comparison is to compare the overlap of two segments. There are many possible answers here:
- not touching, A is fully to the left of B or fully to the right.
- nearly touching, A might be close to B, by some metric.
- overlapping, A can overflow left or right.
- containment, A can be fully enclosed by or fully enclose B. Answers can be true but partial in either direction (6 ~ 3->10 is more true than 25 ~ 3->10)
- maybe: A has a good change of any of the above
- inclusive: We may care if A > B or A >= B. Or if A and B form a continuous segment with no overlap.
- partially true: We know A and B start the same, but don't know how A ends
- error state: when an equation is computable but the answer given doesn't match (this can be normal).

The encoding for these queries look exactly like Numbers (Equations), just the answers may not look right. For example 3x4=12 would be true, while the equations 3x4=20 would be false. If an equation results in the segment 2->8, setting the result to 6->12 will solve to 'overlapping' which may or may not be true, depending on the question. You can set the result as easily as setting the Op, or the First and Second parameter in a BinaryEquationNumber. This may lead to strange looking equations (2+2=5), but that is information (it is incorrect, and we can understand and correct the error).