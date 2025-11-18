using UnityEngine;

namespace OneM.PoolSystem
{
    /// <summary>
    /// Data container for Poolable objects.
    /// </summary>
    public struct PoolableData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public Transform Parent;

        public PoolableData(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Parent = parent;
        }

        public PoolableData(Transform parent) : this(
            position: Vector3.zero,
            rotation: Quaternion.identity,
            scale: Vector3.one,
            parent
        )
        { }
    }
}