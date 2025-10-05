
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using SpiceSharpParser;
using SpiceSharpParser.Common;
using SpiceSharpParser.ModelReaders.Netlist.Spice;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Exporters;
using static System.Net.Mime.MediaTypeNames;

namespace Forces.Primitives.Electricity.Simulation;

public abstract class CircuitSample
{
    public SpiceSharpModel? Model { get; protected set; }
    public Circuit? Circuit => Model?.Circuit;
    public Export[]? Exports { get; protected set; }
    public List<double[]> Results { get; protected set; } = new List<double[]>();

    protected StringBuilder _log = new StringBuilder();
    protected string? _netlist { get; }


    public CircuitSample()
    {
    }
    public CircuitSample(string netList)
    {
        _netlist = netList;
    }
    protected virtual void Create()
    {
        if (_netlist != null)
        {
            // Parsing part
            var parser = new SpiceNetlistParser();
            var parseResult = parser.ParseNetlist(_netlist);
            var netlist = parseResult.FinalModel;

            // Translating netlist model to SpiceSharp
            var reader = new SpiceSharpReader();
            Model = reader.Read(netlist);

            // Simulation using SpiceSharp
            var simulation = Model.Simulations.Single();
            var exportNames = GetExportNames(_netlist);
            var exportList = new List<Export>();
            foreach (var name in exportNames)
            {
                var export = Model.Exports.Find(e => e.Name == name);
                if (export != null)
                {
                    exportList.Add(export);
                }
            }
            Exports = exportList.ToArray();
        }
    }
    public virtual void Run()
    {
        if(Model == null)
        {
            Create();
        }
        Results = new List<double[]>();
        var sim = Model!.Simulations.Single();
        sim.EventExportData += (sender, args) =>
        {
            var vals = new double[Exports!.Length];
            for (int i = 0; i < Exports.Length; i++)
            {
                vals[i] = Exports[i].Extract();
            }
            Results.Add(vals);
        };
        var codes = sim.Run(Circuit, -1);
        codes = sim.InvokeEvents(codes);
        codes.ToArray(); //eval

    }
    protected string[] GetExportNames(string netlistText)
    {
        string[] result;
        // Regex pattern: Match .save line, capture op and params
        string pattern = @"^\.save\s+(\w+)\s+(.+)$";
        Match match = Regex.Match(netlistText, pattern, RegexOptions.Multiline);

        if (match.Success)
        {
            string op = match.Groups[1].Value;  // e.g., "ac"
            string paramsStr = match.Groups[2].Value;  // e.g., "v(in) v(out) i(V1)"

            // Split params by spaces, trim each
            result = paramsStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(p => p.Trim())
                                           .Where(p => !string.IsNullOrWhiteSpace(p))
                                           .ToArray();
        }
        else
        {
            Trace.WriteLine("No .save line found.");
            result = [];
        }
        return result;
    }

    public virtual string[]? Headers { get; }
    public virtual string ResultText => FormatToColumns();
    protected string FormatToColumns()
    {
        if (Results.Count == 0) return "";

        int cols = Results[0].Length;

        List<string[]> strData = Results.Select(row => row.Select(d => d.ToString()).ToArray()).ToList();

        if (Headers != null && Headers.Length == cols)
        {
            strData.Insert(0, Headers);
        }

        int[] widths = new int[cols];
        for (int i = 0; i < cols; i++)
        {
            widths[i] = strData.Max(row => row[i].Length);
        }

        StringBuilder sb = new StringBuilder();
        foreach (var row in strData)
        {
            for (int i = 0; i < cols; i++)
            {
                sb.Append(row[i].PadRight(widths[i] + 2)); // +2 for spacing between columns
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
public class RCL_AC_Circuit : CircuitSample
{
    public RCL_AC_Circuit() : base(SpiceNetlists.RCL_AC) { }
}
public class RCL_TRAN_Circuit : CircuitSample
{
    public RCL_TRAN_Circuit() : base(SpiceNetlists.RCL_TRAN) { }
}

public class CircuitVIR : CircuitSample
{
    private VoltageSource _voltageSource;
    private Resistor _r0;
    private Resistor _r1;
    private List<double[]> _results = new List<double[]>();
    public CircuitVIR()
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
        _results.Clear();
        for (int i = 0; i < 3; i++)
        {
            RunDC(0.0, 5.0, 1.0);
            Resistance0 += 1000.0; // increase R1 by 1000 Ohm each step
            Resistance1 -= 100.0;
        }
    }
    public void RunDC(double startV, double endV, double step, string currentProbe = "V1", string voltageProbe = "out")
    {
        var dc = new DC("dc", "V1", startV, endV, step);

        foreach (int exportType in dc.Run(Circuit))
        {
            double current = -dc.GetCurrent(currentProbe);
            double voltage = dc.GetVoltage(voltageProbe);
            double resistance = voltage / current;
            _results.Add([voltage, current, resistance]);
        }
    }
    public override string[]? Headers => ["voltage","current","resistance"];
    public override string ResultText
    {
        get
        {
            var result = new StringBuilder();
            result.AppendLine(string.Join("\t", Headers!));
            foreach (var values in _results)
            {
                result.AppendLine(new string(
                    values[0].ToString("0.000") + "   \t" +    // voltage V
                    (values[1] * 1000).ToString("0.000") + "   \t" +  // current mA
                    values[2].ToString("0.000")                    // resistance ohms
                )
                );
            }
            return result.ToString();
        }
    }
}

public class CircuitRCL : CircuitSample
{
    private VoltageSource _voltageSource;
    private Resistor _resistor;
    private Inductor _inductor;
    private Capacitor _capacitor;

    private List<double[]> _results = new List<double[]>();

    public CircuitRCL()
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

        var circ =  new Circuit(
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
    public override string[]? Headers => ["ACFreq","vRms", "iRms","res", "induct","xL","xC","phase"];
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