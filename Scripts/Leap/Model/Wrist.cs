using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class Wrist : LeapBone {
        [SerializeField]
        Palm palm;

        public override bool IsTracking {
            get {
                return base.IsTracking;
            }

            internal set {
                if (palm) palm.IsTracking = value;
                base.IsTracking = value;
            }
        }
    }
}
