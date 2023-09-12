using UnityEngine;

namespace UI
{
    public class UIScreen : MonoBehaviour
    {
        private enum InitializeMode { Manual, OnAwake, OnStart }

        [Header("Screen")]
        [SerializeField] private InitializeMode initializeMode = InitializeMode.Manual;
        public GameObject screenContent = null;
        protected Animator animator;
        private bool? isViewed = null;
        private bool isInitialized = false;

        private void Awake()
        {
            if (initializeMode == InitializeMode.OnAwake)
            {
                Initialize();
            }
        }
        private void Start()
        {
            if (initializeMode == InitializeMode.OnStart)
            {
                Initialize();
            }
        }
        private void OnDestroy()
        {
            Disable();
        }
        public virtual bool Initialize()
        {
            if (isInitialized)
                return false;

            isInitialized = true;

            if (screenContent != null)
                animator = screenContent.GetComponent<Animator>();

            return true;
        }

        public virtual bool Disable()
        {
            if (!isInitialized)
                return false;

            isInitialized = false;
            return true;
        }

        public virtual void SetScreenView(bool isView)
        {
            if (isViewed != null && isViewed.Value == isView)
                return;

            if (animator != null)
                animator.SetBool("Enabled", isView);
            else if (screenContent != null)
                screenContent.SetActive(isView);

            isViewed = isView;
        }
    }
}

