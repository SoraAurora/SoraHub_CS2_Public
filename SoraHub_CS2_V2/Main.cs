using SoraHub_CS2;
using System.Numerics;


string game = "cs2";
string clientdll = "client.dll";


RW_Calls rw_calls = new RW_Calls(game);
Console.WriteLine($"[SUCCESS] Found {game} Proccess!");
 
IntPtr client = rw_calls.GetModuleBase(clientdll);
Console.WriteLine($"[SUCESS] Found {clientdll} !");



Renderer renderer = new Renderer();

Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
Console.WriteLine("[INFO] Initalizing Renderer!");
renderThread.Start();

Vector2 screenSize = renderer.screenSize;

List<Entity> entities = new List<Entity>();
Entity localplayer = new Entity();

// Offsets - changes  whenever the game updates -  https://github.com/a2x/cs2-dumper/blob/main/output/win/offsets.cs
int dwEntityList = 0x18C1DB8;
int dwViewMatrix = 0x19231B0;
int dwLocalPlayerPawn = 0x17361E8;
Console.WriteLine("[INFO] Initalized Offets.cs!");

// Client.dll.cs
int m_vOldOrigin = 0x127C;
int m_iTeamNum = 0x3CB;
int m_lifeState = 0x338;
int m_hPlayerPawn = 0x7E4;
int m_vecViewOffset = 0xC58;
int m_iHealth = 0x334;
int m_iszPlayerName = 0x638;
Console.WriteLine("[INFO] Initalized Client.dll.cs!");
// ESP Loop

Console.WriteLine("[INFO] Started Reading Memory...");
while (true)
{
    // Wipe Lists then repopulate it like how i populate jess 
    entities.Clear();

    // Get Entity List
    IntPtr entityList = rw_calls.ReadPointer(client , dwEntityList);

    //make entry
    IntPtr listEntry = rw_calls.ReadPointer(entityList, 0x10);

    // local player 

    IntPtr localPlayerPawn = rw_calls.ReadPointer(client, dwLocalPlayerPawn);

    //local player team

    localplayer.team = rw_calls.ReadInt(localPlayerPawn, m_iTeamNum);

    // loop thr entity lists
    for (int i = 0; i < 65; i++)
    {
        // get current Controller
        IntPtr currentController = rw_calls.ReadPointer(listEntry, i * 0x78);

        if (currentController == IntPtr.Zero) continue;

        //get pawn handler

        int pawnHandle = rw_calls.ReadInt(currentController, m_hPlayerPawn);

        if (pawnHandle == 0) continue;

        // Get current Pawn (playerentity) & make second entry

        IntPtr listEntry2 = rw_calls.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
        if (listEntry2 == IntPtr.Zero) continue;

        // Get current Pawn
        IntPtr currentPawn = rw_calls.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == IntPtr.Zero) continue;

        //check if alive using lifestate

        int lifeState = rw_calls.ReadInt(currentPawn, m_lifeState);
        if (lifeState != 256) continue;


        // get matrix fking cry ptsd from v1.0
        float[] viewMatrix = rw_calls.ReadMatrix(client + dwViewMatrix);

        Entity entity = new Entity();

        // entity lists 
        entity.playername = rw_calls.ReadString(currentController, m_iszPlayerName, 16).Split("\0")[0]; // read first 16 characters of player

        entity.team = rw_calls.ReadInt(currentPawn, m_iTeamNum);
        entity.health = rw_calls.ReadInt(currentPawn, m_iHealth);
        entity.position = rw_calls.ReadVec(currentPawn, m_vOldOrigin);
        entity.viewOffset = rw_calls.ReadVec(currentPawn, m_vecViewOffset);
        entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);
        entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);

        entities.Add(entity);

    }
    // Update renderer data
    renderer.UpdateLocalPlayer(localplayer);
    renderer.UpdateEntities(entities);

    Thread.Sleep(20);


}
