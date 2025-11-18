#if MODULE_ANIMATION
using UnityEngine;
using System.Collections;

namespace OneM.PoolSystem
{
    /// <summary>
    /// Deactivates this GameObject after the local animation finishes.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animation))]
    public sealed class AnimationDisabler : MonoBehaviour
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        [SerializeField] private Animation animation;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        private void Reset()
        {
            animation = GetComponent<Animation>();
            animation.playAutomatically = false;
        }

        private void OnEnable() => StartCoroutine(DesactivateAfterAnimationRoutine());

        private IEnumerator DesactivateAfterAnimationRoutine()
        {
            animation.Play();
            yield return new WaitUntil(() => !animation.isPlaying);
            gameObject.SetActive(false);
        }
    }
}
#endif