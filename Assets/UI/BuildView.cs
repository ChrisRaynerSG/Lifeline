using UnityEngine;
using TMPro;

public class BuildView : MonoBehaviour {

    public TextMeshProUGUI tileInfo; // Text component to display the current build mode

    public void Awake()
    {
        MapController.hoveredTile += UpdateTileInfo; // Subscribe to the hoveredTile event
    }

    public void UpdateTileInfo(GameObject tile) {
        if(tile != null){
            TileDataModel tileData = tile.GetComponent<TileController>().TileData; // Get the TileDataModel component from the hovered tile
            string cornerInfo = ""; // Initialize a string to store corner information
            for (int i = 0; i<tileData.corners.Length; i++){
                cornerInfo += $"Corner {i+1}: {tileData.GetCorner(i).x},{tileData.GetCorner(i).y},{tileData.GetCorner(i).z}\n"; // Append corner information to the string
            }
            tileInfo.text = $"Tile: {tile.name} \nCo-ords: {tileData.position.x},{tileData.position.y},{tileData.position.z}\nCorners: \n[\n{cornerInfo}]"; // Update the text with the name of the hovered tile
        }
        else{
            tileInfo.text = ""; // Reset the text if no tile is hovered
        }
    }

}