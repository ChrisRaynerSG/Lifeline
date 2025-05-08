using UnityEngine;
public interface IWallDeleter{
    void TrimWallToExcludeSegment(Vector3 cornerA, Vector3 cornerB, GameObject wallToTrim); // Method to trim a wall to exclude a segment between two corners
    
}