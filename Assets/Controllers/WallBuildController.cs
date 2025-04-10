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
        wallBlueprint.transform.position = corner1; // Set the position of the wall blueprint to the first corner
        Vector3 raisedPosition = wallBlueprint.transform.position;
        raisedPosition.y += 1.5f; // Adjust the Y-coordinate to raise the wall
        wallBlueprint.transform.position = raisedPosition;
        
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

        Vector3 direction = corner2 - corner1; // Calculate the direction vector between the two corners
        float wallLength = CalculateWallLength(corner1, corner2); // Calculate the length of the wall
        float totalCost = CalculateTotalWallCost(wallLength, 10f); // Calculate the total cost of the wall
        moneyController.SubtractMoney(totalCost); // Subtract money for building the wall
        GameObject wall = Instantiate(wallPrefab, (corner1 + corner2) / 2, Quaternion.identity); // Instantiate the wall prefab at the midpoint between the two corners
        wall.transform.localScale = new Vector3(0.15f, 3f, wallLength); // Set the scale of the wall based on its length
        wall.transform.rotation = Quaternion.LookRotation(direction);

        Vector3 raisedPosition = wall.transform.position;
        raisedPosition.y += 1.5f; // Adjust the Y-coordinate to raise the wall
        wall.transform.position = raisedPosition;

        WallController wc = wall.AddComponent<WallController>();
        wc.Initialise(wall.transform.localScale.z, wall.transform.position, wall.transform.rotation.eulerAngles); // Initialize the WallController with the wall's length, position, and rotation

        mapController.AddWall(wall); // Add the wall to the map controller's list of walls

        isBuildingWall = false; // Reset the flag to indicate that we are no longer building a wall
        
        //Clean up after wall is built
        // need to reset the wall blueprint to just be 1x1
        wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, 0.15f); // Reset the scale of the wall blueprint
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

        if(!corner1Set && isWallBuildSelected){
            wallBlueprint.transform.position = SetClosestCorner(); // Set the position of the wall blueprint to the hovered tile    
             // Set the scale of the wall blueprint
        }

        // display wall blueprint while dragging the wall, as well as updating the cost of the wall in the ui
        if(corner1Set && isBuildingWall){
            corner2 = SetClosestCorner(); // Get the second corner for the wall
            if(corner2 == Vector3.zero){
                return; // Exit the method if no tile is hovered
            }
            float wallLength = CalculateWallLength(corner1, corner2); // Calculate the length of the wall
            float totalCost = CalculateTotalWallCost(wallLength, 10f);
            UpdateBlueprintPosition(wallLength); // Calculate the total cost of the wall
            wallCostUiElement.text = totalCost.ToString(); // Update the UI element with the cost of the wall
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
            Vector3 direction = corner2 - corner1;
            wallBlueprint.transform.position = (corner1 + corner2) / 2;
            wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, wallLength);
            wallBlueprint.transform.rotation = Quaternion.LookRotation(direction);
        }
        else if (wallBlueprint != null)
        {
            wallBlueprint.transform.position = SetClosestCorner(); // Set the position of the wall blueprint to the first corner
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

    private void SetupWallBlueprint(){
        if(wallBlueprint == null){
            wallBlueprint = Instantiate(wallBlueprintPrefab); // Instantiate the wall blueprint prefab
            wallBlueprint.transform.localScale = new Vector3(0.15f, 3f, 0.15f); // Set the scale of the wall blueprint
            wallBlueprint.transform.position = Vector3.zero; // Set the position of the wall blueprint to zero
            wallBlueprint.GetComponent<MeshRenderer>().material.color = Color.blue; // Set the color of the wall blueprint to blue
        }
        wallBlueprint.transform.position = SetClosestCorner();
    }
}