
using System.Numerics;
using System.Text;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Documentation;
using SpiceSharp.Simulations;
using SpiceSharp.ParameterSets;
using SpiceSharp.Entities;
using SpiceSharp.Components.SamplerBehaviors;
using System.Collections.Generic;
using SpiceSharp.Attributes;

namespace Forces.Primitives.Electricity.Simulation;

public class SpiceCircuit
{
    private CircuitSample _circuit;
    StringBuilder _log = new StringBuilder();
	public SpiceCircuit()
	{
    }
    public void RunCircuitVIR()
    {
        var circ = new CircuitVIR();
        _circuit = circ;
        _log.Clear();
        _log.AppendLine(_circuit.GetResultHeader());
        for (int i = 0; i < 3; i++)
        {
            var results = circ.RunDC(0.0, 5.0, 1.0);
            Trace(results);
            circ.Resistance0 += 1000.0; // increase R1 by 1000 Ohm each step
            circ.Resistance1 -= 100.0;
        }
    }
    public void RunCircuitRCL()
    {
        var circ = new CircuitRCL();
        _circuit = circ;
        _log.Clear();
        _log.AppendLine(_circuit.GetResultHeader());
        var results = circ.RunAC(1000.0, 3000.0, 100.0);
        Trace(results);
        _log.AppendLine("");
    }
    public string GetOutput() => _log.ToString();
    private void Trace(List<double[]> values)
    {
        foreach (var r in values)
        {
            _log.AppendLine(_circuit.GetResultText(r));
        }
        _log.AppendLine("");
    }

}





interface CircuitSample
{
    Circuit GetCircuit();
    List<double[]> RunDC(double startV, double endV, double step, string currentProbe, string voltageProbe);

    public string GetResultHeader();
    public string GetResultText(double[] values);
}
class CircuitVIR : CircuitSample
{
    private VoltageSource _voltageSource;
    private Resistor _r0;
    private Resistor _r1;
    public CircuitVIR()
    {
        var r0 = 1.0e3;
        var r1 = 2.0e3;
        _voltageSource = new VoltageSource("V1", "in", "0", 0.0);
        _r0 = new Resistor("R1", "in", "out", r0);
        _r1 = new Resistor("R2", "out", "0", r1);
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
    public Circuit GetCircuit()
    {
        var result = new Circuit(
            _voltageSource,
            _r0,
            _r1
            );
        return result;
    }
    public List<double[]> RunDC(double startV, double endV, double step, string currentProbe = "V1", string voltageProbe = "out")
    {
        var result = new List<double[]>();
        var dc = new DC("dc", "V1", startV, endV, step);

        foreach (int exportType in dc.Run(GetCircuit()))
        {
            double current = -dc.GetCurrent(currentProbe);
            double voltage = dc.GetVoltage(voltageProbe);
            double resistance = voltage / current;
            result.Add([voltage, current, resistance]);
        }
        return result;
    }
    public string GetResultHeader() => "voltage\tcurrent\tresistance";
    public string GetResultText(double[] values)
    {
        return new string(
            values[0].ToString("0.000") + "   \t" +    // voltage V
            (values[1] * 1000).ToString("0.000") + "   \t" +  // current mA
            values[2].ToString("0.000")                    // resistance ohms
            );
    }

}

class CircuitRCL : CircuitSample
{
    private VoltageSource _voltageSource;
    private Resistor _resistor;
    private Inductor _inductor;
    private Capacitor _capacitor;

    public CircuitRCL()
    {
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

    public Circuit GetCircuit()
    {
        _voltageSource = new VoltageSource("V1", "in", "0", 0);
        _voltageSource.SetParameter("acmag", 12.0);// new Sine(0, 12, 1000));
        _resistor = new Resistor("R1", "in", "n1", 100); 
        _inductor = new Inductor("L1", "n1", "n2", 0.1); 
        _capacitor = new Capacitor("C1", "n2", "0", 1.0e-6);    // 1 µF
      
        return new Circuit(
            _voltageSource,
            _resistor,
            _inductor,
            _capacitor
        );
    }
    public List<double[]> RunDC(double startV, double endV, double step, string voltageProbe = "out", string currentProbe = "V1")
    {
        throw new NotImplementedException();
    }
    
    public List<double[]> RunAC(double startFreq, double endFreq, double step, string voltageProbe = "n1", string currentProbe = "V1")
    {
        var ac = new AC("ac", new LinearSweep(startFreq, endFreq, step)); // new DecadeSweep(1e-2, 1.0e3, 5));// 
        var result = new List<double[]>();
        //var voltageComplex = new ComplexVoltageExport(ac, voltageProbe);
        //var currentComplex = new ComplexCurrentExport(ac, currentProbe);

        var voltageComplex = new ComplexPropertyExport(ac, "R1", "v");
        var currentComplex = new ComplexPropertyExport(ac, "R1", "i");
        foreach (int exportType in ac.Run(GetCircuit()))
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

            result.Add([ac.Frequency, voltageRms, currentRms, resistance, inductance * 1000, xL, xC, phase]);
        }
        return result;
    }
    public string GetResultHeader() => "ACFreq    voltageRms currentRms resistance  inductance  xL       xC       phase";
    public string GetResultText(double[] values)
    {
        return new string(
            $"{values[0]:F1}\t" +
            $"{values[1]:F3}\t    " +
            $"{(values[2] * 1000):F3}\t        " + // Current in mA
            $"{values[3]:F3}\t      " +
            $"{(values[4] * 1000):F3}\t        " + // Inductance in mH
            $"{values[5]:F3}    " +
            $"{values[6]:F3}    " +
            $"{values[7]:F3}"
        );
    }
}