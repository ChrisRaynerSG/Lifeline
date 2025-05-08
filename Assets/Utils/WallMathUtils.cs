using UnityEngine;
using System.Collections.Generic; // Importing the System.Collections.Generic namespace for using List<T> class

public static class WallMathUtils{

    public static float CalculateTotalWallCost(float wallLength, float wallPerUnitCost)
    { 
        return wallLength * wallPerUnitCost; // Calculate and return the cost of the wall based on the length and cost per unit length
    }

    public static float CalculateWallLength(Vector3 corner1, Vector3 corner2)
    {
        return Vector3.Distance(corner1, corner2); // Calculate and return the length of the wall based on the two corners
    }

    public static Vector3 AdjustWallHeight(Vector3 position, float height = 1.5f){
        Vector3 heightAdjust = position;
        heightAdjust.y += height; // Adjust the Y-coordinate to raise the wall
        return heightAdjust;
    }

    public static float CalculateOverlapLength(Vector3 newStart, Vector3 newEnd, List<GameObject> existingWalls){
        
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
    public static float CalculateUniqueWallLength(Vector3 newStart, Vector3 newEnd, List<GameObject> existingWalls){

        float totalLength = Vector3.Distance(newStart, newEnd); // Calculate the total length of the new wall
        float uniqueLength = Mathf.Max(0, totalLength - CalculateOverlapLength(newStart, newEnd, existingWalls)); // Calculate the unique length of the new wall
        return uniqueLength; // Return the unique length of the new wall
    }

    public static Vector3 CalculateIntersectionPoint(WallController wallA, WallController wallB)
    {
        Vector3 aStart3D = wallA.WallData.PointA;
        Vector3 aEnd3D = wallA.WallData.PointB;
        Vector3 bStart3D = wallB.WallData.PointA;
        Vector3 bEnd3D = wallB.WallData.PointB;

        Vector2 aStart = new Vector2(aStart3D.x, aStart3D.z);
        Vector2 aEnd = new Vector2(aEnd3D.x, aEnd3D.z);
        Vector2 bStart = new Vector2(bStart3D.x, bStart3D.z);
        Vector2 bEnd = new Vector2(bEnd3D.x, bEnd3D.z);

        Vector2 intersection;
        if (TryGet2DLineIntersection(aStart, aEnd, bStart, bEnd, out intersection))
        {
            return new Vector3(intersection.x, wallA.WallData.Position.y, intersection.y);
        }

        return Vector3.zero;
    }
    private static bool TryGet2DLineIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 r = p2 - p1;
        Vector2 s = q2 - q1;

        float denominator = r.x * s.y - r.y * s.x;
        if (Mathf.Approximately(denominator, 0f))
            return false; // Lines are parallel

        Vector2 qp = q1 - p1;
        float t = (qp.x * s.y - qp.y * s.x) / denominator;
        float u = (qp.x * r.y - qp.y * r.x) / denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = p1 + t * r;
            return true;
        }

        return false;
    }

    public static bool IsPointBetween(Vector3 point, Vector3 start, Vector3 end)
    {
        Vector2 p = new Vector2(point.x, point.z);
        Vector2 a = new Vector2(start.x, start.z);
        Vector2 b = new Vector2(end.x, end.z);

        // Allow a small margin of error
        const float epsilon = 0.01f;

        float lengthAB = Vector2.Distance(a, b);
        float lengthAP = Vector2.Distance(a, p);
        float lengthPB = Vector2.Distance(p, b);

        // If the sum of the parts is almost the same as the whole, it's between
        return Mathf.Abs((lengthAP + lengthPB) - lengthAB) < epsilon;
    }
}