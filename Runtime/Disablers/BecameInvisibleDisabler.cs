using UnityEngine;

namespace OneM.PoolSystem
{
    /// <summary>
    /// Deactivates when GameObject became invisible.
    /// <para>You should add any <see cref="Renderer"/> component in this GameObject.</para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BecameInvisibleDisabler : MonoBehaviour
    {
        private void Reset() => CheckAnyRenderer();
        private void OnBecameInvisible() => gameObject.SetActive(false);

        private void CheckAnyRenderer()
        {
            var hasRenderer = transform.TryGetComponent(out Renderer _);
            if (!hasRenderer) Debug.LogWarning("You should add any Renderer component in this GameObject.");
        }
    }
}