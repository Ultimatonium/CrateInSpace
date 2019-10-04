using System.Security.Principal;
using OpenGL.Mathematics;
using OpenGL.Platform;

namespace OpenGL.Game
{
    public class GameObject
    {
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public MeshRenderer Renderer { get; set; }
        public bool SkipTransformation { get; set; }
        
        public GameObject(string name, MeshRenderer meshRenderer)
        {
            Name = name;
            Renderer = meshRenderer;
            Renderer.gameObject = this;
            Initialize();
        }

        public virtual void Initialize()
        {
            Transform = new Transform();
            Transform.Scale = new Vector3(1,1,1);
        }

        public void Update()
        {

        }

        internal void Commit()
        {
            SetTransform();
        }

        private void SetTransform()
        {
            Matrix4 view;
            Matrix4 projection;
            Matrix4 tangentToWorld = Transform.GetTRS().Inverse().Transpose();
            if (SkipTransformation)
            {
                 view = Matrix4.Identity;
                 projection = Matrix4.Identity;
            }
            else
            {
                view = GetViewMatrix();
                projection = GetProjectionMatrix();
            }

            //--------------------------
            // Data passing to shader
            //--------------------------
            Material material = Renderer.Material;

            material["projection"].SetValue(projection);
            material["view"].SetValue(view);
            material["model"].SetValue(Transform.GetTRS());
            material["tangentToWorld"].SetValue(tangentToWorld);
        }

        private static Matrix4 GetProjectionMatrix()
        {
            float fov = 45;

            float aspectRatio = (float)Window.Width / (float)Window.Height;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(Mathf.ToRad(fov), (float)Window.Width / Window.Height, 0.1f, 1000f);
            //projection = Matrix4.CreateOrthographic0.0f, (float)screenWidth, 0.0f, (float)screenHeight, 0.1f, 100.0f);

            return projection;
        }

        private static Matrix4 GetViewMatrix()
        {
            Matrix4 viewTranslation = Matrix4.Identity;
            Matrix4 viewRotation = Matrix4.Identity;
            Matrix4 viewScale = Matrix4.Identity;

            viewTranslation = Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f));
            viewRotation = Matrix4.CreateRotation(new Vector3(0.0f, 0.0f, 1.0f), 0.0f);
            viewScale = Matrix4.CreateScaling(new Vector3(1.0f, 1.0f, 1.0f));

            //Matrix4 view = viewTranslation * viewRotation * viewScale;// TRS matrix -> scale, rotate then translate -> All applied in WORLD Coordinates
            Matrix4 view = viewRotation * viewTranslation * viewScale;// RTS matrix -> scale, rotate then translate -> All applied in LOCAL Coordinates

            return view;
        }

    }
}
