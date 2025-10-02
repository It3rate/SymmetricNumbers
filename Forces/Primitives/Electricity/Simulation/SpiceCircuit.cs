using SpiceSharp.Components;
using SpiceSharp.Simulations;
using SpiceSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Forces.Primitives.Electricity.Simulation;

public class SpiceCircuit
{
	private Circuit _circuit;
	private VoltageSource _voltageSource;
	private Resistor _r0;
	private Resistor _r1;

	StringBuilder _log = new StringBuilder();
	public SpiceCircuit()
	{
		var r0 = 1.0e3;
		var r1 = 2.0e3;
		_voltageSource = new VoltageSource("V1", "in", "0", 0.0);
		_r0 = new Resistor("R1", "in", "out", r0);
		_r1 = new Resistor("R2", "out", "0", r1);

		_circuit = SampleA();

		_log.AppendLine("voltage\tcurrent\tresistance");
		for (int i = 0; i < 3; i++)
		{
			var dc = new DC("dc", "V1", 0.0, 5.0, 1);
			foreach (int exportType in dc.Run(_circuit))
			{
				double current = -dc.GetCurrent("V1");
				double voltage = dc.GetVoltage("out");
				double resistance = voltage / current;
				_log.AppendLine(
					voltage.ToString("0.000") + "   \t" +
					(current * 1000).ToString("0.000") + "   \t" + // mV
					resistance.ToString("0.000")
					);
			}
			_log.AppendLine("");
			_r0.Parameters.Resistance += 1000.0; // increase R1 by 1000 Ohm each step
			_r1.Parameters.Resistance -= 100.0;
		}
	}
	public string GetOutput() => _log.ToString();
	private Circuit SampleA()
	{
		var result = new Circuit(
			_voltageSource,
			_r0,
			_r1
			);
		return result;
	}

}
