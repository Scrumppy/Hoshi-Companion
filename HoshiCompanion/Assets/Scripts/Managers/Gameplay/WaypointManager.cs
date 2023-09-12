using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Managers
{
    public class WaypointManager : Manager<WaypointManager>
    {
        [Header("Jogging Waypoints")]
        [SerializeField] private List<Transform> joggingWaypoints;

        [Header("Passing Waypoints")]
        [SerializeField] private List<Transform> passingWaypoints;

        [Header("Pushup Waypoints")]
        [SerializeField] private List<Transform> pushupWaypoints;

        [Header("Resting Waypoints")]
        [SerializeField] private List<Transform> restingWaypoints;

        #region GETTERS & SETTERS
        /// <summary>
        /// Returns the waypoint from the specified list at the specified index.
        /// </summary>
        /// <param name="index">The index of the desired waypoint in the list.</param>
        /// <param name="waypointList">The waypoint list to retrieve the desired waypoint from.</param>
        /// <returns>The waypoint from the specified list at the specified index, or null if the index is out of range.</returns>
        public virtual Transform GetWaypointAtIndex(int index, List<Transform> waypointList)
        {
            if (index >= waypointList.Count)
            {
                Debug.LogWarning("Index out of range for specified object list!");
                return null;
            }

            return waypointList[index];
        }
        public List<Transform> GetJoggingWaypoints()
        {
            return joggingWaypoints;
        }
        public List<Transform> GetPassingWaypoints()
        {
            return passingWaypoints;
        }
        public List<Transform> GetPushupWaypoints()
        {
            return pushupWaypoints;
        }
        public List<Transform> GetRestingWaypoints()
        {
            return restingWaypoints;
        }
        #endregion
    }
}
