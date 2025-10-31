using SymNum.Expressions;
using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Concepts;
/// <summary>
/// This is a single aspect of a concept, which can be collected into a full concept or separated from a concept.
/// E.g. A table is something with 4 legs, a flat surface, a place to work, a place to gather, etc. 
/// This would represent only one of those aspects of table, like flat surface,
/// and this unit could also be used in 'plateau', 'floor' or 'parking lot'. 
/// Not just the unit, but also the equational expected behaviour.
/// Note that a unit is mechanically compatible to any other concept. 
/// A pin is not very table like, but you can try to apply it in ways you can't creativly apply bad grammar.
/// (E.g. you probably know which end of the pin is the 'table side'. Where 'flat is side pin this' is just wrong.)
/// </summary>
public class PropertyUnit
{
	public string? Name { get; set; } // temperature could be C or F, they would need to map somehow to the same concept
	public object? SensorySource { get; set; } // like vision, touch, sound, emotion, spatial, etc. or abstractions of these

	/// <summary>
	/// the unit value used to calibrate whatever numerator (instance measured value) is used.
	/// These can be more than just single numbers (meters per second sqared, color temperature, jealously, etc).
	/// This is specifically a number, not an extent, and a type needs to include its opposite, in that everything is defined against something.
	/// This allows happy/sad, happy/angry and happy/meh to all cooexist.
	/// </summary>
	public RawNumber? Denominator;
	/// <summary>
	/// Not sure how this works, but we have more than one calibration for a concept, and we can translate them roughly.
	/// I don't think there is a universal 'dollar' or 'temperature' that canadian dollars and celcius get converted to,
	/// but there is possibly a dynamic 'set point' for concepts formed by habituation. Likely we carry multiple ones, like
	/// a bilingual person can translate, but aren't constantly translating to a single preferred language.
	/// This is to suggest there is some way to calibrate similar concepts to each other. Even if this isn't how 
	/// our brains work, it might be useful to have a universal high res (arbitrary) temperature scale stored as a long, and have
	/// all temperature measurements calibrate against it. (I hope not? not sure.)
	/// </summary>
	public Equation? TransformFromDefault { get; set; }
}
