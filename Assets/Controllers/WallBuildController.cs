using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;


public class WallBuildController : MonoBehaviour
{

    [SerializeField] private MapController mapController; // Reference to the MapController script
    [SerializeField] private MoneyController moneyController;
    // Reference to the MoneyController script
    public GameObject wallPrefab; // Prefab for the wall
    public GameObject wallBlueprintPrefab; // Prefab for the wall blueprint
    public TextMeshProUGUI wallCostUiElement; // UI element to display the wall cost will calculate the cost of the wall based on the size of the wall and will update the UI element with the cost in real-time
    private bool isBuildingWall = false;
    private bool isWallBuildSelected = false;
    private bool isWallDeleteSelected = false; // Flag to check if wall delete mode is selected without holding control
    private bool isWallDeleteNonControlSelected = false; 
    private bool corner1Set = false; // flag to check if the first corner is set so we can display the wall blueprint
    private Vector3 corner1;
    private Vector3 corner2;

    private Vector3 initialMousePosition;

    private GameObject wallBlueprint; // Reference to the wall blueprint object

    public void StartBuildWall(){
        if(!isWallBuildSelected){
            return; // Exit the method if wall build mode is not selected
        }
        corner1 = SetClosestCorner();
        if(corner1 == Vector3.zero){
            return; // Exit the method if no tile is hovered
        }
        isBuildingWall = true; // Set the flag to indicate that we are building a wall
        corner1Set = true; // Set the flag to indicate that the first corner is set
        wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, 0.15f);
        wallBlueprint.transform.position = AdjustHeight(corner1); // Set the position of the wall blueprint to the first corner
        
        wallBlueprint.GetComponent<MeshRenderer>().material.color = Color.blue; // Set the scale of the wall blueprint
        // moneyController.SubtractMoney(CalculateTotalWallCost(CalculateWallLength(corner1,corner2),10f)); // Subtract money for building the wall
    }
    public void FinishBuildWall(){
        if(!isBuildingWall){
            return; // Exit the method if we are not building a wall
        }
        corner2 = SetClosestCorner(); // Get the second corner for the wall
        if(corner2 == Vector3.zero){
            return; // Exit the method if no tile is hovered
        }

        Vector3 direction = GameVectorMathUtils.CalculateDirection(corner2,corner1); // Calculate the direction vector between the two corners
        float wallLength = CalculateWallLength(corner1, corner2); // Calculate the length of the wall
        
        List<GameObject> sameDirectionWalls = WallsFacingSameDirection(direction); // Get all walls facing the same direction as the new wall
        
        GameVectorMathUtils.MergeColinearWalls(corner1, corner2, sameDirectionWalls, out Vector3 newStart, out Vector3 newEnd, out List<GameObject> overlappingWalls); // Merge collinear walls if they overlap
        foreach(GameObject existingWall in overlappingWalls){
            Destroy(existingWall); // Destroy the overlapping walls
            mapController.RemoveWall(existingWall); // Remove the overlapping walls from the map controller's list of walls
        }

        float totalCost = CalculateTotalWallCost(CalculateUniqueWallLength(newStart,newEnd,overlappingWalls), 10f);
        
        if(totalCost > moneyController.CurrentMoney){
            return; // Exit the method if not enough money to build the wall
        }

        // Calculate the total cost of the wall
        moneyController.SubtractMoney(totalCost); 
    
        Vector3 mergedDirection = GameVectorMathUtils.CalculateDirection(newEnd, newStart);
        float mergedLength = CalculateWallLength(newStart, newEnd); // Calculate the length of the merged wall
        Vector3 mergedMidpoint = (newStart + newEnd) / 2; // Calculate the midpoint of the merged wall

        // Create the new wall with the updated start and end points
        GameObject wall = Instantiate(wallPrefab, AdjustHeight(mergedMidpoint), Quaternion.LookRotation(mergedDirection)); // Instantiate the wall prefab at the midpoint between the two corners
        wall.transform.localScale = new Vector3(0.15f, 3f, mergedLength); // Set the scale of the wall based on its length
        wall.transform.rotation = Quaternion.LookRotation(mergedDirection);
        WallController wc = wall.AddComponent<WallController>();
        wc.Initialise(wall.transform.localScale.z, wall.transform.position, wall.transform.rotation.eulerAngles);

        mapController.AddWall(wall); // Add the wall to the map controller's list of walls
        isBuildingWall = false; // Reset the flag to indicate that we are no longer building a wall
        
        //Clean up after wall is built
        // need to reset the wall blueprint to just be 1x1
        wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, 0.15f); // Reset the scale of the wall blueprint
        
        corner1Set = false;
        corner1 = Vector3.zero; // Reset the first corner to zero
        corner2 = Vector3.zero; // Reset the second corner to zero
    }

    public void StartDeleteWall(){

    }

    public void FinishDeleteWall(){

    }

    private Vector3 SetClosestCorner()
    {
        GameObject tile = mapController.GetTileAtMousePosition();

        if(tile == null){
            return Vector3.zero; // If no tile is hovered, exit the method
        }

        TileDataModel tileData = tile.GetComponent<TileController>().TileData; // Get the TileDataModel component from the hovered tile
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get the mouse position in world coordinates
        float closestDistance = float.MaxValue;
        Vector3 closestCorner = Vector3.zero; // Initialize the closest corner to zero

        foreach(Vector3 corner in tileData.corners) // Loop through the corners of the tile
        {
            float distance = Vector3.Distance(mousePos, corner); // Calculate the distance between the mouse position and the corner
            if (distance < closestDistance) // Check if this corner is closer than the previous closest corner
            {
                closestDistance = distance; // Update the closest distance
                closestCorner = corner; // Update the closest corner
            }
        }
        return closestCorner;
    }
    public void Update()
    {

        if(isWallBuildSelected && wallBlueprint == null){
            SetupWallBlueprint(); // Instantiate the wall blueprint prefab if it is null
        }
        
        wallCostUiElement.text = ""; // Clear the wall cost UI element

        // mouse commands
        if(Input.GetMouseButtonDown(1)){
            initialMousePosition = Input.mousePosition; // Store the initial mouse position when the right mouse button is clicked
        }
        //Cancel wall build mode when the right mouse button is clicked
        if(Input.GetMouseButtonUp(1) && isWallBuildSelected && !isBuildingWall){
            // only toggle build mode if camera does not move
            // if the camera is moving, we don't want to toggle build mode
            // this is to prevent the player from accidentally toggling build mode while moving the camera
            // if the camera is not moving, we want to toggle build mode
            Vector3 currentMousePosition = Input.mousePosition; // Get the current mouse position
            Vector3 mouseDelta = currentMousePosition - initialMousePosition; // Calculate the difference between the current and initial mouse positions
            if (mouseDelta.magnitude < 10f) // Check if the mouse has moved less than 10 pixels
            {
                ToggleWallBuildMode(); // Toggle the wall build mode
            }
        }
        if(Input.GetMouseButtonUp(1) && isBuildingWall){
            Vector3 currentMousePosition = Input.mousePosition; // Get the current mouse position
            Vector3 mouseDelta = currentMousePosition - initialMousePosition; // Calculate the difference between the current and initial mouse positions
            if (mouseDelta.magnitude < 10f){
                isBuildingWall = false; // Reset the flag to indicate that we are no longer building a wall
                corner1Set = false; // Reset the corner set flag
                corner1 = Vector3.zero; // Reset the first corner to zero
                Destroy(wallBlueprint); // Destroy the wall blueprint object
            }
        }

        if(isWallBuildSelected){
            if(Input.GetKeyDown(KeyCode.LeftControl)){
                // change wall build mode to delete mode
                if(!isWallDeleteSelected){
                    isWallDeleteSelected = true; // Set the flag to indicate that we are in wall delete mode
                    isWallBuildSelected = false; // Reset the wall build mode flag
                    wallBlueprint.GetComponent<MeshRenderer>().material.color = Color.red; // Change the color of the wall blueprint to red
                }
            }
        }

        if(isWallDeleteSelected){
            if(Input.GetKeyUp(KeyCode.LeftControl)){
                // change wall build mode to build mode upon key release
                isWallDeleteSelected = false; // Reset the wall delete mode flag
                isWallBuildSelected = true; // Set the flag to indicate that we are in wall build mode
                wallBlueprint.GetComponent<MeshRenderer>().material.color = Color.blue; // Change the color of the wall blueprint to blue
            }
        }
        
        // build a wall when the left mouse button is clicked
        if(Input.GetMouseButtonUp(0) && isWallBuildSelected){
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Ignore clicks on UI elements
            }
            else if(!corner1Set){
                StartBuildWall(); // Start building the wall if the first corner is not set
            }else{
                FinishBuildWall(); // Finish building the wall if the first corner is set
            }
        }

        if(Input.GetMouseButtonUp(0) && (isWallDeleteSelected || isWallDeleteNonControlSelected)){
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Ignore clicks on UI elements
            }
            else if(!corner1Set){
                StartDeleteWall();
            }
            else{
                FinishDeleteWall();
            }
            
            
        }

        // update wall blueprint position to follow mouse before wall is started to be built or deleted
        if(!corner1Set && (isWallBuildSelected || isWallDeleteSelected)){
            wallBlueprint.transform.position = AdjustHeight(SetClosestCorner()); // Set the position of the wall blueprint to the hovered tile    
             // Set the scale of the wall blueprint
        }

        // display wall blueprint while dragging the wall, as well as updating the cost of the wall in the ui
        if(corner1Set && isBuildingWall){
            corner2 = SetClosestCorner(); // Get the second corner for the wall
            if(corner2 == Vector3.zero){
                return; // Exit the method if no tile is hovered
            }
            float wallLength = CalculateWallLength(corner1, corner2);
            List<GameObject> overlappingWalls = WallsFacingSameDirection(GameVectorMathUtils.CalculateDirection(corner1,corner2));
            float uniqueLength = CalculateUniqueWallLength(corner1, corner2, overlappingWalls); // Calculate the length of the wall
            float totalCost = CalculateTotalWallCost(uniqueLength, 10f);
            UpdateBlueprintPosition(wallLength); // Calculate the total cost of the wall
            wallCostUiElement.text = totalCost.ToString("F1"); // Update the UI element with the cost of the wall
        }
    }

    private float CalculateTotalWallCost(float wallLength, float wallPerUnitCost)
    { 
        return wallLength * wallPerUnitCost; // Calculate and return the cost of the wall based on the length and cost per unit length
    }

    private float CalculateWallLength(Vector3 corner1, Vector3 corner2)
    {
        return Vector3.Distance(corner1, corner2); // Calculate and return the length of the wall based on the two corners
    }

    private void UpdateBlueprintPosition(float wallLength)
    {
        if (wallBlueprint != null && corner1Set && isBuildingWall)
        {
            // Update the position, rotation, and scale of the blueprint
            Vector3 direction = GameVectorMathUtils.CalculateDirection(corner2 , corner1);
            wallBlueprint.transform.position = AdjustHeight((corner1 + corner2) / 2);
            wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, wallLength);
            wallBlueprint.transform.rotation = Quaternion.LookRotation(direction);
        }
        else if (wallBlueprint != null)
        {
            wallBlueprint.transform.position = AdjustHeight(SetClosestCorner());
        }
    }

    public void ToggleWallBuildMode(){

        isWallBuildSelected = !isWallBuildSelected; // Toggle the wall build mode on or off
        if(isWallBuildSelected){
            corner1Set = false; // Reset the corner set flag when entering wall build mode
            SetupWallBlueprint(); // Instantiate the wall blueprint prefab
        }
        if(!isWallBuildSelected && wallBlueprint != null){
            Destroy(wallBlueprint); // Destroy the wall blueprint object when exiting wall build mode
            corner1Set = false; // Reset the corner set flag when exiting wall build mode
        }
    }

    public void ToggleWallDeleteMode(){
        isWallBuildSelected = false;

    }

    private void SetupWallBlueprint(){
        if(wallBlueprint == null){
            wallBlueprint = Instantiate(wallBlueprintPrefab); // Instantiate the wall blueprint prefab
            wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, 0.15f); // Set the scale of the wall blueprint
            wallBlueprint.transform.position = Vector3.zero; // Set the position of the wall blueprint to zero
            wallBlueprint.GetComponent<MeshRenderer>().material.color = Color.blue; // Set the color of the wall blueprint to blue
        }
        wallBlueprint.transform.position = SetClosestCorner();
    }

    private Vector3 AdjustHeight(Vector3 position){
        Vector3 heightAdjust = position;
        heightAdjust.y += 1.5f; // Adjust the Y-coordinate to raise the wall
        return heightAdjust;
    }

    //maybe these should be in a VectorMath utility class? 

    // Get all walls facing the same direction as the given direction so we can delete them if they intersect with the new wall, maybe even use this to merge walls
    private List<GameObject> WallsFacingSameDirection(Vector3 direction){
        List<GameObject> walls = new List<GameObject>();
        foreach(GameObject wall in mapController.builtWalls){
            WallController wc = wall.GetComponent<WallController>();
            if(wc != null && GameVectorMathUtils.VectorsAreParallel(wc.WallData.Direction, direction)){
                walls.Add(wall); // Add the wall to the list if it is facing the same direction
            }
        }
        return walls; // Return the list of walls facing the same direction
    }

    private float CalculateUniqueWallLength(Vector3 newStart, Vector3 newEnd, List<GameObject> existingWalls){

        float totalLength = Vector3.Distance(newStart, newEnd); // Calculate the total length of the new wall
        float overlapLength = 0f; // Initialize the overlap length to zero
        Vector3 wallDirection = GameVectorMathUtils.CalculateDirection(newEnd, newStart); // Calculate the direction of the new wall

        foreach(GameObject existingWall in existingWalls){
            WallController wc = existingWall.GetComponent<WallController>();
            Vector3 start = wc.WallData.PointA;
            Vector3 end = wc.WallData.PointB;
            if(GameVectorMathUtils.AreColinearAndOverlap(newStart, newEnd, start, end)){
                float aMin = Vector3.Dot(wallDirection, newStart);
                float aMax = Vector3.Dot(wallDirection, newEnd);
                float bMin = Vector3.Dot(wallDirection, start);
                float bMax = Vector3.Dot(wallDirection, end);
                
                if(aMin > aMax)(aMin, aMax) = (aMax, aMin); // Ensure aMin is less than aMax
                if(bMin > bMax)(bMin, bMax) = (bMax, bMin); // Ensure bMin is less than bMax

                float overlapMin = Mathf.Max(aMin, bMin); // Calculate the minimum overlap point
                float overlapMax = Mathf.Min(aMax, bMax); // Calculate the maximum overlap point
                float overlap = Mathf.Max(0, overlapMax - overlapMin); // Calculate the overlap length
                overlapLength += overlap; // Add the overlap length to the total overlap length
            }
        }
        float uniqueLength = Mathf.Max(0, totalLength - overlapLength); // Calculate the unique length of the new wall
        return uniqueLength; // Return the unique length of the new wall
    }

    // need methods to check for overlap between walls and merge the walls together if they do



}