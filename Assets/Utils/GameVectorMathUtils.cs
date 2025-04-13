using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
public static class GameVectorMathUtils{

    public static Vector3 CalculateDirection(Vector3 corner1, Vector3 corner2){
        Vector3 direction = corner2 - corner1; // Calculate the direction vector between the two corners
        direction = RoundDirection(direction); // Round the direction vector to a specified precision
        direction.Normalize(); // Normalize the direction vector to get a unit vector
        return direction; // Return the normalized direction vector
    }

    public static bool AreColinearAndOverlap(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, float tolerance = 1.5f)
    {
        Debug.Log("entered colinear check");
        Vector3 dirA = (a2 - a1).normalized;
        Vector3 dirB = (b2 - b1).normalized;

        // 1. Check if directions are the same (or opposite)
        if (!VectorsAreParallel(dirA, dirB, tolerance)){
            Debug.Log("directions are not parallel");
            return false;
        }
        // 2. Check if b1 and b2 lie on the same line as a1–a2
        Vector3 toB1 = b1 - a1;
        Vector3 toB2 = b2 - a1;

        Vector3 cross1 = Vector3.Cross(dirA, toB1);
        Vector3 cross2 = Vector3.Cross(dirA, toB2);

        // Check if the cross products are close to zero (indicating colinearity)
        if (cross1.magnitude > tolerance || cross2.magnitude > tolerance){
            Debug.Log("cross products are not close to zero");
            return false;
        }

        // 3. Project all points onto the direction vector and check for overlap
        float a1Dot = Vector3.Dot(dirA, a1);
        float a2Dot = Vector3.Dot(dirA, a2);
        float b1Dot = Vector3.Dot(dirA, b1);
        float b2Dot = Vector3.Dot(dirA, b2);

        float aMin = Mathf.Min(a1Dot, a2Dot);
        float aMax = Mathf.Max(a1Dot, a2Dot);
        float bMin = Mathf.Min(b1Dot, b2Dot);
        float bMax = Mathf.Max(b1Dot, b2Dot);

        Debug.Log(bMax >= aMin && bMin <= aMax);
        // Check if the projections overlap
        return bMax >= aMin && bMin <= aMax;
    }

    public static bool VectorsAreParallel(Vector3 a, Vector3 b, float tolerance = 1.5f)
    {
        return Vector3.Cross(a.normalized, b.normalized).magnitude < tolerance;
    }

    public static void MergeColinearWalls(
    Vector3 baseStart,
    Vector3 baseEnd,
    List<GameObject> candidateWalls,
    out Vector3 mergedStart,
    out Vector3 mergedEnd,
    out List<GameObject> overlappingWalls
    )
    {
        Vector3 direction = (baseEnd - baseStart).normalized;
        Vector3 origin = baseStart;

        float minProj = Vector3.Dot(direction, baseStart);
        float maxProj = Vector3.Dot(direction, baseEnd);

        overlappingWalls = new List<GameObject>();

        foreach (GameObject wall in candidateWalls)
        {
            Debug.Log("entered wall loop");
            WallController wc = wall.GetComponent<WallController>();
            if (wc == null) continue;

            Vector3 a = wc.WallData.PointA;
            Vector3 b = wc.WallData.PointB;

            if (AreColinearAndOverlap(baseStart, baseEnd, a, b))
            {
                overlappingWalls.Add(wall);
                

                float projA = Vector3.Dot(direction, a);
                float projB = Vector3.Dot(direction, b);

                minProj = Mathf.Min(minProj, projA, projB);
                maxProj = Mathf.Max(maxProj, projA, projB);
            }
        }
        // Convert projection points back into world-space positions
        mergedStart = origin + direction * (minProj - Vector3.Dot(direction, origin));
        mergedEnd = origin + direction * (maxProj - Vector3.Dot(direction, origin));
    }

    private static Vector3 RoundDirection(Vector3 direction, float precision = 0.01f){
        return new Vector3(
            Mathf.Round(direction.x / precision) * precision,
            Mathf.Round(direction.y / precision) * precision,
            Mathf.Round(direction.z / precision) * precision
        );
    }
}