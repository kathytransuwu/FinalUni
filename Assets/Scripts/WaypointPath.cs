using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public Transform getWaypoints(int index)
    {
        //returns the child transforms in order as the waypoints. So like the first child is the first waypoint, the second child is the second waypoint and so on.
        return transform.GetChild(index);
    }

    public int length => transform.childCount;
}
