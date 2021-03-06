﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.Leap {
    public class Hand : LeapBone {
        [SerializeField]
        Palm palm;
        [SerializeField]
        Wrist wrist;

        public Palm Palm {
            get { return palm; }
        }

        public Wrist Wrist {
            get { return wrist; }
        }

        public override bool IsTracking {
            get {
                return base.IsTracking;
            }

            internal set {
                if (palm) palm.IsTracking = value;
                if (wrist) wrist.IsTracking = value;
                base.IsTracking = value;
            }
        }
    }
}
