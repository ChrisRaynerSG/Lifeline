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
    }

}