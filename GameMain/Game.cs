using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GameState;
using GameInput;
using GameRenderer;
using System;

namespace GameMain
{
    public class Game : GameWindow
    {
        private GameManager gameManager;
        private InputManager inputManager;
        private Renderer renderer;
        private AIController aiController;
        private double accumulator = 0.0;
        private const double fixedTimeStep = 1.0 / 60.0;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            int seed = (int)DateTime.Now.Ticks;
            gameManager = new GameManager(seed);
            inputManager = new InputManager();
            renderer = new Renderer();
            aiController = new AIController(gameManager, seed);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            renderer.Initialize();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            accumulator += args.Time;

            while (accumulator >= fixedTimeStep)
            {
                gameManager.Update((float)fixedTimeStep);
                accumulator -= fixedTimeStep;
            }

            renderer.Render(gameManager);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (KeyboardState.IsKeyPressed(Keys.R))
            {
                ToggleRecording();
            }

            if (KeyboardState.IsKeyPressed(Keys.P))
            {
                ToggleReplay();
            }

            if (gameManager.IsGameOver && KeyboardState.IsKeyPressed(Keys.Enter))
            {
                gameManager.ResetGame();
            }

            if (!inputManager.IsReplaying())
            {
                inputManager.Update(new OpenTKKeyboardStateWrapper(KeyboardState), gameManager);
                aiController.Update((float)args.Time);
            }
        }

        private void ToggleRecording()
        {
            if (inputManager.IsRecording())
            {
                inputManager.StopRecording();
            }
            else
            {
                inputManager.StartRecording();
            }
        }

        private void ToggleReplay()
        {
            if (inputManager.IsReplaying())
            {
                inputManager.StopReplay();
            }
            else
            {
                inputManager.StartReplay(gameManager);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            renderer.Resize(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            renderer.Cleanup();
            base.OnUnload();
        }
    }
}

public class OpenTKKeyboardStateWrapper : IKeyboardState
{
    private readonly KeyboardState _keyboardState;

    public OpenTKKeyboardStateWrapper(KeyboardState keyboardState)
    {
        _keyboardState = keyboardState;
    }

    public bool IsKeyDown(Keys key) => _keyboardState.IsKeyDown(key);
}
