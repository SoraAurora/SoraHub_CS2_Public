using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;



namespace SoraHub_CS2
{
public class Radar
{
    public ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
    private string map;
    Entity entity = new Entity();
    private Entity localplayer = new Entity();
    private List<int> teamlist = new List<int>();
    private List<Vector3> positionlist = new List<Vector3>();
    private List<float[]> positionArray = new List<float[]>();
    private List<string> nameArray = new List<string>();

    private static System.Timers.Timer? radarTimer;



    public Radar()
    {
        radarTimer = new System.Timers.Timer(500); // 1000 milliseconds = 1 second
        radarTimer.AutoReset = true;
        radarTimer.Elapsed += async (sender, e) =>
        {
            teamlist.Clear();
            positionlist.Clear();
            positionArray.Clear();
            nameArray.Clear();
            await ProcessEntitiesAsync();
        };
        radarTimer.Start();
    }

    public async Task ProcessEntitiesAsync()
    {
        // Clear lists before processing new entities



        foreach (var entity in entities)
        {
            teamlist.Add(entity.team);
            positionlist.Add(entity.position);
            
        }

        if (teamlist.Count == 0 || positionlist.Count == 0)
        {
            teamlist.Clear();
            positionlist.Clear();
            positionArray.Clear();

            return; // Skip sending JSON data
        }

        foreach (Vector3 vector in positionlist)
        {
            positionArray.Add(new float[] { vector.X, vector.Y });
        }

        int[] teamArray = teamlist.ToArray();

        string jsonData = JsonConvert.SerializeObject(new
        {
            map = map,
            team_num = teamlist,
            position = positionArray
        });



        //Console.WriteLine(jsonData);
        // Ip address of server
        string ipAddress = "0.0.0.0"; 
        int port = 80;

        await SendJsonData(ipAddress, port, jsonData);
    }
    
    private static async Task SendJsonData(string ipAddress, int port, string jsonData)
    {
        try
        {
            // Create a HttpClient instance
            using (var httpClient = new HttpClient())
            {
                // Define the URL to which you want to send the JSON data
                string url = $"http://{ipAddress}:{port}/playerinfo"; // adjust the endpoint as per your server setup

                // Prepare HttpContent with JSON data
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // JSON data sent successfully
                    //Console.WriteLine("JSON data sent successfully.");
                }
                else
                {
                    // Error occurred while sending JSON data
                    //Console.WriteLine($"Error sending JSON data. Status code: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public void UpdateEntities(IEnumerable<Entity> newEntities , string newMap)
    {
        entities = new ConcurrentQueue<Entity>(newEntities);
        map = newMap;
    }



}
}
