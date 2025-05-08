using UnityEngine;
using System.Collections.Generic;

public class WallDeleteService : IWallDeleter{
    private IMapController mapController;
    private IMoneyController moneyController;
    private IWallBuilder wallBuilder;

    public WallDeleteService(IMapController mapController, IMoneyController moneyController, IWallBuilder wallBuilder)
    {
        this.mapController = mapController;
        this.moneyController = moneyController;
        this.wallBuilder = wallBuilder;
    }

    public void TrimWallToExcludeSegment(Vector3 cornerA, Vector3 cornerB, GameObject wallToTrim)
    {
        if (wallToTrim == null) return;

        WallDataModel wallData = ExtractWallData(wallToTrim);

        // get the section of the wall to delete
       (float deleteStart, float deleteEnd, float wallLength, Vector3 wallDirection) = GetDeleteSegmentOnWall(cornerA, cornerB, wallData);

        // If delete covers entire wall, exit early
        if(ShouldDeleteEntireWall(deleteStart, deleteEnd, wallLength)) return;
        
        // Create new wall segment before deleted section
        CreateRemainingWallSegments(wallData.PointA, wallData.PointB, wallDirection, deleteStart, deleteEnd);
    }

    private void CreateRemainingWallSegments(Vector3 start, Vector3 end, Vector3 wallDirection, float deleteStart, float deleteEnd)
    {   
        if (deleteStart > 0)
        {
            Vector3 newStart = start;
            Vector3 newEnd = start + wallDirection * deleteStart;
            wallBuilder.CreateWall(WallMathUtils.AdjustWallHeight(newStart, -1.5f), WallMathUtils.AdjustWallHeight(newEnd, -1.5f));
        }

        if (deleteEnd < Vector3.Distance(start, end))
        {
            Vector3 newStart = start + wallDirection * deleteEnd;
            Vector3 newEnd = end;
            wallBuilder.CreateWall(WallMathUtils.AdjustWallHeight(newStart, -1.5f), WallMathUtils.AdjustWallHeight(newEnd, -1.5f));
        }
    }

    private (float deleteStart, float deleteEnd, float wallLength, Vector3 wallDirection) 
    GetDeleteSegmentOnWall(Vector3 cornerA, Vector3 cornerB, WallDataModel wallData)
    {
        Vector3 start = wallData.PointA;
        Vector3 end = wallData.PointB;
        Vector3 wallDirection = (end - start).normalized;
        float wallLength = Vector3.Distance(start, end);

        float aAlong = Vector3.Dot(cornerA - start, wallDirection);
        float bAlong = Vector3.Dot(cornerB - start, wallDirection);

        float deleteStart = Mathf.Clamp(Mathf.Min(aAlong, bAlong), 0, wallLength);
        float deleteEnd = Mathf.Clamp(Mathf.Max(aAlong, bAlong), 0, wallLength);

        return (deleteStart, deleteEnd, wallLength, wallDirection);
    }

    private bool ShouldDeleteEntireWall(float deleteStart, float deleteEnd, float wallLength)
    {
        return Mathf.Approximately(deleteStart, 0) && Mathf.Approximately(deleteEnd, wallLength);
    }

    private WallDataModel ExtractWallData(GameObject wall)
    {
        return wall.GetComponent<WallController>().WallData;
    }

    
}