using System.Collections.Generic;

namespace OpenGL.Game
{
    public class Game
    {
        public List<GameObject> SceneGraph { get; set; }

        public Game()
        {
            SceneGraph = new List<GameObject>();
        }

        public void Render()
        {
            foreach (GameObject gameObject in SceneGraph)
            {
                gameObject.Renderer.Render();
            }
        }

        public void Update()
        {
            foreach (GameObject gameObject in SceneGraph)
            {
                gameObject.Update();
            }
        }
    }
}
