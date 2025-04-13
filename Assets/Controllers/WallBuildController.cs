using System.Collections.Generic;
using TMPro;
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
    
    //change these two values based on the game difficulty?
    public float wallCost = 10f;
    public float wallReimbursement = 5f;

    // refers to the two corners of the wall being built
    private Vector3 corner1;
    private Vector3 corner2;

    private Vector3 initialMousePosition;

    private GameObject wallBlueprint; // Reference to the wall blueprint object

    public void StartBuildOrDeleteWall(){
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

        List<GameObject> sameDirectionWalls = WallsFacingSameDirection(direction, corner2.y + 1.5f); // Get all walls facing the same direction as the new wall

        GameVectorMathUtils.MergeColinearWalls(corner1, corner2, sameDirectionWalls, out Vector3 newStart, out Vector3 newEnd, out List<GameObject> overlappingWalls); // Merge collinear walls if they overlap
        
        float totalCost = CalculateTotalWallCost(CalculateUniqueWallLength(newStart,newEnd,overlappingWalls), wallCost);
        
        if(totalCost > moneyController.CurrentMoney){
            return; // Exit the method if not enough money to build the wall
        }

        // Calculate the total cost of the wall
        moneyController.SubtractMoney(totalCost); 
        
        foreach(GameObject existingWall in overlappingWalls){
            Destroy(existingWall); // Destroy the overlapping walls
            mapController.RemoveWall(existingWall); // Remove the overlapping walls from the map controller's list of walls
        }

        // Create the new wall with the updated start and end points
        CreateWall(newStart, newEnd); // Create the new wall with the updated start and end points
        isBuildingWall = false; // Reset the flag to indicate that we are no longer building a wall
        
        //Clean up after wall is built
        // need to reset the wall blueprint to just be 1x1
        wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, 0.15f); // Reset the scale of the wall blueprint
        
        corner1Set = false;
        corner1 = Vector3.zero; // Reset the first corner to zero
        corner2 = Vector3.zero; // Reset the second corner to zero
    }

    public void FinishDeleteWall(){

        if(!corner1Set || !isWallDeleteSelected){
            return;
        }
        corner2 = SetClosestCorner(); // Get the second corner for the deleted wall
        if(corner2 == Vector3.zero){
            return; // Exit the method if no tile is hovered
        }

        Vector3 direction = GameVectorMathUtils.CalculateDirection(corner2,corner1); // Calculate the direction vector between the two corners

        List<GameObject> sameDirectionWalls = WallsFacingSameDirection(direction, corner2.y + 1.5f); // Get all walls facing the same direction as the new wall
        if(sameDirectionWalls.Count == 0){
            return; // Exit the method if no walls facing the same direction are found cant delete walls if they dont exist
        }
        GameObject wallToDelete = null;
        foreach(GameObject wall in sameDirectionWalls){
            WallController wc = wall.GetComponent<WallController>();
            if(wc != null && GameVectorMathUtils.AreColinearAndOverlap(corner1, corner2, wc.WallData.PointA, wc.WallData.PointB)){
                wallToDelete = wall; // Set the wall to delete to the one that overlaps with the new wall
                break;
            }
        }

        if(wallToDelete == null || CalculateOverlapLength(corner1, corner2, sameDirectionWalls) == 0){
            return; // Exit the method if no wall to delete is found or there is no overlap
        }

        moneyController.AddMoney(CalculateTotalWallCost(CalculateOverlapLength(corner1, corner2, sameDirectionWalls), wallReimbursement)); // Add money for deleting the wall 5f per unit length so half the cost of building a wall
        TrimWallToExcludeSegment(corner1, corner2, wallToDelete); // Calculate the new walls from the deleted wall
        
        mapController.RemoveWall(wallToDelete); 
        Destroy(wallToDelete); // Destroy the wall to delete

        isBuildingWall = false;

        wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, 0.15f); 

        corner1Set = false;
        corner1 = Vector3.zero; // Reset the first corner to zero
        corner2 = Vector3.zero; // Reset the second corner to zero
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
        if(Input.GetMouseButtonUp(0) && isWallBuildSelected && !isWallDeleteSelected){
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Ignore clicks on UI elements
            }
            else if(!corner1Set){
                StartBuildOrDeleteWall(); // Start building the wall if the first corner is not set
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
                StartBuildOrDeleteWall();
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
            float totalCost = 0f;
            List<GameObject> overlappingWalls = WallsFacingSameDirection(GameVectorMathUtils.CalculateDirection(corner1,corner2), corner2.y + 1.5f); // Get all walls facing the same direction as the new wall
            if(!isWallDeleteSelected){
                float uniqueLength = CalculateUniqueWallLength(corner1, corner2, overlappingWalls); // Calculate the length of the wall
                totalCost = CalculateTotalWallCost(uniqueLength, wallCost);
            }
            else{
                float overlapLength = CalculateOverlapLength(corner1, corner2, overlappingWalls); // Calculate the length of the wall
                totalCost = CalculateTotalWallCost(overlapLength, wallReimbursement); // Calculate the cost of the wall based on the length and cost per unit length
            }
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
    private List<GameObject> WallsFacingSameDirection(Vector3 direction, float height){
        List<GameObject> walls = new List<GameObject>();
        foreach(GameObject wall in mapController.builtWalls){
            WallController wc = wall.GetComponent<WallController>();
            if(wc != null && GameVectorMathUtils.VectorsAreParallel(wc.WallData.Direction, direction) && wc.WallData.Position.y == height){
                walls.Add(wall); // Add the wall to the list if it is facing the same direction
            }
        }
        return walls; // Return the list of walls facing the same direction
    }
    private float CalculateOverlapLength(Vector3 newStart, Vector3 newEnd, List<GameObject> existingWalls){
        
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
        return overlapLength; // Return the unique length of the new wall
    }
    private float CalculateUniqueWallLength(Vector3 newStart, Vector3 newEnd, List<GameObject> existingWalls){

        float totalLength = Vector3.Distance(newStart, newEnd); // Calculate the total length of the new wall
        float uniqueLength = Mathf.Max(0, totalLength - CalculateOverlapLength(newStart, newEnd, existingWalls)); // Calculate the unique length of the new wall
        Debug.Log($"Unique Length: {uniqueLength}"); // Log the unique length for debugging purposes
         // Calculate the unique length of the new wall
        return uniqueLength; // Return the unique length of the new wall
    }

    private void TrimWallToExcludeSegment(Vector3 cornerA, Vector3 cornerB, GameObject wallToTrim){
        if(wallToTrim == null){
            return; // Exit the method if the wall to trim is null
        }

        WallDataModel wallData = wallToTrim.GetComponent<WallController>().WallData; // Get the WallDataModel component from the wall to trim

        Vector3 start = wallData.PointA; // Get the start point of the wall to trim
        Vector3 end = wallData.PointB; // Get the end point of the wall to trim
        Vector3 wallDirection = GameVectorMathUtils.CalculateDirection(end, start); // Calculate the direction of the wall to trim

        float wallStartScalar = 0f; // start - start
        float wallEndScalar = Vector3.Dot(wallDirection, end - start);
        float deleteStartScalar = Vector3.Dot(wallDirection, cornerA - start);
        float deleteEndScalar = Vector3.Dot(wallDirection, cornerB - start);

        if(wallStartScalar > wallEndScalar)(wallStartScalar, wallEndScalar) = (wallEndScalar, wallStartScalar); // Ensure wallStartScalar is less than wallEndScalar
        if(deleteStartScalar > deleteEndScalar)(deleteStartScalar, deleteEndScalar) = (deleteEndScalar, deleteStartScalar); // Ensure deleteStartScalar is less than deleteEndScalar

        if(deleteStartScalar <= wallStartScalar && deleteEndScalar >= wallEndScalar){
            return; // No new wall is created if the wall to trim is completely within the delete segment
        }

        if(deleteStartScalar > wallStartScalar){
            Vector3 newStart = start;
            Vector3 newEnd = start + wallDirection * (deleteStartScalar - wallStartScalar); // Calculate the new end point of the wall to trim
            CreateWall(newStart, newEnd); // Create a new wall with the updated start and end points
        }

        if(deleteEndScalar < wallEndScalar){
            Vector3 newStart = start + wallDirection * (deleteEndScalar - wallStartScalar); // Calculate the new start point of the wall to trim
            Vector3 newEnd = end;
            CreateWall(newStart, newEnd); // Create a new wall with the updated start and end points
        }
    }

    private void CreateWall(Vector3 start, Vector3 end){
        if(start == Vector3.zero || end == Vector3.zero){
            return; // Exit the method if either start or end is zero
        } 
        Vector3 direction = GameVectorMathUtils.CalculateDirection(end, start); // Calculate the direction of the wall
        float wallLength = CalculateWallLength(start, end); // Calculate the length of the wall
        Vector3 wallMidpoint = (start + end) / 2; // Calculate the midpoint of the wall
        GameObject wall = Instantiate(wallPrefab, AdjustHeight(wallMidpoint), Quaternion.LookRotation(direction)); // Instantiate the wall prefab at the midpoint between the two corners
        wall.transform.localScale = new Vector3(0.15f, 3f, wallLength); // Set the scale of the wall based on its length
        wall.transform.rotation = Quaternion.LookRotation(direction); // Set the rotation of the wall to face the direction
        WallController wc = wall.AddComponent<WallController>(); // Add the WallController component to the wall
        wc.Initialise(wall.transform.localScale.z, wall.transform.position, wall.transform.rotation.eulerAngles); // Initialise the WallController with the wall's scale, position, and rotation
        mapController.AddWall(wall); // Add the wall to the map controller's list of walls
    }
}