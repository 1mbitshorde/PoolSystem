using System;
using UnityEngine;
using UnityEngine.Pool;
using ActionCode.Attributes;

namespace OneM.PoolSystem
{
    /// <summary>
    /// Pool System for <see cref="Poolable"/> components. 
    /// Use a Prefab with a <see cref="Poolable"/> component attached on it.    /// 
    /// </summary>
    /// <remarks>
    /// The pool will always return an item even though reaching its max size. 
    /// Remaining items will be destroyed after used.<br/>
    /// For best performance, always set <see cref="Size"/> to the max quantity 
    /// of items you need to use at runtime.
    /// </remarks>
	[DisallowMultipleComponent]
    public sealed class PoolSystem : MonoBehaviour
    {
        [AssetsOnly(typeof(GameObject))]
        [Tooltip("The Pool System Prefab. Put here any Prefab with a Poolable component attached on it.")]
        public GameObject Prefab;
        [Min(1), Tooltip("The Pool System size. Items will be created even above this value.")]
        public uint Size = 4;

        /// <summary>
        /// The current number of poolable instances.
        /// </summary>
        public int Count => pool.CountAll;

        /// <summary>
        /// The current number of active poolable instances.
        /// </summary>
        public int Actives => pool.CountActive;

        /// <summary>
        /// The current number of inactive poolable instances.
        /// </summary>
        public int Inactives => pool.CountInactive;

        private ObjectPool<Poolable> pool;
        private PoolableData internalData;

        private static Transform GlobalParent => lazyGlobalParent.Value;
        private static readonly Lazy<Transform> lazyGlobalParent = new(FindOrCreateGlobalParent);

        private void Awake() => InitializePool();

        /// <summary>
        /// Places a <see cref="Poolable"/> component using the given placement position, rotation and scale.
        /// </summary>
        /// <param name="placement">The transform to place the object.</param>
        /// <returns>A <see cref="Poolable"/> instance.</returns>
        public Poolable Place(Transform placement)
        {
            placement.GetPositionAndRotation(out var position, out var rotation);
            return Place(position, rotation, transform.localScale);
        }

        /// <summary>
        /// Places a <see cref="Poolable"/> component at the given position, with identity rotation and one unit scale.
        /// </summary>
        /// <param name="position">The position to place the object.</param>
        /// <returns><inheritdoc cref="Place(Transform)"/></returns>
        public Poolable Place(Vector3 position) => Place(position, Quaternion.identity, Vector3.one);

        /// <summary>
        /// Places a <see cref="Poolable"/> component at the given position and rotation, with one unit scale.
        /// </summary>
        /// <param name="position"><inheritdoc cref="Place(Vector3)"/></param>
        /// <param name="rotation">The Euler rotation to place the object.</param>
        /// <returns><inheritdoc cref="Place(Vector3)"/></returns>
        public Poolable Place(Vector3 position, Vector3 rotation) => Place(position, Quaternion.Euler(rotation), Vector3.one);

        /// <summary>
        /// Places a <see cref="Poolable"/> component at the given position and rotation, with one unit scale.
        /// </summary>
        /// <param name="position"><inheritdoc cref="Place(Vector3)"/></param>
        /// <param name="rotation"><inheritdoc cref="Place(Vector3, Quaternion)(Vector3)"/></param>
        /// <returns><inheritdoc cref="Place(Vector3)"/></returns>
        public Poolable Place(Vector3 position, Quaternion rotation) => Place(position, rotation, Vector3.one);

        /// <summary>
        /// Places the <see cref="Poolable"/> object at the given position, rotation and scale.
        /// </summary>
        /// <param name="position"><inheritdoc cref="Place(Vector3)"/></param>
        /// <param name="rotation"><inheritdoc cref="Place(Vector3, Quaternion)(Vector3)"/></param>
        /// <param name="scale">The scale to place the object.</param>
        /// <param name="parent">Places the object as a child if specified.</param>
        /// <returns><inheritdoc cref="Place(Vector3)"/></returns>
        public Poolable Place(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null) =>
            Place(new PoolableData(position, rotation, scale, parent));

        /// <summary>
        /// Places the <see cref="Poolable"/> object at the given PoolableData data.
        /// </summary>
        /// <param name="data">The poolable place data.</param>
        /// <returns><inheritdoc cref="Place(Vector3)"/></returns>
        public Poolable Place(PoolableData data)
        {
            if (data.Parent == null) data.Parent = GlobalParent;
            internalData = data;
            return pool.Get(); // Calls CreateInstance and/or OnGetInstance functions.
        }

        internal void SendBack(Poolable poolable) => pool.Release(poolable);

        private void InitializePool()
        {
            var maxSize = (int)Size;
            pool = new ObjectPool<Poolable>(
                CreateInstance,
                OnGetInstance,
                OnReleaseInstance,
                OnDestroyInstance,
                collectionCheck: true,
                defaultCapacity: maxSize,
                maxSize
            );
        }

        private Poolable CreateInstance()
        {
            var instance = Instantiate(Prefab);
            // Deactivating instance so it won't be placed at Prefab position for one frame.
            instance.SetActive(false);

            var poolable = instance.GetComponent<Poolable>();

            poolable.Pool = this;
            poolable.gameObject.name = $"{Prefab.name}_{Count:D2}";

            return poolable;
        }

        private void OnGetInstance(Poolable poolable)
        {
            // Place() should be called before enabling instance to
            // avoid it be placed at the Prefab position for one frame.

            poolable.Place(internalData);
            poolable.gameObject.SetActive(true);

            internalData = default;
        }

        private void OnReleaseInstance(Poolable poolable) => SendBackAfterOneFrame(poolable);
        private void OnDestroyInstance(Poolable poolable) => Destroy(poolable.gameObject);

        private async void SendBackAfterOneFrame(Poolable poolable)
        {
            // Cannot call poolable.Place in the same frame of an OnDisable event since it would change the poolable parent.
            // This would throw an error:
            // https://forum.unity.com/threads/gameobject-is-already-being-activated-or-deactivated.279888/

            poolable.gameObject.SetActive(false);
            await Awaitable.NextFrameAsync();
            // OnDestroyInstance may be called if pool size is smaller than current pool instances
            var isAbleToPlaceBack = poolable && !poolable.gameObject.activeInHierarchy;
            if (isAbleToPlaceBack) poolable.Place(transform);
        }

        private static Transform FindOrCreateGlobalParent()
        {
            const string name = "ActivePoolableObjects";
            var instance = GameObject.Find(name);

            if (instance == null) instance = new GameObject(name);

            DontDestroyOnLoad(instance);
            return instance.transform;
        }
    }
}