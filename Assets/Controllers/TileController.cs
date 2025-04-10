using UnityEngine;
public class TileController : MonoBehaviour
{
    private TileDataModel tileData;
    public TileDataModel TileData
    {
        get { return tileData; }
        private set { tileData = value; }
    }

    // initialise tile with data
    public void Initialise(Vector3 position){
        tileData = new TileDataModel(position);
    }
}