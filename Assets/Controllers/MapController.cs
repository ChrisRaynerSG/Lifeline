using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// This class is responsible for generating a map of tiles in the game.
/// It creates a grid of tiles based on the specified width and height.
/// /// </summary>
public class MapController : MonoBehaviour, IMapController
{
    public static MapController Instance { get; private set; } // Singleton instance of the MapController
    public static event Action<float> percentMapGenerated;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
    public GameObject tilePrefab; // Prefab for the tiles in the map
    public GameObject[,,] tilePrefabs;
    public List<GameObject> builtWalls = new List<GameObject>();
    public List<GameObject> rooms = new List<GameObject>(); // List to store built walls
    public GameObject buildGrid;

    public int mapWidth;
    public int mapHeight;
    public int mapDepth;
    private int currentFloor = 0;
    public int CurrentFloor => currentFloor; // Property to get the current floor
    private int maxFloor = 7;
    public int MaxFloor => maxFloor; // Property to get the maximum floor
    public static event Action<int> OnFloorChanged;
    public static event Action<GameObject> hoveredTile; // Event to notify when the floor changes

    private void Start()
    {
        tilePrefabs = new GameObject[mapWidth, mapHeight, mapDepth]; // Initialize the array to store tile prefabs
        GenerateMap(); // Call the method to generate the map at the start of the game
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.PageUp))
        {
            GoUpFloor(); // Move up a floor when the W key is pressed
        }
        if(Input.GetKeyUp(KeyCode.PageDown))
        {
            GoDownFloor(); // Move down a floor when the S key is pressed
        }
        if(Input.GetKeyUp(KeyCode.G))
        {
            ToggleBuildGrid(); // Toggle the build grid visibility when the G key is pressed
        }
        hoveredTile?.Invoke(GetTileAtMousePosition()); // Notify subscribers about the hovered tile 
    }

    private void GenerateMap()
    {
        StartCoroutine(GenerateMapCoroutine());
        SetupBuildGrid(); // Start the coroutine to generate the map
    }

    public GameObject GetTile(int x, int y, int z)
    {
        // Return the tile at the specified coordinates
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight && z >= 0 && z < mapDepth)
        {
            return tilePrefabs[x, y, z];
        }
        return null; // Return null if the coordinates are out of bounds
    }

    public GameObject GetTileAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the camera to the mouse position
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) // Check if the ray hits an object
        {
            GameObject hitObject = hit.collider.gameObject; // Get the object that was hit
            if (hitObject.CompareTag("Tile")) // Check if the object has the "Tile" tag
            {
                return hitObject; // Return the tile object
            }
        }
        return null; // Return null if no tile was hit
    }

    // method to create a tile when the map is generated
    // This method is called by the GenerateMapCoroutine to create each tile in the map
    private void CreateTile(int x, int y, int z)
    {
        // Calculate the position for each tile
        // Vector3 tilePosition = new Vector3(x+0.54f,y*3,z+0.54f); // height is multiplied by 3 to have 3m tall floors // previous version
        Vector3 tilePosition = new Vector3(x,y*3,z); // height is multiplied by 3 to have 3m tall floors

        // Instantiate the tile prefab at the calculated position
        GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
        tile.name = "Tile_" + x + "_" + y + "_" + z;
            // Set the name of the tile for easier identification

        // Add the tile to the tilePrefabs array
        tilePrefabs[x,y,z] = tile; // Store the tile in the array
        // Set the parent of the tile to this object for better organization in the hierarchy
        tile.transform.parent = transform;
        TileController tc = tile.AddComponent<TileController>();
        tc.Initialise(tilePosition); // Initialize the tile with its position

        if(y!=0){
            // Set the tile to be inactive if it's not the first layer (y=0)
            tile.SetActive(false); // Deactivate the tile if it's not the first layer
        }

    }

    private IEnumerator GenerateMapCoroutine()
    {
        int count = 0;
        int percentCount = 0;
        // Counter to keep track of the number of tiles generated
        // This coroutine is used to generate the map over time
        for(int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for(int z = 0; z < mapDepth; z++) // for one height currently, need to implement more floors later
                {
                    CreateTile(x, y, z); // Call the method to create a tile at the specified coordinates
                    count++;
                    percentCount++; // Increment the counter for each tile generated
                    if(count % 10 == 0){
                        percentMapGenerated?.Invoke((float)count / (mapWidth * mapHeight * mapDepth)); // Notify subscribers about the percentage of the map generated
                        yield return null; // Wait for the next frame to avoid freezing the game
                    }                   
                }
            }
        }
    }

    // floor movement methods
    // These methods are used to move the player up or down a floor in the map
    public void GoUpFloor(){
        if(currentFloor == maxFloor){
            return; // Prevent going above the maximum floor
        }
        int nextFloor = currentFloor + 1;
        FloorMove(nextFloor);        
        currentFloor++; // Calculate the next floor
        buildGrid.transform.position = new Vector3(buildGrid.transform.position.x, buildGrid.transform.position.y + 3, buildGrid.transform.position.z);
        OnFloorChanged?.Invoke(currentFloor); // Notify subscribers about the floor change
        // make all tiles inactive for previous floor and make current floor active
    }

    public void GoDownFloor()
    {
        if(currentFloor == 0){
            return;
        }

        int nextFloor = currentFloor - 1; // Calculate the next floor
        FloorMove(nextFloor);
        // make all tiles inactive for previous floor and make current floor active
        currentFloor--;
        buildGrid.transform.position = new Vector3(buildGrid.transform.position.x, buildGrid.transform.position.y - 3, buildGrid.transform.position.z); // Move the build grid to the new floor position
        OnFloorChanged?.Invoke(currentFloor); // Notify subscribers about the floor change
        
    }

    private void FloorMove(int nextFloor){
        for(int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for(int z = 0; z < mapDepth; z++) // for one height currently, need to implement more floors later
                {
                    // Set the tile to be inactive if it's current floor were moving from
                    if(y == currentFloor)
                    {
                        tilePrefabs[x,y,z].SetActive(false); // Deactivate the tile if it's current layer
                    }
                    else if(y == nextFloor){
                        tilePrefabs[x,y,z].SetActive(true); // activate the tile if its the next floor we are moving to
                    }
                }
            }
        }
        // Deactivate the walls on the current floor and activate the walls on the next floor
        for(int i = 0; i < builtWalls.Count; i++){
            if(builtWalls[i].transform.position.y == currentFloor*3+1.5f){
                builtWalls[i].SetActive(false); // Deactivate the wall if it's on the current layer
            }
            else if(builtWalls[i].transform.position.y == nextFloor*3+1.5f){
                builtWalls[i].SetActive(true); // Activate the wall if it's on the next layer
            }
        }
    }

    // methods for the build grid, should these be in a separate class? ie a BuildGridController?

    public void ToggleBuildGrid(){
        // Toggle the visibility of the build grid
        if(buildGrid.activeSelf){
            buildGrid.SetActive(false); // Hide the build grid if it's currently visible
        }
        else{
            buildGrid.SetActive(true); // Show the build grid if it's currently hidden
        }
    }

    public void SetupBuildGrid(){
        // Set the position of the build grid to the specified position
        float x = mapWidth / 2f - 0.5f; // Calculate the x position for the build grid
        float z = mapDepth / 2f - 0.5f; // Calculate the z position for the build grid

        buildGrid.transform.position = new Vector3(x, 0.015f, z); // Set the y position to a small value to avoid clipping with the ground
        buildGrid.transform.localScale = new Vector3((mapWidth/10)+0.016f, 1, (mapDepth/10)+0.016f); 
        // Set the scale of the build grid to match the map size 
        // add 0.016f to the scale to make it slightly larger than the map size 
        // to show the grid at the top and right of the map
    }

    public void AddWall(GameObject wall){
        // Add a wall to the list of built walls
        builtWalls.Add(wall); // Add the wall to the list of built walls
    }
    public void RemoveWall(GameObject wall){
        // Remove a wall from the list of built walls
        builtWalls.Remove(wall); // Remove the wall from the list of built walls
        Destroy(wall); // Destroy the wall object
    }

    public List<GameObject> GetWalls(){
        // Return the list of built walls
        return builtWalls; // Return the list of built walls
    }
}