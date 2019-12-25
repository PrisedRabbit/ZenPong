using UnityEngine;

namespace UnityAtoms.MonoHooks
{
    [EditorIcon("atom-icon-delicate")]
    [AddComponentMenu("Unity Atoms/Hooks/On Collider 2D Hook")]
    public sealed class OnCollider2DHook : Collider2DHook
    {
        /// <summary>
        /// Set to true if Event should be triggered on `OnTriggerEnter2D`
        /// </summary>
        [SerializeField]
        private bool _triggerOnEnter = default(bool);
        /// <summary>
        /// Set to true if Event should be triggered on `OnTriggerExit2D`
        /// </summary>
        [SerializeField]
        private bool _triggerOnExit = default(bool);
        /// <summary>
        /// Set to true if Event should be triggered on `OnTriggerStay2D`
        /// </summary>
        [SerializeField]
        private bool _triggerOnStay = default(bool);

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_triggerOnEnter) OnHook(other.collider);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (_triggerOnExit) OnHook(other.collider);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (_triggerOnStay) OnHook(other.collider);
        }
    }
}