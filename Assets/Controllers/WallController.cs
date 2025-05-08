using UnityEngine;

// Component to be added to the wall prefab to manage its behavior and interactions in the game.
public class WallController : MonoBehaviour{

    private WallDataModel wallData;
    public WallDataModel WallData
    {
        get { return wallData; } // Property to get the wall data
        set { wallData = value; } // Property to set the wall data
    }

    // public void Awake()
    // {
    //     wallData = new WallDataModel(0); // Initialize the wall data with a default length of 0
    // }

    public void Initialise(float length, Vector3 position, Vector3 rotation)
    {
        wallData = new WallDataModel(length, position, rotation); // Initialize the wall data with the given parameters
        wallData.length = length; // Set the length of the wall
    }

    public bool IsIntersecting(WallController otherWall)
    {
        // Project wall points onto XZ plane
        Vector2 aStart = new Vector2(wallData.PointA.x, wallData.PointA.z);
        Vector2 aEnd = new Vector2(wallData.PointB.x, wallData.PointB.z);
        Vector2 bStart = new Vector2(otherWall.wallData.PointA.x, otherWall.wallData.PointA.z);
        Vector2 bEnd = new Vector2(otherWall.wallData.PointB.x, otherWall.wallData.PointB.z);

        return DoLineSegmentsIntersect(aStart, aEnd, bStart, bEnd);
    }

    private bool DoLineSegmentsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2)
    {
        // Uses the cross product method to check intersection
        float d = (p2.x - p.x) * (q2.y - q.y) - (p2.y - p.y) * (q2.x - q.x);

        if (Mathf.Approximately(d, 0))
            return false; // Lines are parallel

        float u = ((q.x - p.x) * (q2.y - q.y) - (q.y - p.y) * (q2.x - q.x)) / d;
        float v = ((q.x - p.x) * (p2.y - p.y) - (q.y - p.y) * (p2.x - p.x)) / d;

        return (u >= 0 && u <= 1) && (v >= 0 && v <= 1);
    }
}