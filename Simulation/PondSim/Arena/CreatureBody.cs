namespace Simulation.PondSim.Arena;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Forces.Primitives.Steering.Simulation;
using SkiaSharp;
using Simulation.Render;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Collision.Shapes;
using System.Diagnostics;

public class CreatureBody : DrawableBase
{
    public Vector2 Position => Body?.GetPosition() ?? Vector2.Zero;
    public float Rotation => Body?.GetAngle() ?? 0;

    public Body? Body { 
        get;
        private set; }
    public Creature _creature;

    public SKPoint LeftEye { get; }
    public SKPoint RightEye { get; }
	public Simulator _sim { get; }

    public CreatureBody(Simulator sim, Creature creature)
    {
		_sim = sim;
        _creature = creature;

		var offset = _creatureRadius * -0.7f;
		LeftEye = new(-offset, offset);
		RightEye = new(-offset, -offset);
	}

    public void InitPhysics(PondArena arena, Vector2 startPos, float startRot)
    {
        var bodyDef = new BodyDef
        {
            type = BodyType.Dynamic,
            position = new Vector2(startPos.X, startPos.Y),
            angle = startRot,
            linearDamping = 1f,
            angularDamping = 1f,
            bullet = false
        };
        var body = arena.PhysicsWorld.CreateBody(bodyDef);
        Body = body;
		Body.SetUserData(this);

		var circleShape = new CircleShape { Radius = _creatureRadius };
        Body.CreateFixture(new FixtureDef { 
			shape = circleShape, 
			density = 1f, 
			friction = 0.01f,
			restitution = 0.8f,
			filter = new Filter
			{
				categoryBits = 0x0002, // Creature category
				maskBits = 0x0003 // collide with wall and other creatures   
				//maskBits = 0x0001 // Only collide with wall category
			}
		});
    }

	//private float _thrustForce = 1f; // Tune for sim scale
	//private float _turnTorque = 3000f;
	private Random _rnd = new Random();
	private float _turnTotal = 0f;
	private float _energyUse = 0f;
	//private float _lastForce = 0f;
	public override void Update(float deltaTime)
	{
		ExternalInput();
        _creature.OnTick();

        if(Body != null)
		{
			var c = _creature;
			var s = c.Sight;

			var ptL = Body.GetWorldPoint(new(0, -1f));
			var ptR = Body.GetWorldPoint(new(0, 1f));
			var speed = Body.GetLinearVelocity().Length();
			var angle = Body.GetAngle();
            var angleVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

			// todo: A complex number that shows how much you are turning (- or +) and how much you are keeping straight.
			// A complex number that shows that shows how much thrust, and how much dampening
			// A complex number that shows how big a light difference each eye has, and how similar they are.
			var force = angleVec * c.EnergyRatio.Value;
			_turnTotal = c.BrighterDirection.Value * 50f;
			var lTurn = 2000f + (_turnTotal < 0 ? MathF.Abs(_turnTotal) : 0);
			var rTurn = 2000f + (_turnTotal > 0 ? MathF.Abs(_turnTotal) : 0);

			Body.ApplyForce(force * lTurn, ptL, true);
			Body.ApplyForce(force * rTurn, ptR, true);
			_energyUse = (lTurn + rTurn) * c.EnergyRatio.Value;

			Trace.WriteLine("ts: " + _turnTotal);
			//Trace.WriteLine("eyes: " + _creature.Brightness);
		}
	}
	public void OnWallHit()
	{
		if (Body != null)
		{
			var angle = Body.GetAngle();
			var angleVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
			var vec = new Vector2(angleVec.X + _rnd.NextSingle(), angleVec.Y - _rnd.NextSingle());
			Body.ApplyForce(vec * -500000, Body.GetWorldPoint(new(4f, 0)), true);
		}
	}
	private void ExternalInput()
	{
		if(_leftEyePos != null && _rightEyePos != null)
		{
			var cLeft = _sim.GetLightAt((SKPoint)_leftEyePos);
			var cRight = _sim.GetLightAt((SKPoint)_rightEyePos!);
			var energyInput = (cLeft + cRight) * 6000;
			_creature.SetInputs(cLeft, cRight, energyInput, (long)_energyUse);
		}
	}

	private SKPoint? _leftEyePos;
	private SKPoint? _rightEyePos;
	private static float _creatureRadius = 8f;
	private SKPaint FillL = CorePens.GetBrush(SKColor.Parse("#00C080"));
	private SKPaint FillR = CorePens.GetBrush(SKColor.Parse("#00A0F0"));
	private SKPaint Yellow = CorePens.GetBrush(SKColor.Parse("#FFFF00"));
	private SKPaint LightGray = CorePens.GetBrush(SKColor.Parse("#AAAAAA"));
	private SKPaint Gray = CorePens.GetBrush(SKColor.Parse("#777777"));
	private SKPaint Outline = CorePens.GetPen(SKColor.Parse("#C01000"), 2);
	public override void Draw()
    {
		if (Body != null)
		{
			var e = _creature.EnergyStore;
			byte el = 255;// (byte)((e.Position / (float)e.Maximum) * 255);
			var turningRight = _turnTotal > 0;
			var f = turningRight ? FillR : FillL;
			var fillAlpha = CorePens.GetBrush(new SKColor(f.Color.Red, f.Color.Green, f.Color.Blue, el));
			Canvas?.DrawCircle(SKPoint.Empty, _creatureRadius, fillAlpha);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, turningRight ? LightGray : Gray);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, turningRight ? Gray : LightGray);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, Outline);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, Outline);

			_leftEyePos = Canvas?.TotalMatrix.MapPoint(LeftEye);
			_rightEyePos = Canvas?.TotalMatrix.MapPoint(RightEye);
		}
    }
}
