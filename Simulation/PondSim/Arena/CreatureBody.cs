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
    private Creature _creature;

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
            linearDamping = 3f,
            angularDamping = 1.3f,
            bullet = false
        };
        var body = arena.PhysicsWorld.CreateBody(bodyDef);
        Body = body;
        var circleShape = new CircleShape { Radius = _creatureRadius };
        Body.CreateFixture(new FixtureDef { 
			shape = circleShape, 
			density = 1f, 
			friction = 0.01f,
			restitution = 0.8f,
			//filter = new Filter
			//{
			//	categoryBits = 0x0002,  // Creature category
			//	maskBits = 0x0001       // Only collide with wall category
			//}
		});
    }

	private float _thrustForce = 6000f; // Tune for sim scale
	private float _turnTorque = 4000f;
	private Random _rnd = new Random();
	private float _turnTotal = 0f;
	public override void Update(float deltaTime)
	{
		SampleEyes();
        _creature.OnTick();

        if(Body != null)
		{
			var speed = Body.GetLinearVelocity().Length();
			var angle = Body.GetAngle();

			var turnOffset = (float)_creature.BrighterDirection;
			var thrustSclalar = 1.2f;
			if(_creature.Brightness.SumValue < 4) // dark
			{
				turnOffset = 0;
				thrustSclalar = 4f;
			}
			_turnTotal = float.Clamp(turnOffset * _turnTorque, -15000f, 15000f);

            var turnVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
			if (speed < 1)
			{
				thrustSclalar = 1000;
				_turnTotal = (_rnd.NextSingle() - 0.5f) * 4000000f;
			}

			Body.ApplyTorque(_turnTotal, true);
			Body.ApplyForceToCenter(turnVec * thrustSclalar * _thrustForce, true);
			//Trace.WriteLine("tt: "+ _turnTotal);
			 
		}
	}
	private void SampleEyes()
	{
		//var xform = Body.GetTransform();
		//var angle = Body.GetAngle();
		//var cosA = (float)Math.Cos(angle);
		//var sinA = (float)Math.Sin(angle);
		//var leftWorldOffset = new Vector2(
		//	LeftEye.X * cosA - LeftEye.Y * sinA,
		//	LeftEye.X * sinA + LeftEye.Y * cosA);
		//var rightWorldOffset = new Vector2(
		//	RightEye.X * cosA - RightEye.Y * sinA,
		//	RightEye.X * sinA + RightEye.Y * cosA);
		//var leftWorldPos = Position + leftWorldOffset;
		//var rightWorldPos = Position + rightWorldOffset;

		//Trace.WriteLine(_leftEyePos?.X + "," + _leftEyePos?.Y + " - " + leftWorldPos.X + "," + leftWorldPos.Y);

		//var cLeft = _sim.GetLightAt(leftWorldPos.SKPoint());
		//var cRight = _sim.GetLightAt(rightWorldPos.SKPoint());
		if(_leftEyePos != null && _rightEyePos != null)
		{
			var cLeft = _sim.GetLightAt((SKPoint)_leftEyePos);
			var cRight = _sim.GetLightAt((SKPoint)_rightEyePos!);
			_creature.SetSight(cLeft, cRight);

		}
	}

	private SKPoint? _leftEyePos;
	private SKPoint? _rightEyePos;
	private static float _creatureRadius = 8f;
	private SKPaint FillL = CorePens.GetBrush(SKColor.Parse("#00F0F0"));
	private SKPaint FillR = CorePens.GetBrush(SKColor.Parse("#F0F000"));
	private SKPaint White = CorePens.GetBrush(SKColor.Parse("#FFFFFF"));
	private SKPaint Outline = CorePens.GetPen(SKColor.Parse("#F01000"), 2);
	public override void Draw()
    {
        var offset = _creatureRadius * -0.7f;

		if (Body != null)
		{
			var f = _turnTotal > 0 ? FillR : FillL;
			Canvas?.DrawCircle(SKPoint.Empty, _creatureRadius, f);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, White);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, White);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, Outline);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, Outline);

			_leftEyePos = Canvas?.TotalMatrix.MapPoint(LeftEye);
			_rightEyePos = Canvas?.TotalMatrix.MapPoint(RightEye);
		}
    }
}
