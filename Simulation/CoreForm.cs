using Forces.Primitives.Electricity.Simulation.Circuits;
using Simulation.PondSim;
using Simulation.Render;
using Simulation.Motion;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Sys = System.Windows.Forms;
using SymNum.Mind;
using Simulation.PondSim.Arena;

namespace Simulation;

public partial class CoreForm : Form
{
	private readonly DemoBase _demos;
	private readonly CoreRenderer _renderer;
	private readonly SKControl _control;
	private readonly MouseAgent _mouseAgent;
	private Runner _runner;
	private Workspace _workspace;
	public CoreForm()
	{
		InitializeComponent();
		DoubleBuffered = true;
		KeyPreview = true;

		_renderer = CoreRenderer.Instance;

		_control = new SKControl();
		_control.Width = Width;
		_control.Height = Height;
		_control.Dock = DockStyle.Fill;
		_renderer.Width = Width;
		_renderer.Height = Height;
		corePanel.Controls.Add(_control);

		_control.PaintSurface += DrawOnPaintSurface;
		_control.MouseDown += OnMouseDown;
		_control.MouseMove += OnMouseMove;
		_control.MouseUp += OnMouseUp;
		_control.MouseDoubleClick += OnMouseDoubleClick;
		_control.MouseWheel += OnMouseWheel;
		_control.PreviewKeyDown += OnPreviewKeyDown;
		corePanel.Resize += OnResize;
		KeyDown += OnKeyDown;
		KeyUp += OnKeyUp;

		//_demos = new Demos(_workspace.Brain);
		_demos = new PondArena(_control);
		_mouseAgent = new MouseAgent(_renderer, _demos);
		_runner = _mouseAgent.Runner;
		_runner.OnContextStringChanged += _runner_OnContextStringChanged;
		_ = Execute(null, 50);

        NextPanel();
    }

	private void OnResize(object? sender, EventArgs e)
	{
		_renderer.Width = corePanel.Width;
		_renderer.Height = corePanel.Height;
	}

	private void _runner_OnContextStringChanged(object? sender, EventArgs e)
	{
		//lbText.Text = _runner.EquationText;
	}

	private void DrawOnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
	{
		_renderer.DrawOnCanvas(e.Surface.Canvas);
	}
	public async Task Execute(Action action, int timeoutInMilliseconds)
	{
		await Task.Delay(timeoutInMilliseconds);
		ReloadTest();
		NeedsUpdate();
	}
	public void PreviousTest()
	{
		_runner.HasUpdated = false;
		_demos.PreviousTest(_mouseAgent);
		_runner.HasUpdated = true;
	}
	public void ReloadTest()
	{
		_runner.HasUpdated = false;
		_demos.Reload(_mouseAgent);
		_runner.HasUpdated = true;
	}
	public void NextTest()
	{
		_runner.HasUpdated = false;
		_demos.NextTest(_mouseAgent);
		_runner.HasUpdated = true;
	}


	private void OnMouseDown(object? sender, MouseEventArgs e) { if (_mouseAgent.MouseDown(e.ToMouseArgs())) { NeedsUpdate(); } }
	private void OnMouseMove(object? sender, MouseEventArgs e) { if (_mouseAgent.MouseMove(e.ToMouseArgs())) { NeedsUpdate(); } }
	private void OnMouseUp(object? sender, MouseEventArgs e) { if (_mouseAgent.MouseUp(e.ToMouseArgs())) { NeedsUpdate(); } }
	private void OnMouseDoubleClick(object? sender, MouseEventArgs e) { if (_mouseAgent.MouseDoubleClick(e.ToMouseArgs())) { NeedsUpdate(); } }
	private void OnMouseWheel(object? sender, MouseEventArgs e) { if (_mouseAgent.MouseWheel(e.ToMouseArgs())) { NeedsUpdate(); } }


	private void OnPreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
	{
		if (e.KeyData == Sys.Keys.Right || e.KeyData == Sys.Keys.Left || e.KeyData == Sys.Keys.Down || e.KeyData == Sys.Keys.Up) // todo: can't figure out why arrow keys aren't passed, passing manually for now but may lead to double invoke.
		{
			var ea = new KeyEventArgs(e.KeyData);
			if (_mouseAgent.KeyDown(ea.ToKeyArgs())) { NeedsUpdate(); }
		}
	}
	private void OnKeyDown(object? sender, KeyEventArgs e)
	{
		if (_mouseAgent.KeyDown(e.ToKeyArgs())) { NeedsUpdate(); }
		e.SuppressKeyPress = true; // ### Don't do this if eventually using menus etc. This supresses the alt'n sound the system gives thinking it can't find a menu item.
	}
	private void OnKeyUp(object? sender, KeyEventArgs e) { if (_mouseAgent == null || _mouseAgent.KeyUp(e.ToKeyArgs())) { NeedsUpdate(); } }

	private void NeedsUpdate()
	{
		_control.Invalidate();
		_runner.NeedsUpdate();
	}





	private void btRunVIRm_Click(object sender, EventArgs e)
	{
		var circuit = new VIR_Man_Circuit();
		circuit.Run();
		tbOutput.Text = circuit.ResultText;
	}
	private void btRunRCLm_Click(object sender, EventArgs e)
	{
		var circuit = new RCL_Man_Circuit();
		circuit.Run();
		tbOutput.Text = circuit.ResultText;
	}
	private void btRunRCL_Click(object sender, EventArgs e)
	{
		var circuit = new RCL_AC_Circuit();
		circuit.Run();
		tbOutput.Text = circuit.ResultText;
	}
	private void btRunTransient_Click(object sender, EventArgs e)
	{
		var circuit = new RCL_TRAN_Circuit();
		circuit.Run();
		tbOutput.Text = circuit.ResultText;
	}

	private enum SimulationState
	{
		Electric,
		Arena,
	}
	private SimulationState _state = SimulationState.Electric;
	private void NextPanel(object? sender = null, EventArgs? e = null)
	{
		if (_state == SimulationState.Electric)
		{
			_state = SimulationState.Arena;
			tbOutput.Visible = false;
			btRunVIR.Visible = false;
			btRCL_AC.Visible = false;
			btRclMan.Visible = false;
			btRCL_Tran.Visible = false;

			corePanel.Visible = true;
		}
		else
		{
			_state = SimulationState.Electric;
			tbOutput.Visible = true;
			btRunVIR.Visible = true;
			btRCL_AC.Visible = true;
			btRclMan.Visible = true;
			btRCL_Tran.Visible = true;

			corePanel.Visible = false;
		}
	}

}
