using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using GameState;
using GameCamera;
using System;
using System.IO;

namespace GameRenderer
{
    public class Renderer
    {
        private ShaderProgram? shaderProgram;
        private Camera? camera;
        private int vao;
        private int vbo;

        public void Initialize()
        {
            GL.ClearColor(0.0f, 0.0f, 0.1f, 1.0f);
            
            string vertexPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VertexShader.glsl");
            string fragmentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FragmentShader.glsl");
            
            if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
            {
                throw new FileNotFoundException("Shader files not found");
            }
            
            shaderProgram = new ShaderProgram(vertexPath, fragmentPath);
            camera = new Camera(800, 600);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void Render(GameManager gameManager)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shaderProgram?.Use();
            shaderProgram?.SetMatrix4("projection", camera!.ProjectionMatrix);
            shaderProgram?.SetMatrix4("view", camera!.ViewMatrix);

            RenderPlayer(gameManager.PlayerManager.GetPlayerData());
            RenderAsteroids(gameManager.AsteroidManager.GetAsteroids());
            RenderBullets(gameManager.BulletManager.GetBullets());

            RenderUI(gameManager);
        }

        private void RenderPlayer(PlayerData playerData)
        {
            Matrix4 model = Matrix4.CreateRotationZ(playerData.Rotation) *
                    Matrix4.CreateScale(20) * // Increase size
                    Matrix4.CreateTranslation(new Vector3(playerData.Position.X, playerData.Position.Y, 0));
    shaderProgram?.SetMatrix4("model", model);
    DrawTriangle(1.0f, Color4.White); // Add size parameter (1.0f)
        }

        private void RenderAsteroids(List<AsteroidData> asteroids)
        {
            foreach (var asteroid in asteroids)
    {
        Matrix4 model = Matrix4.CreateScale(asteroid.Size) *
                        Matrix4.CreateRotationZ(asteroid.Rotation) *
                        Matrix4.CreateTranslation(new Vector3(asteroid.Position.X, asteroid.Position.Y, 0));
        shaderProgram?.SetMatrix4("model", model);
        DrawPolygon(8, 1.0f, Color4.Gray); // Add radius parameter (1.0f)
            }
        }

        private void RenderBullets(List<BulletData> bullets)
        {
            foreach (var bullet in bullets)
    {
        Matrix4 model = Matrix4.CreateScale(5) * // Increase size
                        Matrix4.CreateTranslation(new Vector3(bullet.Position.X, bullet.Position.Y, 0));
        shaderProgram?.SetMatrix4("model", model);
        DrawCircle(Color4.Yellow);
            }
        }

        private void DrawTriangle(float size, Color4 color)
        {
            float[] vertices = {
                0, size, 0,
                -size, -size, 0,
                size, -size, 0
            };

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram!.Handle, "color"), color);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        private void DrawPolygon(int sides, float radius, Color4 color)
        {
            float[] vertices = new float[sides * 3];
            for (int i = 0; i < sides; i++)
            {
                float angle = i * 2 * MathF.PI / sides;
                vertices[i * 3] = MathF.Cos(angle) * radius;
                vertices[i * 3 + 1] = MathF.Sin(angle) * radius;
                vertices[i * 3 + 2] = 0;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.Uniform4(GL.GetUniformLocation(shaderProgram!.Handle, "color"), color);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, sides);
        }

        private void DrawCircle(Color4 color)
        {
            const int segments = 16;
            DrawPolygon(segments, 1f, color);
        }

        private void RenderUI(GameManager gameManager)
        {
            // TODO: Implement proper text rendering
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Score: {gameManager.PlayerManager.Score}");
            Console.WriteLine($"Health: {gameManager.PlayerManager.Health}");
            Console.WriteLine($"Difficulty: {gameManager.CurrentDifficulty:F2}");
        }

        public void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            camera?.UpdateProjectionMatrix(width, height);
        }

        public void Cleanup()
        {
            shaderProgram?.Dispose();
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
        }
    }
}
