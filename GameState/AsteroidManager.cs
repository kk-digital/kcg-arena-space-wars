using System;
using System.Collections.Generic;
using System.Numerics;

namespace GameState
{
    public struct AsteroidData
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float RotationSpeed;
        public float Size;
    }

    public class AsteroidManager
    {
        private List<AsteroidData> asteroids;
        private RandomGenerator random;

        public AsteroidManager(RandomGenerator random)
        {
            asteroids = new List<AsteroidData>();
            this.random = random;
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < asteroids.Count; i++)
            {
                var asteroid = asteroids[i];
                asteroid.Position += asteroid.Velocity * deltaTime;
                asteroid.Rotation += asteroid.RotationSpeed * deltaTime;
                
                // Wrap around screen
                asteroid.Position.X = (asteroid.Position.X + 800) % 800;
                asteroid.Position.Y = (asteroid.Position.Y + 600) % 600;

                asteroids[i] = asteroid;
            }
        }

        public void SpawnAsteroid(float difficulty)
        {
            float size = random.NextFloat() < 0.7f ? 30f : (random.NextFloat() < 0.7f ? 20f : 10f);

            var asteroid = new AsteroidData
            {
                Position = new Vector2(random.Next(800), random.Next(600)),
                Velocity = new Vector2(random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f)) * 50f * difficulty,
                Rotation = random.NextFloat() * MathF.PI * 2,
                RotationSpeed = random.NextFloat(-1f, 1f) * 2f * difficulty,
                Size = size
            };

            asteroids.Add(asteroid);
        }

        public List<AsteroidData> GetAsteroids() => asteroids;

        public void RemoveAsteroid(int index)
        {
            if (index >= 0 && index < asteroids.Count)
            {
                asteroids.RemoveAt(index);
            }
        }

        public void SplitAsteroid(int index)
        {
            if (index < 0 || index >= asteroids.Count) return;

            var asteroid = asteroids[index];
            if (asteroid.Size > 10f) // Only split if not already the smallest size
            {
                float newSize = asteroid.Size / 2;
                Vector2 position = asteroid.Position;

                for (int i = 0; i < 2; i++)
                {
                    var newAsteroid = new AsteroidData
                    {
                        Position = position,
                        Velocity = new Vector2(random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f)) * 50f,
                        Rotation = random.NextFloat() * MathF.PI * 2,
                        RotationSpeed = random.NextFloat(-1f, 1f) * 2f,
                        Size = newSize
                    };
                    asteroids.Add(newAsteroid);
                }
            }

            asteroids.RemoveAt(index);
        }
    }
}
