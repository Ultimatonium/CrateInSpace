namespace OpenGL.Game
{
    public class MeshRenderer
    {
        public Material Material { get; set; }
        public VAO Geometry { get; set; }
        public GameObject gameObject { get; internal set; }

        public MeshRenderer()
        {
        }

        public MeshRenderer(Material material, VAO vao)
        {
            Material = material;
            Geometry = vao;
        }

        public void Render()
        {
            Geometry.Program.Use();
            gameObject.Commit();
            Geometry.Draw();
        }
    }
}
