using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace something;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private KeyboardState _prevKeyboardState;
    private Effect _shader;
    private EffectParameter _positionsParam;
    private EffectParameter _colorsParam;
    private EffectParameter _deviationParam; 
    private EffectParameter _deviationMultParam;
    private const int NUM_NODES = 30;
    private Vector2[] _positions;
    private Vector2[] _vels;
    private float[] _deviationMult;
    private float _speed = 1f;
    private float _t = 0;
    private Vector4[] _colors;
    private readonly VertexPositionTexture[] _verts = [
        new(new(-1, 1, 0), Vector2.Zero),
        new(new(1, 1, 0), Vector2.UnitX),
        new(new(-1, -1, 0), Vector2.UnitY),
        new(new(1, -1, 0), Vector2.One)
    ];
    private readonly short[] _indices = [0, 1, 2, 1, 3, 2]; 

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = false;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        _positions = new Vector2[NUM_NODES];
        _colors = new Vector4[NUM_NODES];
        _vels = new Vector2[NUM_NODES];
        _deviationMult= new float[NUM_NODES];
        for (int i = 0; i < NUM_NODES; i++)
        {
            _colors[i] = new Vector4(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1);
            _positions[i] = new((Random.Shared.NextSingle()-.5f) * 2, (Random.Shared.NextSingle()-.5f) * 2);
            _vels[i] = new((Random.Shared.NextSingle() - .5f), (Random.Shared.NextSingle() - .5f));
            _deviationMult[i] = Random.Shared.NextSingle();
        }
        _graphics.HardwareModeSwitch = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _shader = Content.Load<Effect>("shader");
        _positionsParam = _shader.Parameters["Positions"];
        _colorsParam = _shader.Parameters["Colors"];
        _colorsParam.SetValue(_colors);
        _deviationParam = _shader.Parameters["Deviation"];
        _deviationMultParam = _shader.Parameters["DeviationMult"];
        _deviationMultParam.SetValue(_deviationMult);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        float dt = gameTime.ElapsedGameTime.Ticks / 10000000f;
        _t += dt;
        Console.WriteLine("FPS: " + 1/dt);
        if (_prevKeyboardState.IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyUp(Keys.R))
        {
            for (int i = 0; i < NUM_NODES; i++)
            {
                _colors[i] = new Vector4(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1);
                _deviationMult[i] = Random.Shared.NextSingle() + .5f;
            }
            _colorsParam.SetValue(_colors);
            _deviationMultParam.SetValue(_deviationMult);
        }
        for (int i = 0; i < NUM_NODES; i++)
        {
            _positions[i] += Vector2.Rotate(_vels[i], MathF.Sin(_t) * MathHelper.PiOver4) * dt * _speed;
            if ((_positions[i].X > 1 && _vels[i].X > 0) || (_positions[i].X < -1 && _vels[i].X < 0))
            {
                _vels[i].X *= -1;
            }
            if ((_positions[i].Y > 1 && _vels[i].Y > 0) || (_positions[i].Y < -1 && _vels[i].Y < 0))
            {
                _vels[i].Y *= -1;
            }
            
        }


        _positionsParam.SetValue(_positions);
        _deviationParam.SetValue((float) Mouse.GetState().X / GraphicsDevice.Viewport.Width);
        _prevKeyboardState = Keyboard.GetState();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        foreach (var pass in _shader.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _verts, 0, 4, _indices, 0, 2);
        }


        base.Draw(gameTime);
    }
}
