namespace SymNum.Measures.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;

// A concept describes any entity, real or abstract.
// A common concept gets a word or category to name it, but most concepts are very specific and constructed on the fly by combining and constraining existing concepts or primitive input.
// So while there is eventually a shared list of these, they are ultimately dynamically generated and never identical (because you can never step into the same context twice).
// It consists of input information, and constraints that help define it.
// Not all input information needs to be present, and not all constraints need to be satisfied.
// There are many true facts and relations we can say about a concept, but we will only use the ones that are relevant in a context.
// The concept itself is all the 'usually true' things that can stated about it, though many more things than this are derivable.
public class ConceptSpace
{
    // *** A concept is defined by what it does (forward) as much as where it came from (backward).***
    //    Power is current * voltage, but power is also the amount of work it can do, the work it does, the cost, the power grid, force, control, etc.
    //    A concept can be used metaphorically to constrian a new concept, but that will also contribute to the definition of the original concept.
    //       Power is a force, power outage, power tools, and grrrl power all contribute to the hazy concept of power by preferred relations and contstraints.
    // thus a concept is really the terminal of all the gradients it is defined against. It can have its own raw inputs, but those are leveraged to the max.
    // many concepts can constrain a space, like V=IR is one aspect of an electric field, and it helps to comprehend electricity, but it doesn't define it (nothing alone does).


    // inputs:
    //    can be raw inputs like length or lumens, or synthesized ones like area or color.
    //    can be presice measures, estimates, or time series statistical interpretations.
    // relations/constraints: 
    //    there are many of these, many are fairly obvious equationally (diff, product, ratio, etc)
    //    the common ones have names, specific ones have domain expert names, but many others are constructed on the fly.
    //    these aren't just measures, they are an active part of the concept, they can be pinned or adjusted to help define other parts of the concept.
    //    "that lake is small, but it holds a lot of water" (alludes it is probably deep, and the depth of the lake is significant to the context)
    //    A CAD drawing has 20 types of constraints it can use, but until they are used they aren't in the context.
    // context:
    //    these are relations brought into focus, sometimes to pin a value, sometimes to signal an exception.
    //    If you talk of a square, but never allude to size or area, you are not using the area relation to define the concept. Maybe it's about angles etc.
    //    Constraints can be transferred across concepts, but only the in context relations transfer. A bridge loan says nothing about arches.
    //    Picking an equation matters as much to context than the computation of it. Any two values have a sum or product, but it may not be relevant to the concept.
    // scaling:
    //    A concept always has at least one gradient (more of it, less of it).
    //    this gradient/value can be used to construct new concepts (like area from length, or speed from distance and time).
    //    You don't have to go all the way back to primitives to calculate with it, as it has its own expectations and utility.
    //    You can go back to primitives to estimate missing information. A lot of solving is filling in missing information based on partial context.


    string? Name; // optional, if it matches a general category or concept.
    List<UnconstrainedNumber> ActiveInputs; // you may only see the head and color of a dog, this makes 'leg count' not an input, though it can be inferred in ExpectedContstraints.
    List<IConstraint> ActiveConstraints; // relations between inputs, or external values.
    List<IValue> ActiveProperties; // can be inputs or derived from constraint results;

    List<IConstraint> ExpectedConstraints; // if you guess a category, you can guess other properties. Can be generated based on context.
    List<IConstraint> ForwardReferences; // things that use this concept as input help define it. Do not need to be stored like this.

	// AllPossibleConstraints are computed as much as looked up

	// There is a whole category of binary operations that represent expected motion.
	// This is akin to prepositions. The position is static, but over time the position changes, and this is expected.
	// so move near, move over, move across, move to etc are expected motions of a position,
	// and can be encoded in a second equation (or just default move left to right on a segment that is part of an equation).
	List<UnconstrainedNumber> Motions;
}
