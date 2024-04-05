using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ClickableTransparentOverlay;
using ImGuiNET;


namespace SoraHub_CS2
{
    public class Renderer : Overlay
    {



        // screensizes
        public Vector2 screenSize = new Vector2(1920, 1080);

        // Entities Copy , using more thread safe methods than v1 where u pop a thread n kill
        public ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localplayer = new Entity();
        private readonly object entityLock = new object();

        // DrawList

        ImDrawListPtr drawList;

        // Gui Elements 

        private bool enableESP = true;
        private bool enableName = true;
        private bool enableBox = true;
        private bool enableLine = true;
        private bool enableHealth = true;

        private Vector4 enemyColor = new Vector4(1, 0, 0, 1); // RGBA Enemy
        private Vector4 teamColor = new Vector4(0, 1, 0, 1); // RGBA Team
        private Vector4 nameColor = new Vector4(0, 0, 0, 1); // RGBA White Text




        //protected renderer
        protected override void Render()
        {
            // ImGui Menu
            ImGui.Begin("SoraHub CS2 V1.3_Beta_Edition");
            if (ImGui.BeginTabBar("Tabs"))
            {
                if (ImGui.BeginTabItem("Walls"))
                {
                    ImGui.Checkbox("Enable ESP", ref enableESP);
                    if (ImGui.CollapsingHeader("ESP Settings"))
                    {
                        ImGui.Checkbox("Show Name", ref enableName);
                        ImGui.Checkbox("Show Box", ref enableBox);
                        ImGui.Checkbox("Show Line", ref enableLine);
                        ImGui.Checkbox("Show Health", ref enableHealth);

                        if (ImGui.CollapsingHeader("Team Color"))
                            ImGui.ColorPicker4("Team Color", ref teamColor);

                        if (ImGui.CollapsingHeader("Enemy Color"))
                            ImGui.ColorPicker4("Enemy Color", ref enemyColor);

                        if (ImGui.CollapsingHeader("Name Color"))
                            ImGui.ColorPicker4("Name Color", ref nameColor);
                    }




                    ImGui.EndTabItem();
                }
            }


            //draw overlay

            DrawOverlay(screenSize);
            drawList = ImGui.GetWindowDrawList();

            if (enableESP)
            {
                foreach(var entity in entities)
                {
                    // check if entity on screen
                    if(EntityOnScreen(entity))
                    {
                        DrawHealthBar(entity);
                        DrawName(entity , 15); // entity , offsets
                        DrawBox(entity);
                        DrawLine(entity);
                    }
                }
            }

        }

        // check if inside screen

        bool EntityOnScreen(Entity entity)
        {
            if (entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y)
            {
                return true;
            }
            return false;
        }

        // Drawing Methods

        private void DrawName(Entity entity , int yOffset) // broken

        {
            if (enableName)
            {
                Vector2 textLocation = new Vector2(entity.viewPosition2D.X - 10, entity.viewPosition2D.Y - yOffset);

                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), $"{entity.playername}");
            }
        }
        private void DrawHealthBar(Entity entity)
        {
            if (enableHealth)
            {
                // calculate bar height
                float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y; // get box location
                float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
                float boxRight = entity.position2D.X + entityHeight / 3;

                // calculate health bar width
                float barPercentWidth = 0.05f; // 5% or 1/20 of box width
                float barPixelWidth = barPercentWidth * (boxRight - boxLeft);

                // calculate bar height after health
                float barHeight = entityHeight * (entity.health / 100f);

                // calculate bar rectangle, two vectors
                Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - barHeight);
                Vector2 barBottom = new Vector2(boxLeft, entity.position2D.Y);

                // get bar color (green)
                Vector4 barColor = new Vector4(0, 1, 0, 1);

                // draw health bar
                drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));
            }
        }


        private void DrawBox(Entity entity)
        {
            if (enableBox)
            {
                //Calculate Box Height
                float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

                Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);

                Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

                // getting correct color
                Vector4 boxColor = localplayer.team == entity.team ? teamColor : enemyColor;

                drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));
            }
        }

        private void DrawLine(Entity entity) // Line to Boxes
        {
            if (enableLine)
            {
                Vector4 lineColor = localplayer.team == entity.team ? teamColor : enemyColor;

                //Draw line
                drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
            }

        }


        // transfer entity methods

        public void UpdateEntities(IEnumerable<Entity> newEntities) // updated Entities
        {
            entities = new ConcurrentQueue<Entity>(newEntities);

        }

        public void UpdateLocalPlayer(Entity newEntity) // update localplayer
        {
            lock (entityLock)
            {
                localplayer = newEntity;
            }
        }

        public Entity GetLocalPlayer()
        {
            lock(entityLock)
            {
                return localplayer;
            }
        }


        void DrawOverlay(Vector2 screenSize) // draw a new window over game , or 
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );
        }


    }
}
