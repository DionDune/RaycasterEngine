﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaycasterEngine
{
    internal class Camera
    {
        public Vector2 WorldPosition { get; set; }

        public int FOV { get; set; }
        public float Direction { get; set; }

        float RenderDistance { get; set; }
        float RayJumpDistance { get; set; }

        public Camera(Settings settings, Vector2 WorldPosition)
        {
            this.WorldPosition = WorldPosition;
            FOV = settings.cameraFOV;
            Direction = 0;

            RenderDistance = settings.cameraRenderDistance;
            RayJumpDistance = 0.1f;
        }


        public void CastRays(SpriteBatch spriteBatch, Settings settings, Screen Screen, Grid Grid)
        {
            int RayCount = Screen.RayCount;
            float RayAngleJump = (float)FOV / RayCount;
            float CurrentAngle = 0;

            for (int i = 0; i < RayCount; i++)
            {
                CastRay(spriteBatch, settings, Screen, Grid, (Direction + CurrentAngle) , i * Screen.RayWidth);

                CurrentAngle += RayAngleJump;
            }
        }
        private void CastRay(SpriteBatch spriteBatch, Settings settings, Screen Screen, Grid Grid, float Angle, int ScreenPosX)
        {
            Angle = Angle * (float)(Math.PI / 180);

            Vector2 CurrentPosition = new Vector2(WorldPosition.X, WorldPosition.Y);
            Vector2 PointPositionChange = new Vector2(RayJumpDistance * (float)Math.Cos(Angle),
                                                       RayJumpDistance * (float)Math.Sin(Angle));

            

            // Total number of points to be checked
            int PointCount = (int)(RenderDistance / RayJumpDistance);

            for (int i = 0; i < PointCount; i++)
            {
                CurrentPosition += PointPositionChange;
                int HalfRenderHeight;
                int ScreenPosY;


                bool borderReached = true;
                if (CurrentPosition.X > 0 && CurrentPosition.X < Grid.Dimentions.X &&
                    CurrentPosition.Y > 0 && CurrentPosition.Y < Grid.Dimentions.Y)
                {
                    GridSlot Slot = Grid.Slots[(int)CurrentPosition.Y][(int)CurrentPosition.X];
                    
                    // RENDER SOLID
                    if (Slot != null)
                    {
                        HalfRenderHeight = (int)( 180F / (Vector2.Distance(WorldPosition, CurrentPosition) / 100) ) / 2;
                        ScreenPosY = (Screen.Dimentions.Y / 2) - HalfRenderHeight;

                        spriteBatch.Draw(Game1.TextureWhite, new Rectangle(ScreenPosX, ScreenPosY, 
                                                                            Screen.RayWidth, HalfRenderHeight * 2),
                                                                            Slot.Color);


                        return;
                    }

                    borderReached = false;
                }

                // RENDER MAX DISTANCE REACHED or GRID BORDER
                if (borderReached || (settings.cameraRenderRayEnd && i == PointCount - 1))
                {
                    HalfRenderHeight = (int)(180F / (Vector2.Distance(WorldPosition, CurrentPosition) / 100)) / 2;
                    ScreenPosY = (Screen.Dimentions.Y / 2) - HalfRenderHeight;

                    if (borderReached)
                        spriteBatch.Draw(Game1.TextureWhite, new Rectangle(ScreenPosX, ScreenPosY,
                                                                            Screen.RayWidth, HalfRenderHeight * 2),
                                                                            Color.Gray * 0.5f);
                    else
                        spriteBatch.Draw(Game1.TextureWhite, new Rectangle(ScreenPosX, ScreenPosY,
                                                                            Screen.RayWidth, HalfRenderHeight * 2),
                                                                            Color.Purple * 0.5f);

                    return;
                }
            }
        }
    }
}
