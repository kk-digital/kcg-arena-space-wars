using OpenTK.Mathematics;

namespace GameCamera
{
    public class Camera
    {
        public Matrix4 ProjectionMatrix { get; private set; }
        public Matrix4 ViewMatrix { get; private set; }

        public Camera(float width, float height)
        {
            UpdateProjectionMatrix(width, height);
            ViewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
        }

        public void UpdateProjectionMatrix(float width, float height)
        {
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
        }
    }
}
