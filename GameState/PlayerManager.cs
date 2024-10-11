using System.Numerics;

namespace GameState
{
    public struct PlayerData
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float RotationVelocity;
    }

    public class PlayerManager
    {
        private PlayerData playerData;
        public int Health { get; private set; }
        public int Score { get; private set; }

        private float invulnerabilityTimer = 0f;
        private const float InvulnerabilityDuration = 3f;
        private float shootCooldown = 0f;
        private const float ShootCooldownDuration = 0.5f;

        public PlayerManager()
        {
            ResetPlayer();
            Health = 3;
            Score = 0;
        }

        public void ResetPlayer()
        {
            playerData = new PlayerData
            {
                Position = new Vector2(400, 300),
                Velocity = Vector2.Zero,
                Rotation = 0,
                RotationVelocity = 0
            };
            invulnerabilityTimer = InvulnerabilityDuration;
        }

        public void Update(float deltaTime)
        {
            playerData.Position += playerData.Velocity * deltaTime;
            playerData.Rotation += playerData.RotationVelocity * deltaTime;

            // Wrap around screen
            playerData.Position.X = (playerData.Position.X + 800) % 800;
            playerData.Position.Y = (playerData.Position.Y + 600) % 600;

            // Apply friction
            playerData.Velocity *= 0.99f;
            playerData.RotationVelocity *= 0.99f;

            if (invulnerabilityTimer > 0)
            {
                invulnerabilityTimer -= deltaTime;
            }

            if (shootCooldown > 0)
            {
                shootCooldown -= deltaTime;
            }
        }

        public void Accelerate(float amount)
        {
            Vector2 direction = new Vector2(MathF.Cos(playerData.Rotation), MathF.Sin(playerData.Rotation));
            playerData.Velocity += direction * amount;
        }

        public void Rotate(float amount)
        {
            playerData.RotationVelocity += amount;
        }

        public PlayerData GetPlayerData() => playerData;

        public bool CanShoot() => shootCooldown <= 0;

        public void Shoot()
        {
            if (CanShoot())
            {
                shootCooldown = ShootCooldownDuration;
                // Logic to create a bullet should be handled in GameManager or BulletManager
            }
        }

        public void TakeDamage()
        {
            if (invulnerabilityTimer <= 0)
            {
                Health--;
                invulnerabilityTimer = InvulnerabilityDuration;
            }
        }

        public bool IsInvulnerable() => invulnerabilityTimer > 0;

        public void AddScore(int points)
        {
            Score += points;
        }
    }
}
