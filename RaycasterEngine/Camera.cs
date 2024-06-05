using Microsoft.Xna.Framework;
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
            RayJumpDistance = settings.cameraRenderPointInterval;
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


                        List<Point> CornerScreenPositions = GetCornerScreenPositions(Screen, Slot);
                        foreach (Point CornerScreenPos in CornerScreenPositions)
                        {
                            spriteBatch.Draw(Game1.TextureWhite, new Rectangle(
                                CornerScreenPos.X,
                                CornerScreenPos.Y,
                                5, 5 ), 
                                Color.Pink);
                        }


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

        private List<Point> GetCornerScreenPositions(Screen Screen, GridSlot Slot)
        {
            List<Point> CornerScreenPositions = new List<Point>();
            List<Point> Corners = new List<Point>()
            {
                new Point(Slot.Position.X, Slot.Position.Y),
                new Point(Slot.Position.X + 1, Slot.Position.Y),
                new Point(Slot.Position.X + 1, Slot.Position.Y + 1),
                new Point(Slot.Position.X, Slot.Position.Y + 1)
            };


            foreach (Point Corner in Corners)
            {
                float WorldAngle = (float)Math.Atan2(Corner.Y - WorldPosition.Y, Corner.X - WorldPosition.X) * (float)(180 / Math.PI);
                float ReletiveAngle = WorldAngle - Direction;
                ReletiveAngle /= FOV;

                float HalfRenderHeight = (int)(180F / (Vector2.Distance(WorldPosition, Corner.ToVector2()) / 100)) / 2;
                float ScreenPosY = (Screen.Dimentions.Y / 2) - HalfRenderHeight;

                // Top
                CornerScreenPositions.Add(new Point(
                    (int)(Screen.Dimentions.X * ReletiveAngle),
                    (Screen.Dimentions.Y / 2) - (int)HalfRenderHeight
                    ));
                // Bottom
                CornerScreenPositions.Add(new Point(
                    (int)(Screen.Dimentions.X * ReletiveAngle),
                    (Screen.Dimentions.Y / 2) + (int)HalfRenderHeight
                    ));
            }


            return CornerScreenPositions;
        } 
    }
}
