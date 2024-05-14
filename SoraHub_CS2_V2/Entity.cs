using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace SoraHub_CS2
{
    public class Entity


    {
        public Vector3 position {  get; set; }

        public Vector3 viewOffset { get; set; }

        public Vector2 position2D { get; set; }

        public Vector2 viewPosition2D {  get; set; }

        public int team {  get; set; }

        public int health { get; set; }

        public string playername { get; set; }
            
        public static string MapData { get; set; }
    }
}
