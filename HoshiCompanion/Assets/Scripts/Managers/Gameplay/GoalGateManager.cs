using Gameplay.Environment.Ball;
using Gameplay.Environment.Goal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Managers
{
    public class GoalGateManager : ObjectManager<GoalGate>
    {
        public override void AddObject(GoalGate newObject)
        {
            base.AddObject(newObject);
        }

        public override void RemoveObject(GoalGate objectToRemove)
        {
            base.RemoveObject(objectToRemove);
        }
    }
}
