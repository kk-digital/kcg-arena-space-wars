using GameState;
using System.Numerics;
using System;
using Newtonsoft.Json;

namespace GameInput
{
    [JsonConverter(typeof(InputCommandConverter))]
    public abstract class InputCommand
    {
        public int Frame { get; set; }
        public abstract void Execute(GameManager gameManager);
    }

    public class AccelerateCommand : InputCommand
    {
        public float Amount { get; set; }

        public AccelerateCommand() { } // Parameterless constructor for deserialization

        [JsonConstructor]
        public AccelerateCommand(float amount)
        {
            Amount = amount;
        }

        public override void Execute(GameManager gameManager)
        {
            gameManager.PlayerManager.Accelerate(Amount);
        }
    }

    public class RotateCommand : InputCommand
    {
        public float Amount { get; set; }

        public RotateCommand() { } // Parameterless constructor for deserialization

        [JsonConstructor]
        public RotateCommand(float amount)
        {
            Amount = amount;
        }

        public override void Execute(GameManager gameManager)
        {
            gameManager.PlayerManager.Rotate(Amount);
        }
    }

    public class FireCommand : InputCommand
    {
        public FireCommand() { } // Parameterless constructor for deserialization

        public override void Execute(GameManager gameManager)
        {
            var playerData = gameManager.PlayerManager.GetPlayerData();
            Vector2 direction = new Vector2(MathF.Cos(playerData.Rotation), MathF.Sin(playerData.Rotation));
            gameManager.BulletManager.SpawnBullet(playerData.Position, direction * 200f);
        }
    }
}
