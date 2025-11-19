using UnityEngine;

namespace OneM.PoolSystem
{
    /// <summary>
    /// Poolable component used by a <see cref="Pool"/> component.
    /// </summary>
    /// <remarks>
    /// When disabled, the GameObject will be sent back to its Pool System.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class Poolable : MonoBehaviour
    {
        /// <summary>
        /// The Pool owner this component.
        /// </summary>
        public Pool Owner { get; internal set; }

        private void OnDisable()
        {
            if (Owner) Owner.SendBack(this);
        }

        internal void Place(Transform parent) => Place(new PoolableData(parent));

        internal void Place(PoolableData data)
        {
            transform.parent = data.Parent;
            transform.SetPositionAndRotation(data.Position, data.Rotation);
            transform.localScale = data.Scale;
        }
    }
}
