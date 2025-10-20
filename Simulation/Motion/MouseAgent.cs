using Microsoft.CodeAnalysis;
using Simulation.Render;
using SkiaSharp;

namespace Simulation.Motion;

public class MouseAgent 
{
	#region Properties
	//public static Dictionary<int, SKWorkspaceMapper> WorkspaceMappers = new Dictionary<int, SKWorkspaceMapper>();
	//public SKWorkspaceMapper WorkspaceMapper
	//{
	//	get
	//	{
	//		WorkspaceMappers.TryGetValue(Workspace.Id, out var result);
	//		return result;
	//	}
	//}

	public CoreRenderer Renderer { get; }
	public Runner Runner;
	public bool HasFocus { get; set; }
	public string Text = "";

	public DemoBase Demos { get; }
	public bool IsPaused { get; set; } = true;
	public bool IsDown { get; private set; }
	public bool IsDragging { get; private set; }

	public event EventHandler OnModeChange;
	//public event EventHandler OnDisplayModeChange;
	public event EventHandler OnSelectionChange;

	public bool DoSyncMatchingBasis { get; set; } = true;

	private ColorTheme _colorTheme = ColorTheme.Normal;
	public ColorTheme ColorTheme
	{
		get => _colorTheme;
		set
		{
			if (_colorTheme != value)
			{
				_colorTheme = value;
				Renderer.GeneratePens(_colorTheme);
			}
		}
	}

	private SKPoint _rawMousePoint;
	private float _minDragDistance = 4f;
	public float ScaleTickSize { get; set; } = 0.2f;
	public Keys CurrentKey { get; private set; }


	#endregion

	public MouseAgent(CoreRenderer renderer, DemoBase demos)
	{
		Renderer = renderer;
		Renderer.Agent = this;
		Runner = new Runner(this);
		//Stack.LastTime.SetWith(Runner.CurrentMS);

		Demos = demos;// new Demos(Brain, Renderer);

		ClearMouse();
	}

	public void Update(double currentTime, double deltaTime)
	{
        Renderer.CurrentDrawable?.Update((float)deltaTime);
	}

	#region Mouse
	public SKPoint DragPoint;
	public bool MouseDown(MouseArgs e)
	{
		if (IsPaused) { return false; }

		// Add to selection if ctrl down etc.
		_rawMousePoint = e.Location;
		var mousePoint = GetTransformedPoint(_rawMousePoint);
		IsDown = true;
		return true;
	}

	public bool MouseMove(MouseArgs e)
	{
		if (IsPaused) { return false; }

		var result = false;
		_rawMousePoint = e.Location;
		var mousePoint = GetTransformedPoint(_rawMousePoint);
		return true;
	}

	private bool _isDrawing = false;
	public bool MouseDrag(SKPoint mousePoint)
	{
		if (IsPaused) { return false; }
		return true;
	}

	public bool MouseUp(MouseArgs e)
	{
		if (IsPaused) { return false; }

		_rawMousePoint = e.Location;
		var mousePoint = GetTransformedPoint(_rawMousePoint);

		ClearMouse();
		return true;
	}
	public bool MouseDoubleClick(MouseArgs e)
	{
		if (IsPaused) { return false; }

		if (CurrentKey == Keys.Space && e.Button == MouseButtons.Left)
		{
			//Data.ResetZoom();
		}
		return true;
	}
	public bool MouseWheel(MouseArgs e)
	{
		if (IsPaused) { return false; }

		var scale = 1f + (Math.Sign(e.Delta) * ScaleTickSize);
		var rawMousePoint = e.Location;
		var mousePoint = GetTransformedPoint(_rawMousePoint);
		//Data.SetPanAndZoom(Data.Matrix, mousePoint, new SKPoint(0, 0), scale);
		return true;
	}
	public void ClearMouse()
	{
		IsDown = false;
		IsDragging = false;
		_isDrawing = false;
		DragPoint = SKPoint.Empty;
	}

	#endregion


	public bool IsControlDown() => (Keyboard.ModifierKeys & Keys.Control) == Keys.Control;

	#region Keyboard
	private bool _isControlDown;
	private bool _isShiftDown;
	private bool _isAltDown;
	private KeyArgs _lastKeyUp;
	public bool KeyDown(KeyArgs e)
	{
		if (CurrentKey == Keys.Escape)
		{
			IsPaused = !IsPaused;
		}
		if (IsPaused) { return false; }


		if (e.KeyCode != Keys.Control && e.KeyCode != Keys.Shift && e.KeyCode != Keys.Alt)
		{
			CurrentKey = e.KeyCode;
		}
		_isControlDown = e.Control;
		_isShiftDown = e.Shift;
		_isAltDown = e.Alt;

		return true;
	}
	public bool KeyUp(KeyArgs e)
	{
		bool result = true;
		_lastKeyUp = null;
		CurrentKey = Keys.None;
		_isControlDown = e.Control;
		_isShiftDown = e.Shift;
		return result;
	}
	#endregion

	#region Render
	public void Draw()
	{
	}
	public SKPoint GetTransformedPoint(SKPoint point) => point; // will be matrix etc
	#endregion

	public void ClearAll()
	{
		ClearMouse();
		Runner.Clear();
	}
}
