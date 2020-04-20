using UnityEngine;
using UnityEngine.Events;

namespace AKAGF.GameArchitecture.MonoBehaviours.GUI {

    
    public class AnimableGUIElement : MonoBehaviour {

        public enum SCALE_PIVOT { TOP_LEFT, TOP, TOP_RIGHT, MIDDLE_LEFT, MIDDLE, MIDDLE_RIGHT, BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT }
        public SCALE_PIVOT pivot;

        public bool unscaledTime;

        public MoveGUIAnimation moveAnim;
        public ScaleGUIAnimation scaleAnim;
        public RotateGUIAnimation rotateAnim;
        public FadeGUIAnimation fadeAnim;
        public AnimableGUIElementEvents events;

        public bool endAnimationsFlag { get; private set; }
        private bool[] eventsFlags = new bool[4];

        private bool active;
        private RectTransform tr ;
        private float timer;

        [ExecuteInEditMode]
        void Start() {
            tr = GetComponent<RectTransform>();
            endAnimationsFlag = true;
            setPivot(tr);
            moveAnim.init(tr);
            rotateAnim.init(tr);
            scaleAnim.init(tr);
            fadeAnim.init(tr);
        }

        private void LateUpdate() {

            timer += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            setPivot(tr);
            eventsFlags[0] = moveAnim.updateEffect(tr, active, timer);
            eventsFlags[1] = scaleAnim.updateEffect(tr, active, timer);
            eventsFlags[2] = rotateAnim.updateEffect(tr, active, timer);
            eventsFlags[3] = fadeAnim.updateEffect(tr, active, timer);


            if (eventsFlags[0] && eventsFlags[1] && eventsFlags[2] && eventsFlags[3] && !endAnimationsFlag) {

                endAnimationsFlag = true;

                if (active) {
                    events.onActivationEnd.Invoke();
                    //Debug.Log("On Activation End event invoked");
                }
                else {
                    events.onDeactivationEnd.Invoke();
                    //Debug.Log("On Deactivation End event invoked");
                }

            }
        }

        public void switchState() {

            setState(!active);

        }

        public void delayedSwithState(float delay) {

            Invoke("switchState", delay);
        }

        public void setState(bool state) {

            if (state != active) {

                timer = 0;
                active = state;
                endAnimationsFlag = false;

                if (state)
                    events.onActivationStart.Invoke();
                else
                    events.onDeactivationStart.Invoke();
            }
        }

        private void setPivot(RectTransform transf) {

            switch (pivot) {
                case SCALE_PIVOT.TOP_LEFT:
                    SetPivot(transf, new Vector2(0f, 1f));
                    break;
                case SCALE_PIVOT.TOP:
                    SetPivot(transf, new Vector2(0.5f, 1f));
                    break;
                case SCALE_PIVOT.TOP_RIGHT:
                    SetPivot(transf, new Vector2(1f, 1f));
                    break;
                case SCALE_PIVOT.MIDDLE_LEFT:
                    SetPivot(transf, new Vector2(0f, 0.5f));
                    break;
                case SCALE_PIVOT.MIDDLE:
                    SetPivot(transf, new Vector2(0.5f, 0.5f));
                    break;
                case SCALE_PIVOT.MIDDLE_RIGHT:
                    SetPivot(transf, new Vector2(1, 0.5f));
                    break;
                case SCALE_PIVOT.BOTTOM_LEFT:
                    SetPivot(transf, new Vector2(0f, 0f));
                    break;
                case SCALE_PIVOT.BOTTOM:
                    SetPivot(transf, new Vector2(0.5f, 0f));
                    break;
                case SCALE_PIVOT.BOTTOM_RIGHT:
                    SetPivot(transf, new Vector2(1f, 0f));
                    break;
            }
        }

        public static void SetPivot(RectTransform target, Vector2 pivot) {
            if (!target) return;
            var offset = pivot - target.pivot;
            offset.Scale(target.rect.size);
            var wordlPos = target.position + target.TransformVector(offset);
            target.pivot = pivot;
            target.position = wordlPos;
        }

        [System.Serializable]
        public struct AnimableGUIElementEvents {
            public UnityEvent onActivationStart;
            public UnityEvent onActivationEnd;
            public UnityEvent onDeactivationStart;
            public UnityEvent onDeactivationEnd;
        }

        [System.Serializable]
        public abstract class BaseGUIAnimation {

            public bool enable;

            public AnimationCurve easeInCurve = new AnimationCurve();
            public AnimationCurve easeOutCurve = new AnimationCurve();
            public float effectInDuration = 1f;
            public float effectOutDuration = 1f;

            public abstract void init(RectTransform transf);
            public abstract bool updateEffect(RectTransform transf, bool active, float timer);
        }

        [System.Serializable]
        public class MoveGUIAnimation : BaseGUIAnimation {

            public RectTransform startPosition;
            public RectTransform endPosition;
            private Vector3 targetPosition;
            private Vector3 inititalPosition;
            private Vector3 cachedDir;

            public override void init(RectTransform transf) {

                if (!enable) return;

                transf.position = startPosition.position;

            }

            public override bool updateEffect(RectTransform transf, bool active, float timer) {

                if (!enable) return true;

                targetPosition = active ? endPosition.position : startPosition.position;
                inititalPosition = !active ? endPosition.position : startPosition.position;

                cachedDir = (targetPosition - inititalPosition).normalized;
                float distance = Vector3.Distance(inititalPosition, targetPosition);
                float t = active ? easeInCurve.Evaluate(timer / effectInDuration) : easeOutCurve.Evaluate(timer / effectOutDuration);
                transf.position = inititalPosition + cachedDir * (distance * t);

                return Mathf.Approximately(transf.position.sqrMagnitude, targetPosition.sqrMagnitude);
            }
        }

        [System.Serializable]
        public class ScaleGUIAnimation : BaseGUIAnimation {

            public Vector3 startScale;
            public Vector3 endScale;

            private Vector3 targetScale = new Vector3();
            private Vector3 cachedScale = new Vector3();

            public override void init(RectTransform transf) {

                if (!enable) return;

                transf.localScale = startScale;

            }


            public override bool updateEffect(RectTransform transf, bool active, float timer) {

                if (!enable) return true;

                targetScale = active ? endScale : startScale;

                float t = active ? easeInCurve.Evaluate(timer / effectInDuration) : easeOutCurve.Evaluate(timer / effectOutDuration);

                if (active)
                    cachedScale = (targetScale * t);
                else {
                    cachedScale.x -= endScale.x != startScale.x ? (transf.localScale.x * t) : 0f;
                    cachedScale.y -= endScale.y != startScale.y ? (transf.localScale.y * t) : 0f;
                    cachedScale.z -= endScale.z != startScale.z ? (transf.localScale.z * t) : 0f;
                }

                transf.localScale = cachedScale;

                return Mathf.Approximately(transf.localScale.sqrMagnitude, targetScale.sqrMagnitude);
            }
        }

        [System.Serializable]
        public class RotateGUIAnimation : BaseGUIAnimation {

            public Vector3 startRotation;
            public Vector3 endRotation;

            private Vector3 initialRotation = new Vector3();
            private Vector3 targetRotation = new Vector3();

            private Vector3 cachedEulerRotation = new Vector3();

            public override void init(RectTransform transf) {

                if (!enable) return;

                initialRotation = transf.rotation.eulerAngles;
                transf.rotation = Quaternion.Euler(startRotation);
                cachedEulerRotation = new Vector3();
            }


            public override bool updateEffect(RectTransform transf, bool active, float timer) {

                if (!enable) return true;

                targetRotation = active ? endRotation : startRotation;
                float t = active ? easeInCurve.Evaluate(timer / effectInDuration) : easeOutCurve.Evaluate(timer / effectOutDuration);


                if (active)
                    cachedEulerRotation -= (cachedEulerRotation * t);
                else {
                    cachedEulerRotation = targetRotation * t;
                }


                transf.rotation = Quaternion.Euler(cachedEulerRotation);

                return Mathf.Approximately(cachedEulerRotation.sqrMagnitude, targetRotation.sqrMagnitude);
            }
        }

        [System.Serializable]
        public class FadeGUIAnimation : BaseGUIAnimation {

            [Range(0f, 1f)]
            public float startAlpha;
            [Range(0f, 1f)]
            public float endAlpha;

            private CanvasGroup canvasGroup;
            private float cachedAlpha;
            private float targetAlpha;

            public override void init(RectTransform transf) {

                if (!enable) return;

                canvasGroup = transf.GetComponent<CanvasGroup>();

                if (!canvasGroup)
                    canvasGroup = transf.gameObject.AddComponent<CanvasGroup>();

                canvasGroup.alpha = startAlpha;

            }


            public override bool updateEffect(RectTransform transf, bool active, float timer) {

                if (!enable) return true;

                targetAlpha = active ? endAlpha : startAlpha;

                float t = active ? easeInCurve.Evaluate(timer / effectInDuration) : easeOutCurve.Evaluate(timer / effectOutDuration);

                if (active)
                    cachedAlpha = (targetAlpha * t);
                else {
                    cachedAlpha -= (cachedAlpha * t);
                }

                if (!canvasGroup)
                    init(transf);

                canvasGroup.alpha = cachedAlpha;

                return canvasGroup.alpha == targetAlpha;
            }
        }

    }
}