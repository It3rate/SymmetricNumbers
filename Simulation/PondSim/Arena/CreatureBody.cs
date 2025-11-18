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
            linearDamping = 4f,
            angularDamping = 3f,
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

	private float _thrustForce = 5000f; // Tune for sim scale
	//private float _turnTorque = 3000f;
	private Random _rnd = new Random();
	private float _turnTotal = 0f;
	private float _thrustScale = 0f;
	private float _lastForce = 0f;
	public override void Update(float deltaTime)
	{
		ExternalInput();
        _creature.OnTick();

        if(Body != null)
		{
			var speed = Body.GetLinearVelocity().Length();
			var angle = Body.GetAngle();
            var angleVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

			var darkness = _creature.Sight.MinValue * 6f;
			_thrustScale = (8.5f-Math.Clamp(darkness, .5f, 6f)) * (_creature.EnergyStore.Position/3000f);// (darkness == 0 ? 1f : 1f/darkness) + 2f;
			_lastForce = _thrustForce * _thrustScale;
			var force = angleVec * _lastForce;

			var dir = _creature.ThrustDirection;
			_turnTotal = Math.Abs(dir) > 0.6f ? Math.Clamp(dir * 0.38f, -1.2f, 1.2f) : 0;
			var point = Body.GetWorldPoint(new(0, _turnTotal));

			Body.ApplyForce(force, point, true);
			// remove energy based on how much thrust expended.
		}
	}
	public void OnWallHit()
	{
		if (Body != null)
		{
			var angle = Body.GetAngle();
			var angleVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
			var vec = new Vector2(angleVec.X + _rnd.NextSingle(), angleVec.Y - _rnd.NextSingle());
			Body.ApplyForce(vec * -5000000, Body.GetWorldPoint(new(4f, 0)), true);
		}
	}
	private void ExternalInput()
	{
		if(_leftEyePos != null && _rightEyePos != null)
		{
			var cLeft = _sim.GetLightAt((SKPoint)_leftEyePos);
			var cRight = _sim.GetLightAt((SKPoint)_rightEyePos!);
			var energyInput = (cLeft + cRight) / 50;
			var energyUse = (long)(_thrustScale);
			_creature.SetInputs(cLeft, cRight, energyInput, energyUse);
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
			var el = (byte)((e.Position / (float)e.Maximum) * 255);
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
