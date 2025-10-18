namespace Simulation.PondSim.Arena;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;

public class PondArena
{
    public World PhysicsWorld { get; } = new World(new Vector2(0f, 0f)); // No gravity
    private List<Body> _obstacleBodies = new();

    public void AddObstacle(Vector2 start, Vector2 end)
    {
        var bodyDef = new BodyDef { BodyType = BodyType.StaticBody, Position = new Vector2(0f, 0f) };
        var body = PhysicsWorld.CreateBody(bodyDef);

        var edgeShape = new EdgeShape();
        edgeShape.SetTwoSided(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y));
        body.CreateFixture(new FixtureDef { Shape = edgeShape, Density = 0f, Friction = 0.3f });
        _obstacleBodies.Add(body);
    }
}
