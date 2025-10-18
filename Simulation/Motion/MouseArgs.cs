using SkiaSharp;

namespace Simulation.Motion;

public class MouseArgs
{
	public MouseButtons Button { get; private set; }
	public int Clicks { get; private set; }
	public int X { get; private set; }
	public int Y { get; private set; }
	public int Delta { get; private set; }

	public MouseArgs(int x, int y, int delta, int clicks, MouseButtons button)
	{
		X = x;
		Y = y;
		Delta = delta;
		Clicks = clicks;
		Button = button;
	}

	public SKPoint Location => new SKPoint(X, Y);
}
public enum MouseButtons
{
	Left = 0x100000,
	None = 0,
	Right = 0x200000,
	Middle = 0x400000,
	XButton1 = 0x800000,
	XButton2 = 0x1000000
}

public static class MouseEventArgsExtension
{
	public static MouseArgs ToMouseArgs(this MouseEventArgs args)
	{
		var btn = (MouseButtons)args.Button;
		var result = new MouseArgs(args.X, args.Y, args.Delta, args.Clicks, btn);
		return result;
	}
}
public static class KeyEventArgsExtension
{
	public static KeyArgs ToKeyArgs(this KeyEventArgs args)
	{
		var keyData = (Keys)args.KeyData;
		var result = new KeyArgs(keyData);
		return result;
	}
}