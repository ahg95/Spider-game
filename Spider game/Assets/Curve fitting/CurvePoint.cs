using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class serves the simple purpose of tagging an object as a <c>CurvePoint</c>.
/// </summary>
public class CurvePoint : MonoBehaviour
{
    CurveCalculator parentCalculator;

    private void OnEnable()
    {
        TryFindingCurveCalculatorInParents();

        if (parentCalculator)
            parentCalculator.AddCurvePoint(transform);
    }

    private void OnDisable()
    {
        if (parentCalculator)
            parentCalculator.RemoveCurvePoint(transform);
    }

    void TryFindingCurveCalculatorInParents()
    {
        Transform child = transform;

        while (child.parent != null)
        {
            parentCalculator = child.parent.GetComponent<CurveCalculator>();

            if (parentCalculator)
                break;

            child = child.parent;
        }
    }


}
