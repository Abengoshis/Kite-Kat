using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using EcsCore.Components;

namespace KiteKat.Components
{
    class Spring : IEcsComponent
    {
        public int ConnectedEntity;
        public float Stiffness;
        public float Damping;
        public Vector2 RestOrigin;
        public float RestDistance;
        public float? RestAngle;
        public float? FixedRotation;
        public float? MaxForce;

        public Spring(int attachmentEntity, float stiffness, float damping, Vector2 restOrigin, float restDistance, float? restAngle = null, float? fixedRotation = null, float? maxForce = null)
        {
            ConnectedEntity = attachmentEntity;
            Stiffness = stiffness;
            Damping = damping;
            RestOrigin = restOrigin;
            RestDistance = restDistance;
            RestAngle = restAngle;
            FixedRotation = fixedRotation;
            MaxForce = maxForce;
        }

        public Spring()
        {
        }
    }
}
