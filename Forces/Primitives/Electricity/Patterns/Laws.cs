namespace Forces.Primitives.Electricity.Patterns
{
	using Forces.Primitives.Electricity.Gradients;
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
		public VoltageByCurrent DynamicValue { get; private set; }
		public ResistanceByPower Result { get; private set; }
		public float Voltage
		{
			get => DynamicValue.RightValue;
			private set => DynamicValue.RightValue = value;
		}
		public float Current
		{
			get => DynamicValue.LeftValue;
			private set => DynamicValue.LeftValue = value;
		}
		public float Resistance
		{
			get => Result.RightValue;
			private set => Result.RightValue = value;
		}

		public long _resistanceResolution;

		public VItoR(float voltage, float current, 
			long voltageResolution = 1000, long currentResolution = 1000, long resistanceResolution = 1000)
        {
            DynamicValue = new VoltageByCurrent(
                (long)(voltage * voltageResolution),
                currentResolution,
				(long)(current * currentResolution),
				voltageResolution);
            var min_resolution = Math.Min(voltageResolution, currentResolution);
			_resistanceResolution = resistanceResolution;
			CalculateResistance();
		}
		public void AdjustVoltage(float voltage)
		{
			Voltage = voltage;
			CalculateResistance();
		}
		public void AdjustCurrent(float current)
		{
			Current = current;
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
			Result = new ResistanceByPower(
				(long)(Resistance * min_resolution), min_resolution, 
				(long)(power * _resistanceResolution), _resistanceResolution);
        }
	}
}
