using UnityEngine;

namespace OneM.PoolSystem
{
    /// <summary>
    /// Deactivates this GameObject after the <see cref="TimeAlive"/> finishes.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SecondsDisabler : MonoBehaviour
    {
        [Tooltip("Time (in seconds) where this GameObject will wait until deactivates it.")]
        public float TimeAlive = 2f;

        private void OnEnable() => Invoke(nameof(Deactivate), TimeAlive);
        private void Deactivate() => gameObject.SetActive(false);
    }
}