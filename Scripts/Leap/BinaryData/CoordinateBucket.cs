using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateBucket {
    public enum Bucket : int {
        BucketMin = 0,
        Bucket_Disabled = 0,
        Bucket_10,
        Bucket_13,
        Bucket_16,
        Bucket_19,
        Bucket_20,
        Bucket_24,
        Bucket_32,
        BucketMax
    }

    public static readonly int[] PositionBucketSizes = {
        0,
        10,
        13,
        16,
        19,
        20,
        24,
        32
    };

    public static int GetBucketSize(Bucket bucket) {
        return PositionBucketSizes[(int) bucket];
    }
}