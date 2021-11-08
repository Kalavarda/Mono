using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;

namespace TowerDefense
{
    public class GameHost: WpfGame
    {
        private const float CameraSpeed = 0.1f;
        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private Model _model;
        private readonly Vector3 _ambientLightColor = new Vector3(1f, 0, 0);

        //Camera
        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        protected override void Initialize()
        {
            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();


            //Setup Camera
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(0f, 0f, -5);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                _graphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
                1f,
                1000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget,
                new Vector3(0f, 1f, 0f));// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.
                Forward, Vector3.Up);

            //_model = Content.Load<Model>("cottage");
            _model = Content.Load<Model>("Sword");
        }

        protected override void Update(GameTime time)
        {
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                camPosition.X -= CameraSpeed;
                camTarget.X -= CameraSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                camPosition.X += CameraSpeed;
                camTarget.X += CameraSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                camPosition.Y -= CameraSpeed;
                camTarget.Y -= CameraSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                camPosition.Y += CameraSpeed;
                camTarget.Y += CameraSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                camPosition.Z += CameraSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                camPosition.Z -= CameraSpeed;
            }


            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            foreach (var mesh in _model.Meshes)
            {
                foreach (var effect in mesh.Effects.OfType<BasicEffect>())
                {
                    effect.AmbientLightColor = _ambientLightColor;
                    effect.View = viewMatrix;
                    effect.World = worldMatrix;
                    effect.Projection = projectionMatrix;
                }

                mesh.Draw();
            }

            base.Draw(time);
        }
    }
}
