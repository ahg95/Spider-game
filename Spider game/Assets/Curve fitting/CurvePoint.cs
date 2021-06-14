using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePoint : MonoBehaviour
{
    CurveCalculator responsibleCurveCalculator;

    private CurveCalculator GetCurveCalculator()
    {
        if (responsibleCurveCalculator == null)
            responsibleCurveCalculator = GetComponentInParent<CurveCalculator>();

        return responsibleCurveCalculator;
    }

    private void OnEnable()
    {
        GetCurveCalculator()?.AddCurvePoint(transform);
    }

    private void OnDisable()
    {
        GetCurveCalculator()?.RemoveCurvePoint(transform);
    }
}
