namespace Forces.Primitives.Electricity.Simulation.Circuits
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using SpiceSharp;
    using SpiceSharpParser;
    using SpiceSharpParser.ModelReaders.Netlist.Spice;
    using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Exporters;

    public abstract class CircuitBase
    {
        public SpiceSharpModel? Model { get; protected set; }
        public Circuit? Circuit => Model?.Circuit;
        public Export[]? Exports { get; protected set; }
        public List<double[]> Results { get; protected set; } = new List<double[]>();

        protected StringBuilder _log = new StringBuilder();
        protected string? _netlist { get; }


        public CircuitBase()
        {
        }
        public CircuitBase(string netList)
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
            if (Model == null)
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
}
