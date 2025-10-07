namespace Forces.Primitives.Electricity.Simulation.Circuits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
	using Forces.Primitives.Electricity.Gradients;
	using SpiceSharp;
    using SpiceSharp.Components;
    using SpiceSharp.Simulations;
    using SpiceSharpParser.ModelReaders.Netlist.Spice;

    public class VIR_Man_Circuit : CircuitBase
    {
        private List<VIRP> _samples = new List<VIRP>();
        private VoltageSource _voltageSource;
        private Resistor _r0;
        private Resistor _r1;
        public VIR_Man_Circuit()
        {
            var r0 = 1.0e3;
            var r1 = 2.0e3;
            _voltageSource = new VoltageSource("V1", "in", "0", 0.0);
            _r0 = new Resistor("R1", "in", "out", r0);
            _r1 = new Resistor("R2", "out", "0", r1);
            Create();
        }
        public double Resistance0
        {
            get => _r0.Parameters.Resistance;
            set => _r0.Parameters.Resistance = value;
        }
        public double Resistance1
        {
            get => _r1.Parameters.Resistance;
            set => _r1.Parameters.Resistance = value;
        }
        protected override void Create()
        {
            var result = new Circuit(
                _voltageSource,
                _r0,
                _r1
                );
            Model = new SpiceSharpModel(result, "CircuitVIR");
        }
        public override void Run()
        {
            _samples.Clear();
            for (int i = 0; i < 3; i++)
            {
                RunDC(1.0, 10.0, 1.0);
                Resistance0 += 1000.0; // increase R1 by 1000 Ohm each step
                Resistance1 -= 100.0;
            }
        }
        private double _resolution = 10000000;
        public void RunDC(double startV, double endV, double step, string currentProbe = "V1", string voltageProbe = "out")
        {
            var dc = new DC("dc", "V1", startV, endV, step);
            foreach (int exportType in dc.Run(Circuit))
            {
                double current = -dc.GetCurrent(currentProbe);
                double voltage = dc.GetVoltage(voltageProbe);
                var sample = new VIRP((long)(voltage * _resolution), null, (long)(current * _resolution), null);
                _samples.Add(sample);
            }
        }
        public override string[]? Headers => ["voltage", "current", "resist", "power"];
        public override string ResultText
        {
            get
            {
                var result = new StringBuilder();
                result.AppendLine(string.Join("\t", Headers!));
                foreach (var value in _samples)
                {
                    result.AppendLine(new string(
                        (value.Voltage / _resolution).ToString("0.000") + "   \t" +    // voltage V
                        (value.Current / _resolution * 1000).ToString("0.000") + "   \t" +  // current mA
                        value.Resistance.ToString("0") + "    \t" + // resistance ohms
						(value.Power / (_resolution * (_resolution / 1000))).ToString("0.00") // power
					)
                    );
                }
                return result.ToString();
            }
        }
    }

}
