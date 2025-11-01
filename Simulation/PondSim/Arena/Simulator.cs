
namespace Simulation.PondSim.Arena;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Forces.Primitives.Steering.Simulation;
using Simulation.Render;
using NumbersSkia.Mappers;

public class Simulator : DrawableBase
{
    private PondArena _arena;
    private List<CreatureBody> _creatures = new();
    private List<LightSource> _lights = new();
    private float _fixedDt = 1f / 60f;
    private float _accumulator = 0f;

    private SKImageMapper? _image;
    public Simulator(PondArena arena)
    {
        _arena = arena;
        //var min = MathF.Min(_arena.Control.Width, _arena.Control.Height); //436;// 
        _image = new SKImageMapper("map1.png", new SKSegment(new(0,0), new(_arena.Control.Width, 0)));

        var light = new LightSource { Position = new Vector2(100, 100), Brightness = 5f };
        _lights.Add(light);

        var creature1 = new Creature();

        var creature = new CreatureBody(creature1);
        creature.InitPhysics(_arena, new Vector2(500, 500), 10f);
        _creatures.Add(creature);

        Renderer.CurrentDrawable = this;
    }
    public Simulator(PondArena arena, IEnumerable<CreatureBody> creatures, IEnumerable<LightSource> lights)
    {
        _arena = arena;
        _lights.AddRange(lights);
        _creatures.AddRange(creatures);

        foreach (var creature in _creatures)
        {
            creature.InitPhysics(_arena, creature.Position, creature.Rotation);
        }
    }
    public override void Update(float deltaTime)
    {
        //_accumulator += deltaTime;
        //while (_accumulator >= _fixedDt)
        //{
            foreach (var creature in _creatures)
            {
                creature.UpdateLightSensor(_arena, _lights);
                creature.Update(deltaTime);
            }
            _arena.PhysicsWorld.Step(_fixedDt, 8, 3); // Vel/pos iterations

        //    _accumulator -= _fixedDt;
        //}
    }

    public override void Draw()
    {
        if (Canvas != null)
        {
            Canvas.Clear(SKColors.DarkSlateBlue);
            _image?.Draw();
            //todo: draw obstacles/lights
            foreach (var creature in _creatures)
            {
                Canvas.Save();
                Canvas.Translate(creature.Position.X, creature.Position.Y);
                Canvas.RotateDegrees(creature.Rotation * 180f / MathF.PI);
                creature.Draw();
                Canvas.Restore();
            }
        }
    }
}
