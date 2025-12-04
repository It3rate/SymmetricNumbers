using SymNum.Numbers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Steering.Environment;

public class EnergyStore : Number
{
	// left side if food store, right side is energy store, zero is current state
	public Measure Energy => Measures[0];
	public Measure FoodMax => Measures[1];
	public Measure EnergyMax => Measures[2];
	public Measure Food => Measures[3];
	public EnergyStore(long foodMax, long energyMax)
	{
		Measures[0] = new Measure(energyMax / 2); // energy
		Measures[1] = new Measure(foodMax); // food max
		Measures[2] = new Measure(energyMax); // energy max
		Measures[3] = new Measure(0); // food
	}


	// these should be expressions
	public void AddFood(Ratio amount)
	{
	}
	public void IdleEnergyUse(Ratio amount)
	{
	}
	public void MuscleEnergyUse(Ratio amount)
	{
	}
	public void DigestEnergyUse(Ratio amount)
	{
	}
	public void DigestFood(Ratio amount)
	{
		// food use
		// energy use
		// energy gain
	}
}
