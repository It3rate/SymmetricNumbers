using Forces.Primitives.Electricity.Simulation;

namespace Simulation
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void btRun_Click(object sender, EventArgs e)
		{
			var circuit = new SpiceCircuit();
			tbOutput.Text = circuit.GetOutput();
		}
	}
}
