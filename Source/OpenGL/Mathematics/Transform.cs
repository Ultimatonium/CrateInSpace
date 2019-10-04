using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL.Mathematics
{
    public class Transform
    {
        #region Properties

        public Vector3 Position
        {
            get;
            set;
        }

        public Vector3 Scale
        {
            get;
            set;
        }


        public Vector3 Rotation
        {
            get;
            set;
        }

        #endregion

        #region Properties

        public Matrix4 GetTRS()
        {
            Matrix4 modelTranslation = Matrix4.CreateTranslation(Position);

            Matrix4 modelRotationX = Matrix4.CreateRotation(new Vector3(1.0f, 0.0f, 0.0f), Mathf.ToRad(Rotation.X));
            Matrix4 modelRotationY = Matrix4.CreateRotation(new Vector3(0.0f, 1.0f, 0.0f), Mathf.ToRad(Rotation.Y));
            Matrix4 modelRotationZ = Matrix4.CreateRotation(new Vector3(0.0f, 0.0f, 1.0f), Mathf.ToRad(Rotation.Z));
            Matrix4 modelRotation = modelRotationX * modelRotationY * modelRotationZ;

            Matrix4 modelScale = Matrix4.CreateScaling(Scale);

            //Matrix4 model = modelTranslation * modelRotation * modelScale;// TRS matrix -> scale, rotate then translate -> All applied in WORLD Coordinates
            Matrix4 model = modelRotation * modelTranslation * modelScale;// RTS matrix -> scale, rotate then translate -> All applied in LOCAL Coordinates

            return model;
        }

        #endregion

    }
}
