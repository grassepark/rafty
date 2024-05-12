using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// Updates the tag of the GameObject based on the state of an interactable.
    /// </summary>
    public class InteractableTag : MonoBehaviour
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
                    if (transform.parent != null && transform.parent.tag == "grabbed")
                    {
                        transform.parent.tag = "caught";
                        gameObject.SetActive(false);
                    }
                    break;
                case InteractableState.Hover:
                    gameObject.tag = "Untagged"; // Adjust tag if needed for hover state
                    break;
                case InteractableState.Select:
                    // Set tag to "grabbed" for both the GameObject and its parent
                    gameObject.tag = "grabbed";
                    if (transform.parent != null)
                    {
                        transform.parent.tag = "grabbed";
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
