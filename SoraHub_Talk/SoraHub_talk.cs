using SoraHub_talk;
using System.Numerics;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

class Program
{
    static void Main()
    {
        string game = "cs2";
        string clientdll = "client.dll";

        // The Offsets we found
        int entitylistoffset = 0x1860B80;
        int xyzoffset = 0xDB8;
        int healthoffset = 0x344;


        // we'll use this library to read memory gets a handle to the game
        RW_Calls rw_calls = new RW_Calls(game);
        Console.WriteLine($"[SUCCESS] Found {game} Process!");

        // Get the base address of the client.dll module
        IntPtr client = rw_calls.GetModuleBase(clientdll);
        Console.WriteLine($"[SUCCESS] Found {clientdll}!");

        Console.WriteLine("[INFO] Initialized Offsets!");

        List<Entity> entities = new List<Entity>();

        while (true)
        {
            Console.Clear(); // Clear screen before printing all entities again
            Console.WriteLine("=========================================================");
            Console.WriteLine("                     SoraHub CS2 Talk             ");
            Console.WriteLine("=========================================================\n");

            // loop thr ough the entity list (64 entities) can be changed
            for (int entityCount = 0; entityCount < 64; entityCount++)
            {
                // finds the address of the entity by using client.dll + entitylistoffset + ( no of entities * 0x08 )
                IntPtr entAddr = rw_calls.ReadPointer(client + entitylistoffset + (entityCount * 0x08));
                if (entAddr == IntPtr.Zero)
                    continue; // Skip if the address is null

                 // create a new entity object to store individual entity data
                Entity entity = new Entity { EntityAddress = entAddr };

                // Read the entity's data from memory
                entity.Update(rw_calls , xyzoffset , healthoffset);

                // now check if the entity is alive and has a valid health value
                if (entity.health < 1 || entity.health > 100)
                    continue;

                // ensure theres no duplicates because we got both C_CSPlayer & C_CSOberserver by checking the x & y coordinates if its the same we ignore it 
                if (!entities.Any(e => e.origin.X == entity.origin.X && e.origin.Y == entity.origin.Y))
                    entities.Add(entity);

                // after which we have the list of players in the game who are alive & valid so dumping their health n positions
                Console.WriteLine($"[INFO] Health: {entity.health} Pos: {entity.origin}");
            }

            Thread.Sleep(100);
        }
    }

    public class Entity
    {
        public IntPtr EntityAddress { get; set; }
        public Vector3 origin { get; set; }
        public int health { get; set; }

        public void Update(RW_Calls rw, int xyzoffset, int healthoffset)
        // Read the position and health values from memory
        {
            origin = rw.ReadVec(EntityAddress + xyzoffset);
            health = rw.ReadInt(EntityAddress + healthoffset);
        }
    }
}
