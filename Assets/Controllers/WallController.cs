using UnityEngine;

// Component to be added to the wall prefab to manage its behavior and interactions in the game.
public class WallController : MonoBehaviour{

    private WallDataModel wallData;
    public WallDataModel WallData
    {
        get { return wallData; } // Property to get the wall data
        set { wallData = value; } // Property to set the wall data
    }

    // public void Awake()
    // {
    //     wallData = new WallDataModel(0); // Initialize the wall data with a default length of 0
    // }

    public void Initialise(float length, Vector3 position, Vector3 rotation){
        wallData = new WallDataModel(length, position, rotation); // Initialize the wall data with the given parameters
        wallData.length = length; // Set the length of the wall
        Debug.Log("WallController: Initialised wall data with length: " + length); // Log the initialization of the wall data
        Debug.Log("WallController: Initialised wall data with position: " + position); // Log the initialization of the wall data
        Debug.Log("WallController: Initialised wall data with rotation: " + rotation); // Log the initialization of the wall data
        Debug.Log("WallController: Initialised wall data with pointA: " + wallData.PointA); // Log the initialization of the wall data
        Debug.Log("WallController: Initialised wall data with pointB: " + wallData.PointB); // Log the initialization of the wall data
        Debug.Log("WallController: Initialised wall data with direction: " + wallData.Direction); // Log the initialization of the wall data
    }

}