using Gameplay.AI;
using Gameplay.Environment.Ball;
using Gameplay.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Managers
{
    public class StrikerManager : ObjectManager<StrikerAI>
    {
        public override void AddObject(StrikerAI newObject)
        {
            base.AddObject(newObject);
        }

        public override void RemoveObject(StrikerAI objectToRemove)
        {
            base.RemoveObject(objectToRemove);
        }
    }
}
