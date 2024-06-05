using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaycasterEngine
{
    internal class Settings
    {
        public int cameraFOV { get; set; }
        public float cameraRenderDistance { get; set; }
        public float cameraRenderPointInterval { get; set; }
        public bool cameraRenderRayEnd { get; set; }
        public float cameraMovementSpeed { get; set; }
        public float cameraRotationSpeed { get; set; }
        

        public Point gridDimentions { get; set; }


        public Settings()
        {
            cameraFOV = 120;
            cameraRenderDistance = 150;
            cameraRenderPointInterval = 0.1f;
            cameraRenderRayEnd = true;
            cameraMovementSpeed = 0.2f;
            cameraRotationSpeed = 1.2f;
            

            gridDimentions = new Point(200, 200);
        }
    }
}
