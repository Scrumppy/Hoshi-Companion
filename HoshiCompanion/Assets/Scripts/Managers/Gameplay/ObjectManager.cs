using Gameplay.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StrikersParser;

namespace Gameplay.Managers
{
    /// <summary>
    /// Parent class of all Managers who store objects.
    /// </summary>
    public class ObjectManager<T> : Manager<ObjectManager<T>> where T : Component
    {
        [SerializeField] protected List<T> objectsInScene = new List<T>();

        /// <summary>
        /// Adds the specified object to this manager's list of active object in the scene.
        /// </summary>
        /// <param name="newObject">The object to add to the list.</param>
        public virtual void AddObject(T newObject)
        {
            objectsInScene.Add(newObject);
        }

        /// <summary>
        /// Removes the specified object from this manager's list of active objects in the scene.
        /// </summary>
        /// <param name="objectToRemove">The object to remove from the list.</param>
        public virtual void RemoveObject(T objectToRemove)
        {
            objectsInScene.Remove(objectToRemove);
        }

        /// <summary>
        /// Returns the list of active objects in the scene from this manager, and removes ones that are null.
        /// </summary>
        /// <returns>The list of active specified objects in the scene from this manager.</returns>
        public virtual List<T> GetObjectsInScene()
        {
            objectsInScene.RemoveAll(item => item == null);
            return objectsInScene;
        }

        /// <summary>
        /// Returns the object from the specified list at the specified index.
        /// </summary>
        /// <param name="index">The index of the desired object in the list.</param>
        /// <param name="objectList">The object list to retrieve the desired object from.</param>
        /// <returns>The object from the specified list at the specified index, or null if the index is out of range.</returns>
        public virtual T GetObjectAtIndex(int index, List<T> objectList)
        {
            if (index >= objectList.Count)
            {
                Debug.LogWarning("Index out of range for specified object list!");
                return null;
            }

            return objectList[index];
        }

        /// <summary>
        /// Runs this manager's object list and returns the one that is closest to the given position.
        /// </summary>
        /// <param name="targetPos">The target position to check which object is closer to.</param>
        /// <returns>The closest object from this manager's object list to a given target position.</returns>
        public virtual T GetClosestObjectInScene(Vector3 targetPos)
        {
            float minDistance = Mathf.Infinity;
            T nearestObject = null;

            foreach (T objectToFind in objectsInScene)
            {
                float distance = Vector3.Distance(targetPos, objectToFind.transform.position);

                if (distance < minDistance)
                {
                    nearestObject = objectToFind;
                    minDistance = distance;
                }
            }

            return nearestObject;
        }

        /// <summary>
        /// Runs this manager's object list and returns the one that is closest to the given position, excluding a given object.
        /// </summary>
        /// <param name="targetPos">The target position to check which object is closer to.</param>
        /// <param name="excludeObject">The object to exclude.</param>
        /// <returns>The closest object from this manager's object list to a given target position, excluding a given object.</returns>
        public virtual T GetClosestObjectInSceneExcluding(Vector3 targetPos, T excludeObject)
        {
            float minDistance = Mathf.Infinity;
            T nearestObject = null;

            foreach (T objectToFind in objectsInScene)
            {
                if (objectToFind == excludeObject)
                    continue;

                float distance = Vector3.Distance(targetPos, objectToFind.transform.position);

                if (distance < minDistance)
                {
                    nearestObject = objectToFind;
                    minDistance = distance;
                }
            }

            return nearestObject;
        }

        /// <summary>
        /// Runs this manager's object list and returns a random object.
        /// </summary>
        /// <returns>A random object from this manager's object list.</returns>
        public virtual T GetRandomObjectInScene()
        {
            if (objectsInScene.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, objectsInScene.Count);
            T randomObject = objectsInScene[randomIndex];

            return randomObject;
        }
    }
}

