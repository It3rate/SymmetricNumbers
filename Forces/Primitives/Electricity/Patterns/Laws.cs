namespace Forces.Primitives.Electricity.Patterns
{
	using SymNum.Patterns;
	using System;
    using System.Collections.Generic;
    using System.Linq;
	using System.Numerics;
	using System.Text;
    using System.Threading.Tasks;

    public class Laws
    {
        // Patterns include RCL calculations, Kirchoff's laws, Ohm's law etc
        // reactance, impedance, admittance, susceptance, conductance.
    }
    public class VItoR : Laws
	{
		public SymmetricNumber DynamicValue { get; private set; }
		public SymmetricNumber Result { get; private set; }
		private float Voltage
		{
			get => DynamicValue.RightValue;
			set => DynamicValue.RightValue = value;
		}
		private float Current
		{
			get => DynamicValue.LeftValue;
			set => DynamicValue.LeftValue = value;
		}
		private float Resistance
		{
			get => Result.RightValue;
			set => Result.RightValue = value;
		}

		public long _resistanceResolution;

		public VItoR(float voltage, float current, 
			long voltageResolution = 1000, long currentResolution = 1000, long resistanceResolution = 1000)
        {
            DynamicValue = new SymmetricNumber(
                (long)(voltage * voltageResolution),
                currentResolution,
				(long)(current * currentResolution),
				voltageResolution);
            var min_resolution = Math.Min(voltageResolution, currentResolution);
			_resistanceResolution = resistanceResolution;
			CalculateResistance();

		}

        private void CalculateResistance()
        {
            if (Current != 0)
            {
                Resistance = Voltage / Current;
            }
            else
            {
				// todo: make sure infinity results in a paritally defined number (value can be anything within correct direction).
				Resistance = Voltage >= 0 ? float.PositiveInfinity : float.NegativeInfinity; 
            }
			var power = Voltage * Current;
			var min_resolution = DynamicValue.MinResolution;

			// Result = a gradient to resistance(+) from 'everything that caused it' (VI, or power)
			// this is opposed to resistance caused by specific things, like a change in voltage.
			// This would be useful when eg. calculating heat loss (power * resistance) where you don't care why there is resistance.
			Result = new SymmetricNumber(
				(long)(Resistance * min_resolution), min_resolution, 
				(long)(power * _resistanceResolution), _resistanceResolution);
        }
	}
}
