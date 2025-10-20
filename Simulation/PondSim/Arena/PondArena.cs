namespace Simulation.PondSim.Arena;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Common;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using SkiaSharp.Views.Desktop;

public class PondArena : DemoBase
{
    public Simulator Sim { get; }
    public World PhysicsWorld { get; } = new World(new Vector2(0f, 0f)); // No gravity
    private List<Body> _obstacleBodies = new();
    public PondArena(SKControl control) : base(control)
    {
        Sim = new Simulator(this);
       // AddObstacle(new Vector2(10, 10), new Vector2(30, 15));
    }

    public void AddObstacle(Vector2 start, Vector2 end)
    {
        var bodyDef = new BodyDef { type = BodyType.Static, position = new Vector2(0f, 0f) };
        var body = PhysicsWorld.CreateBody(bodyDef);

        var edgeShape = new EdgeShape();
        edgeShape.SetTwoSided(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y));
        body.CreateFixture(new FixtureDef { shape = edgeShape, density = 0f, friction = 0.3f });
        _obstacleBodies.Add(body);
    }
}
