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
    public float LightSensorValue { get; private set; } // Output: 0 (dark) to 1 (bright)

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
            linearDamping = 0.5f,
            angularDamping = 0.2f,
            bullet = false
        };
        var body = arena.PhysicsWorld.CreateBody(bodyDef);
        Body = body;
        var circleShape = new CircleShape { Radius = _creatureRadius };
        Body.CreateFixture(new FixtureDef { shape = circleShape, density = 1f, friction = 0.3f, restitution = 0.1f });
    }

	private float _thrustForce = 6000f; // Tune for sim scale
	private float _turnTorque = 1000f;
	public override void Update(float deltaTime)
	{
		UpdateLightSensor();
        _creature.OnTick();
        if(Body != null)
        {
            var turn = _creature.TurnRatio.Calculate().GetRational().FloatValue/100f + 0.01f;
            //var turn = (float)_creature.SightRatio.Value - 1.1f;
            var thrust = 4f;// (float)_creature.Thrust.Value;
            var facing = new Vector2((float)Math.Cos(Body.GetAngle()), (float)Math.Sin(Body.GetAngle()));
            turn = (float)Math.Clamp(turn, -3f, 3f);
            Body.ApplyForceToCenter(facing * thrust * _thrustForce, true);

            Body.ApplyTorque(turn * _turnTorque, true);
            //Trace.WriteLine(thrust + " : " + turn + " : " + turn*_turnTorque);
        }
    }

    public void UpdateLightSensor()
	{
		var cLeft = _sim.GetLightAt(LeftEye + Position.SKPoint());
		var cRight = _sim.GetLightAt(RightEye + Position.SKPoint());
		_creature.SetSight(cLeft, cRight);
		//Trace.WriteLine($"({cLeft}, {cRight})");

		//float totalIntensity = 0f;
		//foreach (var light in lights)
		//{
		//    Vector2 toLight = new Vector2(light.Position.X - Position.X, light.Position.Y - Position.Y);
		//    float distance = toLight.Length();
		//    //toLight.Normalize();

		//    // Raycast callback collector
		//    //var rayCastCallback = new RayCastCallback(distance);
		//    //arena.PhysicsWorld.RayCast(rayCastCallback,
		//    //    new Vector2(Position.X, Position.Y),
		//    //    new Vector2(Position.X, Position.Y) + toLight * distance);

		//    //bool visible = !rayCastCallback.ClosestHitDist.HasValue || rayCastCallback.ClosestHitDist >= distance;
		//    //if (visible)
		//    //{
		//    //    float angleDiff = MathF.Acos(Vector2.Dot(new Vector2(toLight.X, toLight.Y),
		//    //        new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation))));
		//    //    if (angleDiff < MathF.PI / 2)
		//    //    {
		//    //        float intensity = light.Brightness / (distance * distance + 1f);
		//    //        totalIntensity += (float)Math.Clamp(intensity, 0f, 1f);
		//    //    }
		//    //}
		//}
		//LightSensorValue = (float)Math.Clamp(totalIntensity / lights.Count, 0f, 1f);
	}

	//private class RayCastCallback : IRayCastCallback
	//{
	//    public float? ClosestHitDist { get; private set; }
	//    private float _distance;

	//    public RayCastCallback(float distance)
	//    {
	//        _distance = distance;
	//        ClosestHitDist = null;
	//    }

	//    public float ReportFixture(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
	//    {
	//        ClosestHitDist ??= fraction * _distance;
	//        return -1f; // Continue to find closest
	//    }

	//    float IRayCastCallback.RayCastCallback(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
	//    {
	//        return 0; //throw new NotImplementedException();
	//    }
	//}
	private static float _creatureRadius = 8f;
	private SKPaint Fill = CorePens.GetBrush(SKColor.Parse("#F0F000"));
	private SKPaint White = CorePens.GetBrush(SKColor.Parse("#FFFFFF"));
	private SKPaint Outline = CorePens.GetPen(SKColor.Parse("#F01000"), 2);
	public override void Draw()
    {
        var offset = _creatureRadius * -0.7f;

		if (Body != null)
		{
			Canvas?.DrawCircle(SKPoint.Empty, _creatureRadius, Fill);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, White);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, White);
			Canvas?.DrawCircle(LeftEye, _creatureRadius / 2.5f, Outline);
			Canvas?.DrawCircle(RightEye, _creatureRadius / 2.5f, Outline);
		}
    }
}
