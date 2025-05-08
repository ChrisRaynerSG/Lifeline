using UnityEngine;
using System.Collections.Generic; // Importing the System.Collections.Generic namespace for using List<T> class
public interface IMapController{

    void AddWall(GameObject wall); // Method to add a wall to the map controller's list of walls
    void RemoveWall(GameObject wall); // Method to remove a wall from the map controller's list of walls

    GameObject GetTileAtMousePosition(); // Method to get the tile at the mouse position
    void GoUpFloor();
    void GoDownFloor();
    void ToggleBuildGrid();
    

    // List<GameObject> GetWalls(); // Method to get the list of walls in the map controller
    // void ClearWalls(); // Method to clear all walls from the map controller
    // void SetMapSize(Vector3 size); // Method to set the size of the map
    // Vector3 GetMapSize(); // Method to get the size of the map

}