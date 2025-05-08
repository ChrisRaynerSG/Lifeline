using System.Collections.Generic;
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

    public void SplitIntersectingWalls(){

        if(mapController.GetWalls().Count < 2){
            return; // Exit the method if there are less than two walls to check for intersections
        }
        
        List<GameObject> walls = mapController.GetWalls(); // Get the list of walls from the map controller

        for(int i = 0; i < walls.Count; i++){
            for(int j = i + 1; j < walls.Count; j++){
                WallController wallA = mapController.GetWalls()[i].GetComponent<WallController>();
                WallController wallB = mapController.GetWalls()[j].GetComponent<WallController>();

                if (wallA == null || wallB == null) continue; // Skip if either wall is null
                if (wallA == wallB) continue; // Skip if both walls are the same
                
                if(wallA.IsIntersecting(wallB)){
                    Debug.Log("Splitting walls: " + wallA.gameObject.name + " and " + wallB.gameObject.name); // Log the names of the walls being split
                    SplitWall(wallA, wallB); // Call the method to split the intersecting walls
                }
            }
        }
    }

    private void SplitWall(WallController wallA, WallController wallB){

        if(wallA.WallData.PointA == wallB.WallData.PointA || wallA.WallData.PointA == wallB.WallData.PointB){
            return; // Exit the method if the start point of wall A is equal to either end point of wall B
        }
        if(wallA.WallData.PointB == wallB.WallData.PointA || wallA.WallData.PointB == wallB.WallData.PointB){
            return; // Exit the method if the end point of wall A is equal to either end point of wall B
        }
        // need to find the intersection point of the two walls
        Vector3 intersectionPoint = WallMathUtils.CalculateIntersectionPoint(wallA, wallB); // Calculate the intersection point of the two walls
        intersectionPoint.y = intersectionPoint.y - 1.5f; // Adjust the Y-coordinate of the intersection point

        if(!WallMathUtils.IsPointBetween(intersectionPoint, wallA.WallData.PointA, wallA.WallData.PointB))
        {
            return; // Exit the method if the intersection point is not between the endpoints of either wall
        }
        if(!WallMathUtils.IsPointBetween(intersectionPoint, wallB.WallData.PointA, wallB.WallData.PointB))
        {
            return; // Exit the method if the intersection point is not between the endpoints of either wall
        }

        Debug.Log("Intersection point: " + intersectionPoint); // Log the intersection point
        if(intersectionPoint == Vector3.zero){
            return; // Exit the method if the intersection point is zero
        }

        if(intersectionPoint.x == wallA.WallData.PointA.x){
            return; // Exit the method if the intersection point is equal to the start point of wall A
        }
        // Create new walls from the intersection point to the endpoints of the original walls
        Vector3 wallAStart = wallA.WallData.PointA - new Vector3(0,1.5f,0);
        Vector3 wallAEnd = wallA.WallData.PointB - new Vector3(0,1.5f,0);
        Vector3 wallBStart = wallB.WallData.PointA - new Vector3(0,1.5f,0);
        Vector3 wallBEnd = wallB.WallData.PointB - new Vector3(0,1.5f,0);
        
        CreateWall(wallAStart, intersectionPoint); // Create a new wall from the start of wall A to the intersection point
        CreateWall(intersectionPoint, wallAEnd); // Create a new wall from the intersection point to the end of wall A
        CreateWall(wallBStart, intersectionPoint); // Create a new wall from the start of wall B to the intersection point
        CreateWall(intersectionPoint, wallBEnd); // Create a new wall from the intersection point to the end of wall B
        
        // Remove the original walls from the map controller's list of walls
        mapController.RemoveWall(wallA.gameObject); // Remove wall A from the map controller
        mapController.RemoveWall(wallB.gameObject); // Remove wall B from the map controller
        
    }
}