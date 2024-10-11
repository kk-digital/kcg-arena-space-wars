using GameState;
using System;
using System.Numerics;

namespace GameInput
{
    public class AIController
    {
        private GameManager gameManager;
        private Random random;
        private bool isActive = false;

        public AIController(GameManager gameManager, int seed)
        {
            this.gameManager = gameManager;
            this.random = new Random(seed);
        }

        public void Update(float deltaTime)
        {
            if (!isActive) return;

            var playerData = gameManager.AIPlayerManager.GetPlayerData();
            var nearestAsteroid = FindNearestAsteroid(playerData.Position);

            if (nearestAsteroid != null)
            {
                HandleAsteroidInteraction(playerData, nearestAsteroid.Value);
            }
            else
            {
                PerformRandomMovement();
            }
        }

        private void HandleAsteroidInteraction(PlayerData playerData, AsteroidData asteroid)
        {
            Vector2 toAsteroid = asteroid.Position - playerData.Position;
            float angleToAsteroid = MathF.Atan2(toAsteroid.Y, toAsteroid.X);

            // Rotate towards asteroid
            float angleDiff = AngleDifference(playerData.Rotation, angleToAsteroid);
            if (Math.Abs(angleDiff) > 0.1f)
            {
                gameManager.AIPlayerManager.Rotate(Math.Sign(angleDiff) * 0.1f);
                Console.WriteLine("AI: Rotating towards asteroid");
            }

            // Accelerate towards asteroid
            if (toAsteroid.Length() > 200)
            {
                gameManager.AIPlayerManager.Accelerate(5f);
                Console.WriteLine("AI: Accelerating towards asteroid");
            }

            // Fire if aimed correctly and close enough
            if (Math.Abs(angleDiff) < 0.2f && toAsteroid.Length() < 300)
            {
                Vector2 direction = new Vector2(MathF.Cos(playerData.Rotation), MathF.Sin(playerData.Rotation));
                gameManager.BulletManager.SpawnBullet(playerData.Position, direction * 200f);
                Console.WriteLine("AI: Firing at asteroid");
            }
        }

        private void PerformRandomMovement()
        {
            // No asteroids in sight, move randomly
            if (random.Next(100) < 5)
            {
                gameManager.AIPlayerManager.Rotate((float)(random.NextDouble() - 0.5) * 0.2f);
                Console.WriteLine("AI: Random rotation");
            }
            if (random.Next(100) < 2)
            {
                gameManager.AIPlayerManager.Accelerate(5f);
                Console.WriteLine("AI: Random acceleration");
            }
        }

        private AsteroidData? FindNearestAsteroid(Vector2 position)
        {
            AsteroidData? nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var asteroid in gameManager.AsteroidManager.GetAsteroids())
            {
                float distance = Vector2.Distance(position, asteroid.Position);
                if (distance < nearestDistance)
                {
                    nearest = asteroid;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        private float AngleDifference(float angle1, float angle2)
        {
            float diff = (angle2 - angle1 + MathF.PI) % (2 * MathF.PI) - MathF.PI;
            return diff < -MathF.PI ? diff + 2 * MathF.PI : diff;
        }

        public void ToggleAI()
        {
            isActive = !isActive;
            Console.WriteLine($"AI Controller is now {(isActive ? "active" : "inactive")}");
        }

        public bool IsActive() => isActive;
    }
}
