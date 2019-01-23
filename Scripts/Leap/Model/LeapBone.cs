using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class LeapBone : MonoBehaviour {
        [SerializeField]
        protected bool enablePositionTracking = true;
        [SerializeField]
        protected bool enableRotationTracking = false;
        private bool isTracking;

        protected LineRenderer lineRenderer;

        private void Start() {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer) {
                lineRenderer.enabled = false;
            }
        }

        public LineRenderer LineRenderer {
            get {
                return lineRenderer;
            }
        }

        public virtual bool IsTracking {
            get {
                return isTracking;
            } internal set {
                isTracking = value;
            }
        }

        public virtual void ApplyBones(BoneDecoder boneDecoder) {
            if (enablePositionTracking) boneDecoder.ApplyTransform(transform);
            if (enableRotationTracking) boneDecoder.ApplyGlobalRotation(transform);
        }
    }
}
