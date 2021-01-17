using System;
using Patterns.Interfaces;
using UnityEngine;

namespace Patterns.Implementations
{
    [Serializable]
    public abstract class StateMonoBehaviour : MonoBehaviour, IState
    {
        public IStateMachine Owner { get; set; }

        public virtual void DestroyState() {}
        public virtual void PrepareState() {}
        public virtual void UpdateState() {}
    }
}