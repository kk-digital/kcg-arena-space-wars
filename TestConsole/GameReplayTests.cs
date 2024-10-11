using GameState;
using GameInput;
using System;
using System.Collections.Generic;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TestConsole
{
    public class GameReplayTests
    {
        private class MockKeyboardState : IKeyboardState
        {
            public bool IsKeyDown(Keys key) => false;
        }

        public static void RunTest()
        {
            var gameManager = new GameManager((int)DateTime.Now.Ticks);
            var inputManager = new InputManager();

            // Record some inputs
            var inputs = new List<InputCommand>
            {
                new AccelerateCommand(5f) { Frame = 0 },
                new RotateCommand(0.1f) { Frame = 1 },
                new FireCommand() { Frame = 2 }
            };

            foreach (var input in inputs)
            {
                input.Execute(gameManager);
                inputManager.Update(new MockKeyboardState(), gameManager);
            }

            inputManager.SaveInputBufferToJson("test_inputs.json");

            // Reset game state
            gameManager = new GameManager((int)DateTime.Now.Ticks);

            // Replay inputs
            inputManager.LoadInputBufferFromJson("test_inputs.json");
            inputManager.StartReplay(gameManager);
            while (inputManager.IsReplaying())
            {
                inputManager.Update(new MockKeyboardState(), gameManager);
            }

            // Check game state after replay
            var playerData = gameManager.PlayerManager.GetPlayerData();
            Console.WriteLine($"Player position: {playerData.Position}");
            Console.WriteLine($"Player rotation: {playerData.Rotation}");
            Console.WriteLine($"Bullets fired: {gameManager.BulletManager.GetBullets().Count}");
        }
    }
}
