using Unity.VisualScripting;
using UnityEngine;
public static class GameVectorMathUtils{

    public static Vector3 CalculateDirection(Vector3 corner1, Vector3 corner2){
        Vector3 direction = corner2 - corner1; // Calculate the direction vector between the two corners
        direction.Normalize(); // Normalize the direction vector to get a unit vector
        return direction; // Return the normalized direction vector
    }

    public static bool AreColinearAndOverlap(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2){
        // Check if the two line segments are collinear and overlap
        Vector3 directionA = (a2 - a1).normalized; // Direction vector of line segment A
        Vector3 directionB = (b2 - b1).normalized;

        if(Mathf.Abs(Vector3.Dot(directionA, directionB)) < 0.999f){ // Check if the segments are not collinear
            return false; // If not collinear, return false
        }

        float aMin = Vector3.Dot(directionA, a1);
        float aMax = Vector3.Dot(directionA, a2);
        float bMin = Vector3.Dot(directionA, b1);
        float bMax = Vector3.Dot(directionA, b2);

        if(aMin > aMax)(aMin, aMax) = (aMax, aMin); // Ensure aMin is less than aMax;
        if(bMin > bMax)(bMin, bMax) = (bMax, bMin); // Ensure bMin is less than bMax

        return (aMin <= bMax && bMin <= aMax); // Check if the projections overlap
        // Direction vector of line segment B
    }
}