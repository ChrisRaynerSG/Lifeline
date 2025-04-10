using UnityEngine;

public class TileDataModel {
    public Vector3 position;
    public Vector3[] corners = new Vector3[4];

    public Vector3[] CalculateCorners(Vector3 position){
        corners[0] = position + new Vector3(-0.5f, 0, -0.5f);
        corners[1] = position + new Vector3(0.5f, 0, -0.5f);
        corners[2] = position + new Vector3(0.5f, 0, 0.5f);
        corners[3] = position + new Vector3(-0.5f, 0, 0.5f);
        return corners;
    }

    public TileDataModel(Vector3 position){
        this.position = position;
        CalculateCorners(position);
    }

    public Vector3 GetCorner(int index){
        if(index < 0 || index >= corners.Length){
            Debug.LogError("Index out of bounds for corners array.");
            return Vector3.zero;
        }
        return corners[index];
    }
}