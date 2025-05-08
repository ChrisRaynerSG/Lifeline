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
}