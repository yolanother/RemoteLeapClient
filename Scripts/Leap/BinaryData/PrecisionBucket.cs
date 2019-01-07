using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionBucket {
    public enum Precision : int {
        TinyPrecision,
        LowPrecision,
        MediumPrecision,
        HighPrecision,
        PrecisionMax
    }

    public static readonly float[] PrecisionMultipliers = {
        100,
        1000,
        10000,
        100000
    };

    public static float GetPrecisionMultiplier(Precision precision) {
        return PrecisionMultipliers[(int)precision];
    }

    public static float ApplyPrecision(Precision precision, float value) {
        return value / (float) GetPrecisionMultiplier(precision);
    }
}