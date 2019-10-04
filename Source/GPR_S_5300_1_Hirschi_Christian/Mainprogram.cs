using OpenGL;
using OpenGL.Game;
using OpenGL.Mathematics;
using OpenGL.Platform;
using System;
using System.Collections.Generic;

namespace GPR_S_5300_1_Hirschi_Christian
{
    public class Mainprogram
    {
        private const int defaultWidth = 800;
        private const int defaultHeight = 600;
        private const float orbitCameraRadius = 5;
        private const float orbitCameraSpeed = 0.01f;
        private const float orbitLightBallRadius = 3;
        private const float orbitLightBallSpeed = 1;
        private Game game;
        private GameObject lightBall;
        private bool isLightBallMoving = true;

        /// <summary>
        /// </summary>
        /// <param name="args">no args</param>
        static void Main(string[] args)
        {
            new Mainprogram().StartMainLoop();
        }

        private void StartMainLoop()
        {
            Time.Initialize();
            Window.CreateWindow("GPR_S_5300_1_Hirschi_Christian", defaultWidth, defaultHeight);
            Window.OnMouseMoveCallbacks.Add(OnMouseMove);
            Window.OnMouseCallbacks.Add(OnMouseClick);
            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            Gl.Enable(EnableCap.DepthTest);

            game = new Game();
            InitializeWorld(game);

            while (Window.Open)
            {
                Window.HandleInput();

                OnPreRenderFrame();

                if (isLightBallMoving)
                {
                    lightBall.Transform.Position = new Vector3(Mathf.Sin(Time.TimeSinceStart * orbitLightBallSpeed) * orbitLightBallRadius, 0, Mathf.Cos(Time.TimeSinceStart * orbitLightBallSpeed) * orbitLightBallRadius);
                }

                game.Update();
                game.Render();

                OnPostRenderFrame();

                Time.Update();
            }
        }

        private void InitializeWorld(Game game)
        {
            game.SceneGraph.Clear();

            lightBall = GenerateLightball("LightBall", 10, 10, 0.1f);
            game.SceneGraph.Add(lightBall);

            game.SceneGraph.Add(GenerateCrate("cube1", 1, 1, 1, "textures/crate.jpg"));

            game.SceneGraph.Add(GenerateSkybox("skybox", 50, 50, 50, "textures/space.png"));
        }

        private GameObject GenerateLightball(string name, int totalLongitude, int totoalLatitude, float radius)
        {
            Material material = Material.Create("shaders\\color.vs", "shaders\\color.fs");
            Vector3[] vertices = Sphere.GenerateVertices(totalLongitude, totoalLatitude, radius);

            List<IGenericVBO> vbos = new List<IGenericVBO>();
            vbos.Add(new GenericVAO.GenericVBO<Vector3>(new VBO<Vector3>(vertices), "POSITION"));
            vbos.Add(new GenericVAO.GenericVBO<Vector4>(new VBO<Vector4>(Sphere.GenerateTintWhite(vertices.Length)), "COLOR"));
            vbos.Add(new GenericVAO.GenericVBO<uint>(new VBO<uint>(Array.ConvertAll(Sphere.GenerateTriangles(totalLongitude, totoalLatitude, vertices.Length), new Converter<int, uint>(PrimitiveForm.intToUint)), BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead)));
            VAO vao = new VAO(material, vbos.ToArray());

            GameObject lightBall = new GameObject(name, new MeshRenderer(material, vao));
            lightBall.GetCustomViewMatrix = GetCustomViewMatrix;
            lightBall.SkipTangentToWorld = true;
            return lightBall;
        }

        private GameObject GenerateCrate(string name, float length, float width, float height, string texture)
        {
            Material material = Material.Create("shaders\\full.vs", "shaders\\full.fs");
            VAO vao = Cube.GenerateVAO(length, width, height, material);
            GameObject crate = new GameObject(name, new MeshRenderer(material, vao));
            crate.GetCustomViewMatrix = GetCustomViewMatrix;
            crate.GetCustomLightMatrix = GetLightData;
            crate.MainTexture = new Texture(texture);
            return crate;
        }

        private GameObject GenerateSkybox(string name, float length, float width, float height, string texture)
        {
            Material material = Material.Create("shaders\\skybox.vs", "shaders\\texture.fs");

            List<IGenericVBO> vbos = new List<IGenericVBO>();
            vbos.Add(new GenericVAO.GenericVBO<Vector3>(new VBO<Vector3>(Cube.GenerateVertices(length, width, height)), "POSITION"));
            vbos.Add(new GenericVAO.GenericVBO<Vector2>(new VBO<Vector2>(Cube.GenerateUVs()), "TEXCOORD"));
            vbos.Add(new GenericVAO.GenericVBO<uint>(new VBO<uint>(Array.ConvertAll(Cube.GenerateTriangles(), new Converter<int, uint>(PrimitiveForm.intToUint)), BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead)));
            VAO vao = new VAO(material, vbos.ToArray());

            GameObject skybox = new GameObject(name, new MeshRenderer(material, vao));
            skybox.GetCustomViewMatrix = GetCustomViewMatrix;
            skybox.SkipTangentToWorld = true;
            skybox.MainTexture = new Texture(texture);
            return skybox;
        }

        private Matrix4 GetLightData()
        {
            float ambientIntensity = 0.3f;
            float diffuseIntensity = 0.8f;
            float specularIntensity = 1.0f;
            float hardness = 256;
            Vector3 lightPosition = lightBall.Transform.Position;
            Vector3 viewPosition = getCameraOrbitPosition(mousePos.X, mousePos.Y, orbitCameraRadius, orbitCameraSpeed);
            Vector3 ambientLightColor = new Vector3(1, 1, 1);
            Vector3 lightColor = new Vector3(1, 1, 1);
            Matrix4 lightData = new Matrix4
            (
                new Vector4(lightPosition, ambientIntensity),
                new Vector4(ambientLightColor, diffuseIntensity),
                new Vector4(lightColor, specularIntensity),
                new Vector4(viewPosition, hardness)
            );
            return lightData;
        }

        private void OnPreRenderFrame()
        {
            Gl.Viewport(0, 0, Window.Width, Window.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private void OnPostRenderFrame()
        {
            OpenGL.UI.UserInterface.Draw();
            Window.SwapBuffers();
        }

        private bool OnMouseClick(int button, int state, int x, int y)
        {
            // take care of mapping the Glut buttons to the UI enums
            if (!OpenGL.UI.UserInterface.OnMouseClick(button + 1, (state == 0 ? 1 : 0), x, y))
            {
                switch (button)
                {
                    case 1: //left mouse button
                        if (state == 1) //pressed
                            SetUniformBoolOnAllMaterials("correctSpeclarReflection", true);
                        else
                            SetUniformBoolOnAllMaterials("correctSpeclarReflection", false);
                        break;
                    case 3: //right mouse button
                        if (state == 1) //pressed
                            isLightBallMoving = false;
                        else
                            isLightBallMoving = true;
                        break;
                    default:
                        break;
                }

            }
            return false;
        }

        private static Vector2 mousePos;
        private static bool OnMouseMove(int x, int y)
        {
            if (!OpenGL.UI.UserInterface.OnMouseMove(x, y))
            {
                mousePos = new Vector2(x, y);
            }
            return false;
        }

        private static Vector3 getCameraOrbitPosition(float x, float y, float radius, float speed)
        {
            return new Vector3(Mathf.Sin(x * speed) * radius, Mathf.Sin(y * speed) * radius, Mathf.Cos(x * speed) * radius);
        }

        private static Matrix4 getCameraOrbitView(Vector3 position)
        {
            return Matrix4.LookAt(position, Vector3.Zero, Vector3.UnitY);
        }

        private static Matrix4 GetCustomViewMatrix()
        {
            return getCameraOrbitView(getCameraOrbitPosition(mousePos.X, mousePos.Y, orbitCameraRadius, orbitCameraSpeed));
        }

        private void SetUniformBoolOnAllMaterials(string paramter, bool value)
        {
            for (int i = 0; i < game.SceneGraph.Count; i++)
            {
                try
                {
                    game.SceneGraph[i].Renderer.Material[paramter].SetValue(value);
                }
                catch (NullReferenceException) { }
            }
        }
    }
}
