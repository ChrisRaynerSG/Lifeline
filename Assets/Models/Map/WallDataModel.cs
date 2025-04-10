using UnityEngine;

public class WallDataModel{

    public float length;

    private Vector3 pointA;
    public Vector3 PointA
    {
        get { return pointA; } // Property to get the first point of the wall
        set { pointA = value; } // Property to set the first point of the wall
    }
    private Vector3 pointB;
    public Vector3 PointB
    {
        get { return pointB; } // Property to get the second point of the wall
        set { pointB = value; } // Property to set the second point of the wall
    }
    private Vector3 position;
    public Vector3 Position
    {
        get { return position; } // Property to get the position of the wall
        set { position = value; } // Property to set the position of the wall
    }
    private Vector3 rotation;
    public Vector3 Rotation
    {
        get { return rotation; } // Property to get the rotation of the wall
        set { rotation = value; } // Property to set the rotation of the wall
    }
    private Vector3 direction;
    public Vector3 Direction
    {
        get { return direction; } // Property to get the direction of the wall
        set { direction = value; } // Property to set the direction of the wall
    }

    public WallDataModel(float length, Vector3 position, Vector3 rotation){
        

        //initialse the wall data with important vectors

        this.length = length; // Set the length of the wall
        this.position = position;
        this.rotation = rotation;
        
        Quaternion rot = Quaternion.Euler(rotation);
        Vector3 forward = rot * Vector3.forward; // assuming wall stretches along Z
        Vector3 halfVector = forward.normalized * (length / 2f);

        pointA = position - halfVector;
        pointB = position + halfVector;
        direction = (pointB - pointA).normalized;
    }
}