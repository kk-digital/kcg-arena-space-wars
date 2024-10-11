using System.Collections.Generic;
using System.Numerics;

namespace GameState
{
    public struct BulletData
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Lifetime;
    }

    public class BulletManager
    {
        private List<BulletData> bullets;

        public BulletManager()
        {
            bullets = new List<BulletData>();
        }

        public void Update(float deltaTime)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bullet.Position += bullet.Velocity * deltaTime;
                bullet.Lifetime -= deltaTime;

                if (bullet.Lifetime <= 0)
                {
                    bullets.RemoveAt(i);
                }
                else
                {
                    bullets[i] = bullet;
                }
            }
        }

        public void SpawnBullet(Vector2 position, Vector2 velocity)
        {
            var bullet = new BulletData
            {
                Position = position,
                Velocity = velocity,
                Lifetime = 2f // Bullets live for 2 seconds
            };

            bullets.Add(bullet);
        }

        public List<BulletData> GetBullets() => bullets;

        public void RemoveBullet(int index)
        {
            if (index >= 0 && index < bullets.Count)
            {
                bullets.RemoveAt(index);
            }
        }
    }
}
