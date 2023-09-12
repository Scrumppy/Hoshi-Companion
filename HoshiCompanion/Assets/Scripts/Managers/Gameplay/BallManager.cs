using Gameplay.AI;
using Gameplay.Environment.Ball;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Managers
{
    public class BallManager : ObjectManager<BallBehavior>
    {
        public override void AddObject(BallBehavior newObject)
        {
            base.AddObject(newObject);
        }

        public override void RemoveObject(BallBehavior objectToRemove)
        {
           base.RemoveObject(objectToRemove);
        }
    }
}
