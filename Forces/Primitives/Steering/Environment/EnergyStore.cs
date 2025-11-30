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
	public IMeasure Energy => Measures[0];
	public IMeasure FoodMax => Measures[1];
	public IMeasure EnergyMax => Measures[2];
	public IMeasure Food => Measures[3];
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
