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
        public bool cameraRenderWireFrames { get; set; }
        public bool cameraRenderBaseRays { get; set; }
        public float cameraMovementSpeed { get; set; }
        public float cameraRotationSpeed { get; set; }

        public bool cameraWireFrameEfficientMode { get; set; }
        
        public Point gridDimentions { get; set; }


        public Settings()
        {
            cameraFOV = 120;
            cameraRenderDistance = 5000;
            cameraRenderPointInterval = 1f;
            cameraRenderRayEnd = true;
            cameraMovementSpeed = 0.5f;
            cameraRotationSpeed = 1.2f;

            cameraRenderBaseRays = true;
            cameraRenderWireFrames = true;
            cameraWireFrameEfficientMode = true;


            gridDimentions = new Point(1000, 1000);
        }
    }
}
