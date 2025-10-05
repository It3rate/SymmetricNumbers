namespace Forces.Primitives.Electricity.Simulation.Circuits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SpiceSharp;
    using SpiceSharp.Components;
    using SpiceSharp.Simulations;
    using SpiceSharpParser.ModelReaders.Netlist.Spice;

    public class RCL_Man_Circuit : CircuitBase
    {
        private VoltageSource _voltageSource;
        private Resistor _resistor;
        private Inductor _inductor;
        private Capacitor _capacitor;

        private List<double[]> _results = new List<double[]>();

        public RCL_Man_Circuit()
        {
            Create();
        }

        public double Resistance
        {
            get => _resistor.Parameters.Resistance;
            set => _resistor.Parameters.Resistance = value;
        }

        public double Inductance
        {
            get => _inductor.Parameters.Inductance;
            set => _inductor.Parameters.Inductance = value;
        }

        public double Capacitance
        {
            get => _capacitor.Parameters.Capacitance;
            set => _capacitor.Parameters.Capacitance = value;
        }

        protected override void Create()
        {
            _voltageSource = new VoltageSource("V1", "in", "0", 0);
            _voltageSource.SetParameter("acmag", 12.0);// new Sine(0, 12, 1000));
            _resistor = new Resistor("R1", "in", "n1", 100);
            _inductor = new Inductor("L1", "n1", "n2", 0.1);
            _capacitor = new Capacitor("C1", "n2", "0", 1.0e-6);    // 1 µF

            var circ = new Circuit(
                _voltageSource,
                _resistor,
                _inductor,
                _capacitor
            );
            Model = new SpiceSharpModel(circ, "CircuitRCL");
        }
        public override void Run()
        {
            _results.Clear();
            RunAC(1000.0, 3000.0, 100.0);
        }

        public void RunAC(double startFreq, double endFreq, double step, string voltageProbe = "n1", string currentProbe = "V1")
        {
            var ac = new AC("ac", new LinearSweep(startFreq, endFreq, step)); // new DecadeSweep(1e-2, 1.0e3, 5));// 
                                                                              //var voltageComplex = new ComplexVoltageExport(ac, voltageProbe);
                                                                              //var currentComplex = new ComplexCurrentExport(ac, currentProbe);

            var voltageComplex = new ComplexPropertyExport(ac, "R1", "v");
            var currentComplex = new ComplexPropertyExport(ac, "R1", "i");
            foreach (int exportType in ac.Run(Circuit))
            {

                double voltageRms = Math.Abs(voltageComplex.Value.Real) / Math.Sqrt(2.0); // RMS voltage at output node
                double currentRms = Math.Abs(currentComplex.Value.Real) / Math.Sqrt(2.0); // RMS current through voltage source
                double impedance = voltageRms / currentRms;
                // Calculate phase of voltage in degrees
                double phase = Math.Atan2(voltageComplex.Value.Imaginary, voltageComplex.Value.Real) * 180.0 / Math.PI;


                //Calculate resistance and total reactance
                double resistance = impedance * Math.Cos(phase * Math.PI / 180.0);
                double reactance = impedance * Math.Sin(phase * Math.PI / 180.0);

                //Separate inductive and capacitive reactance
                double xL = 0.0, xC = 0.0;
                if (reactance >= 0)
                {
                    xL = reactance; // Inductive reactance dominates
                    xC = 0.0; // Assume negligible capacitive reactance
                }
                else
                {
                    xC = -reactance; // Capacitive reactance dominates
                    xL = 0.0; // Assume negligible inductive reactance
                }
                double inductance = xL / (2 * Math.PI * ac.Frequency); // L = X_L / (2πf)

                _results.Add([ac.Frequency, voltageRms, currentRms, resistance, inductance * 1000, xL, xC, phase]);
            }
        }
        public override string[]? Headers => ["ACFreq", "vRms", "iRms", "res", "induct", "xL", "xC", "phase"];
        public override string ResultText
        {
            get
            {
                var result = new StringBuilder();
                result.AppendLine(string.Join("\t", Headers!));
                foreach (var values in _results)
                {
                    var s = new string(
                        $"{values[0]:F1}\t" +
                        $"{values[1]:F3}\t" +
                        $"{(values[2] * 1000):F3}\t" + // Current in mA
                        $"{values[3]:F3}\t" +
                        $"{(values[4] * 1000):F3}\t" + // Inductance in mH
                        $"{values[5]:F3}\t" +
                        $"{values[6]:F3}\t" +
                        $"{values[7]:F3}\t");

                    result.AppendLine(s);
                }
                return result.ToString();
            }
        }
    }
}
