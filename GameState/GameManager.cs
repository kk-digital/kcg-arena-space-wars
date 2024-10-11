using System;
using System.Collections.Generic;
using System.Numerics;

namespace GameState
{
    public class GameManager
    {
        public AsteroidManager AsteroidManager { get; private set; }
        public PlayerManager PlayerManager { get; private set; }
        public PlayerManager AIPlayerManager { get; private set; }
        public BulletManager BulletManager { get; private set; }
        private RandomGenerator random;

        private float asteroidSpawnTimer = 0f;
        private float difficultyTimer = 0f;
        private const float AsteroidSpawnInterval = 3f;
        private const float DifficultyIncreaseInterval = 30f;
        public float CurrentDifficulty { get; private set; } = 1f;

        public bool IsGameOver => PlayerManager.Health <= 0;

        public GameManager(int seed)
        {
            random = new RandomGenerator(seed);
            AsteroidManager = new AsteroidManager(random);
            PlayerManager = new PlayerManager();
            AIPlayerManager = new PlayerManager();
            BulletManager = new BulletManager();
        }

        public void Update(float deltaTime)
        {
            if (IsGameOver) return;

            AsteroidManager.Update(deltaTime);
            PlayerManager.Update(deltaTime);
            AIPlayerManager.Update(deltaTime);
            BulletManager.Update(deltaTime);

            CheckCollisions();
            UpdateAsteroidSpawning(deltaTime);
            UpdateDifficulty(deltaTime);
        }

        private void UpdateAsteroidSpawning(float deltaTime)
        {
            asteroidSpawnTimer += deltaTime;
            if (asteroidSpawnTimer >= AsteroidSpawnInterval / CurrentDifficulty)
            {
                AsteroidManager.SpawnAsteroid(CurrentDifficulty);
                asteroidSpawnTimer = 0f;
            }
        }

        private void UpdateDifficulty(float deltaTime)
        {
            difficultyTimer += deltaTime;
            if (difficultyTimer >= DifficultyIncreaseInterval)
            {
                CurrentDifficulty += 0.1f;
                difficultyTimer = 0f;
                Console.WriteLine($"Difficulty increased to: {CurrentDifficulty:F2}");
            }
        }

        private void CheckCollisions()
        {
            var asteroids = AsteroidManager.GetAsteroids();
            var bullets = BulletManager.GetBullets();

            // Check bullet-asteroid collisions
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (Vector2.Distance(bullets[i].Position, asteroids[j].Position) < asteroids[j].Size)
                    {
                        BulletManager.RemoveBullet(i);
                        AsteroidManager.SplitAsteroid(j);
                        PlayerManager.AddScore(100);
                        break;
                    }
                }
            }

            // Check player-asteroid collisions
            CheckPlayerAsteroidCollisions(PlayerManager);
            CheckPlayerAsteroidCollisions(AIPlayerManager);
        }

        private void CheckPlayerAsteroidCollisions(PlayerManager player)
        {
            if (!player.IsInvulnerable())
            {
                var playerData = player.GetPlayerData();
                var asteroids = AsteroidManager.GetAsteroids();
                for (int i = asteroids.Count - 1; i >= 0; i--)
                {
                    if (Vector2.Distance(playerData.Position, asteroids[i].Position) < asteroids[i].Size + 10)
                    {
                        player.TakeDamage();
                        AsteroidManager.RemoveAsteroid(i);
                        break;
                    }
                }
            }
        }

        public void ResetGame()
        {
            random = new RandomGenerator((int)DateTime.Now.Ticks);
            PlayerManager.ResetPlayer();
            AIPlayerManager.ResetPlayer();
            AsteroidManager = new AsteroidManager(random);
            BulletManager = new BulletManager();
            CurrentDifficulty = 1f;
            asteroidSpawnTimer = 0f;
            difficultyTimer = 0f;
        }
    }
}
