using UnityEngine;
using System.Collections;

namespace OneM.PoolSystem
{
    /// <summary>
    /// Deactivates this GameObject after the <see cref="timeAlive"/> finishes.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FramesDisabler : MonoBehaviour
    {
        [Tooltip("Time (in frames) where this GameObject will wait until deactivates it.")]
        public uint timeAlive = 60;

        private readonly static WaitForFixedUpdate waitOneFrame = new();

        private void OnEnable() => StartCoroutine(DeactivateRoutine());

        private IEnumerator DeactivateRoutine()
        {
            for (int i = 0; i < timeAlive; i++)
                yield return waitOneFrame;

            gameObject.SetActive(false);
        }
    }
}