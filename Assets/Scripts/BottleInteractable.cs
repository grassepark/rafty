using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// Updates the tag of the GameObject based on the state of an interactable.
    /// </summary>
    public class BottleInteractable : MonoBehaviour
    {
        [Tooltip("The interactable to monitor for state changes.")]
        /// <summary>
        /// The interactable to monitor for state changes.
        /// </summary>
        [SerializeField, Interface(typeof(IInteractableView))]
        private UnityEngine.Object _interactableView;

        private IInteractableView InteractableView;


        protected bool _started = false;

        protected virtual void Awake()
        {
            InteractableView = _interactableView as IInteractableView;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(InteractableView, nameof(InteractableView));
            UpdateTag();
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged += UpdateTagState;
                UpdateTag();
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged -= UpdateTagState;
            }
        }

        private void UpdateTag()
        {
            switch (InteractableView.State)
            {
                case InteractableState.Normal:
                    gameObject.tag = "Untagged"; 

                    break;
                case InteractableState.Hover:
                    gameObject.tag = "Untagged"; // Adjust tag if needed for hover state
                    break;
                case InteractableState.Select:
                    gameObject.tag = "grabbed";
                    if (transform.parent != null)
                    {
                        transform.parent.tag = "Grabbed";
                    }
                    break;
                case InteractableState.Disabled:
                    gameObject.tag = "Untagged"; // Adjust tag if needed for disabled state
                    break;
            }
        }

        private void UpdateTagState(InteractableStateChangeArgs args) => UpdateTag();

        #region Inject

        public void InjectAllInteractableTag(IInteractableView interactableView)
        {
            InjectInteractableView(interactableView);
        }

        public void InjectInteractableView(IInteractableView interactableView)
        {
            _interactableView = interactableView as UnityEngine.Object;
            InteractableView = interactableView;
        }

        #endregion
    }
}
