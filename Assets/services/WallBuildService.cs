using UnityEngine;

public class WallBuildService : MonoBehaviour, IWallBuilder{

    IMapController mapController;
    IMoneyController moneyController;
    GameObject wallPrefab;

    public WallBuildService(MapController mapController, MoneyController moneyController, GameObject wallPrefab){
        this.mapController = mapController;
        this.moneyController = moneyController;
        this.wallPrefab = wallPrefab;
    }

    public void CreateWall(Vector3 start, Vector3 end){
        if(start == Vector3.zero || end == Vector3.zero){
            return; // Exit the method if either start or end is zero
        } 
        Vector3 direction = GameVectorMathUtils.CalculateDirection(end, start); // Calculate the direction of the wall
        float wallLength = WallMathUtils.CalculateWallLength(start, end); // Calculate the length of the wall

        Vector3 wallMidpoint = (start + end) / 2; // Calculate the midpoint of the wall
        GameObject wall = Instantiate(wallPrefab, WallMathUtils.AdjustWallHeight(wallMidpoint), Quaternion.LookRotation(direction)); // Instantiate the wall prefab at the midpoint between the two corners
        
        wall.transform.localScale = new Vector3(0.15f, 3f, wallLength); // Set the scale of the wall based on its length
        wall.transform.rotation = Quaternion.LookRotation(direction); // Set the rotation of the wall to face the direction
        
        WallController wc = wall.AddComponent<WallController>(); // Add the WallController component to the wall
        wc.Initialise(wall.transform.localScale.z, wall.transform.position, wall.transform.rotation.eulerAngles); // Initialise the WallController with the wall's scale, position, and rotation
        mapController.AddWall(wall); // Add the wall to the map controller's list of walls
    }

    
}