using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
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


        public void CastRays(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, Settings settings, Screen Screen, Grid Grid)
        {
            int RayCount = Screen.RayCount;
            float RayAngleJump = (float)FOV / RayCount;
            float CurrentAngle = 0;

            if (settings.cameraWireFrameEfficientMode)
            {
                if (settings.cameraRenderFaces)
                {
                    if (settings.cameraFaceRenderFast)
                        renderFacesFast(settings, spriteBatch, GraphicsDevice, Screen, Grid);
                    else
                        renderFaces(settings, spriteBatch, GraphicsDevice, Screen, Grid);
                }
            }
            else
                for (int i = 0; i < RayCount; i++)
                {
                    CastRay(spriteBatch, settings, Screen, Grid, Direction + CurrentAngle, i * Screen.RayWidth);

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
                        if (settings.cameraRenderBaseRays)
                        {
                            HalfRenderHeight = (int)(180F / (Vector2.Distance(WorldPosition, CurrentPosition) / 100)) / 2;
                            ScreenPosY = (Screen.Dimentions.Y / 2) - HalfRenderHeight;

                            spriteBatch.Draw(Game1.TextureWhite, new Rectangle(ScreenPosX, ScreenPosY,
                                                                                Screen.RayWidth, HalfRenderHeight * 2),
                                                                                Slot.Color);
                        }
                        

                        if (settings.cameraRenderWireFrames)
                        {
                            List<Point> CornerScreenPositions = GetCornerScreenPositions(Screen, Slot);
                            foreach (Point CornerScreenPos in CornerScreenPositions)
                            {
                                foreach (Point OtherCornerPos in CornerScreenPositions)
                                {
                                    if (OtherCornerPos != CornerScreenPos)
                                    {
                                        Game1.DrawLineBetween(spriteBatch, CornerScreenPos.ToVector2(), OtherCornerPos.ToVector2(), Color.Pink, 1f);
                                    }
                                }
                            }
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

        private void renderFaces(Settings settings, SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, Screen Screen, Grid Grid)
        {
            List<(GridSlot, float)> slotListOrdered = new List<(GridSlot, float)>();

            // Add initial slot
            if (Grid.SolidSlots.Count > 0)
            {
                slotListOrdered.Add((Grid.SolidSlots[0], Vector2.Distance(Grid.SolidSlots[0].Position.ToVector2(), WorldPosition)));
            }
            // Create list of slots, ordered from farthest to closest
            for (int i = 1; i < Grid.SolidSlots.Count; i++)
            {
                float SlotDistance = Vector2.Distance(Grid.SolidSlots[i].Position.ToVector2(), WorldPosition);

                for (int y = 0; y < slotListOrdered.Count; y++)
                {
                    if (SlotDistance < slotListOrdered[y].Item2 || SlotDistance == slotListOrdered[y].Item2 || y == slotListOrdered.Count - 1)
                    {
                        slotListOrdered.Insert(y,(Grid.SolidSlots[i], SlotDistance));
                        break;
                    }
                }
            }

            // Render faces
            for (int i = slotListOrdered.Count() - 1; i >= 0; i--)
            {
                List<List<Point>> Faces = GetFaces(Screen, slotListOrdered[i].Item1);

                foreach (List<Point> Face in Faces)
                {
                    if (Face.Count == 3)
                        Game1.DrawTriangle(Game1._basicEffect, GraphicsDevice, new Vector3(Face[0].X, Face[0].Y, 0),
                                                                            new Vector3(Face[1].X, Face[1].Y, 0),
                                                                            new Vector3(Face[2].X, Face[2].Y, 0), slotListOrdered[i].Item1.Color);
                }

                if (settings.cameraRenderWireFrames) 
                    foreach (List<Point> Face in Faces)
                    {
                        if (Face.Count == 3)
                        {
                            Game1.DrawLineBetween(spriteBatch, Face[0].ToVector2(), Face[1].ToVector2(), Color.Pink, 1f);
                            Game1.DrawLineBetween(spriteBatch, Face[0].ToVector2(), Face[2].ToVector2(), Color.Pink, 1f);
                            Game1.DrawLineBetween(spriteBatch, Face[1].ToVector2(), Face[2].ToVector2(), Color.Pink, 1f);
                        }
                    }
            }
        }
        private void renderFacesFast(Settings settings, SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, Screen Screen, Grid Grid)
        {
            foreach (GridSlot Slot in Grid.SolidSlots)
            {
                List<List<Point>> Faces = GetFaces(Screen, Slot);

                foreach (List<Point> Face in Faces)
                {
                    if (Face.Count == 3)
                        Game1.DrawTriangle(Game1._basicEffect, GraphicsDevice, new Vector3(Face[0].X, Face[0].Y, 0),
                                                                            new Vector3(Face[1].X, Face[1].Y, 0),
                                                                            new Vector3(Face[2].X, Face[2].Y, 0), Slot.Color);
                }

                if (settings.cameraRenderWireFrames)
                    foreach (List<Point> Face in Faces)
                    {
                        if (Face.Count == 3)
                        {
                            Game1.DrawLineBetween(spriteBatch, Face[0].ToVector2(), Face[1].ToVector2(), Color.Pink, 1f);
                            Game1.DrawLineBetween(spriteBatch, Face[0].ToVector2(), Face[2].ToVector2(), Color.Pink, 1f);
                            Game1.DrawLineBetween(spriteBatch, Face[1].ToVector2(), Face[2].ToVector2(), Color.Pink, 1f);
                        }
                    }
            }
        }
        private void renderWireFramesOther(SpriteBatch spriteBatch, Screen Screen, Grid Grid)
        {
            foreach (GridSlot Slot in Grid.SolidSlots)
            {
                List<Point> CornerScreenPositions = GetCornerScreenPositions(Screen, Slot);
                foreach (Point CornerScreenPos in CornerScreenPositions)
                {
                    spriteBatch.Draw(Game1.TextureWhite, new Rectangle(CornerScreenPos.X, CornerScreenPos.Y, 5, 5), Slot.Color);
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
                float AngleOffset;
                float WorldAngle = (float)Math.Atan2(Corner.Y - WorldPosition.Y, Corner.X - WorldPosition.X) * (float)(180 / Math.PI); ;
                float ReletiveAngle;

                if (WorldAngle < Direction)
                {
                    AngleOffset = 360 - Direction;
                    ReletiveAngle = WorldAngle + AngleOffset;
                }
                else
                {
                    AngleOffset = WorldAngle - Direction;
                    ReletiveAngle = AngleOffset;
                }
                ReletiveAngle /= FOV;


                if (ReletiveAngle >= 0 && ReletiveAngle <= 1)
                {
                    float HalfRenderHeight = (int)(180F / (Vector2.Distance(WorldPosition, Corner.ToVector2()) / 100)) / 2;

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
            }

            return CornerScreenPositions;
        }
        private List<List<Point>> GetFaces(Screen Screen, GridSlot Slot)
        {
            List<List<Point>> FaceScreenPositions = new List<List<Point>>();
            List<List<Vector3>> Faces = new List<List<Vector3>>()
            {
                // Front 1
                new List<Vector3>()
                {
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y, -1),
                    new Vector3(Slot.Position.X, Slot.Position.Y, -1),
                    new Vector3(Slot.Position.X, Slot.Position.Y, +1)
                },
                // Front 2
                new List<Vector3>
                {
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y, - 1),
                    new Vector3(Slot.Position.X, Slot.Position.Y, +1),
                    new Vector3(Slot.Position.X + 1 , Slot.Position.Y, +1)
                },
                //Left 1
                new List<Vector3>()
                {
                    new Vector3(Slot.Position.X, Slot.Position.Y, -1),
                    new Vector3(Slot.Position.X, Slot.Position.Y + 1, -1),
                    new Vector3(Slot.Position.X, Slot.Position.Y, +1)
                },
                //Left 2
                new List<Vector3>()
                {
                    new Vector3(Slot.Position.X, Slot.Position.Y, +1),
                    new Vector3(Slot.Position.X, Slot.Position.Y + 1, -1),
                    new Vector3(Slot.Position.X, Slot.Position.Y + 1, +1)
                },
                //Back 1
                new List<Vector3>()
                {
                    new Vector3(Slot.Position.X, Slot.Position.Y + 1, -1),
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y + 1, -1),
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y + 1, +1)
                },
                //Back 2
                new List<Vector3>()
                {
                    new Vector3(Slot.Position.X, Slot.Position.Y + 1, -1),
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y + 1, +1),
                    new Vector3(Slot.Position.X, Slot.Position.Y + 1, +1)
                },
                //Right 1
                new List<Vector3>()
                {
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y + 1, -1),
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y, -1),
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y + 1, +1)
                },
                //Right 2
                new List<Vector3>()
                {
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y + 1, +1),
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y, -1),
                    new Vector3(Slot.Position.X + 1, Slot.Position.Y, +1)
                }
            };
            foreach (List<Vector3> Face in Faces)
            {
                FaceScreenPositions.Add(new List<Point>());

                foreach (Vector3 Vert in Face)
                {
                    float AngleOffset;
                    float WorldAngle = (float)Math.Atan2(Vert.Y - WorldPosition.Y, Vert.X - WorldPosition.X) * (float)(180 / Math.PI);
                    float ReletiveAngle;

                    if (WorldAngle < Direction)
                    {
                        AngleOffset = 360 - Direction;
                        ReletiveAngle = WorldAngle + AngleOffset;
                    }
                    else
                    {
                        AngleOffset = WorldAngle - Direction;
                        ReletiveAngle = AngleOffset;
                    }
                    ReletiveAngle /= FOV;

                    if (ReletiveAngle >= 0 && ReletiveAngle <= 1)
                    {
                        float HalfRenderHeight = (int)(180F / (Vector2.Distance(WorldPosition, new Vector2(Vert.X, Vert.Y)) / 100)) / 2;

                        FaceScreenPositions.Last().Add(new Point(
                            (int)(Screen.Dimentions.X * ReletiveAngle),
                            (Screen.Dimentions.Y / 2) + (int)(HalfRenderHeight * Vert.Z)
                            ));
                    }
                        
                }
                
            }

            return FaceScreenPositions;
        }
    }
}
