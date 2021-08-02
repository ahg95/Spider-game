using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the <c>transform</c>s that a given <c>CurveCalculator</c> should fit its curve to.
/// </summary>
public class CurvePointManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The CurveCalculator for which the CurvePoints should be managed for.")]
    CurveCalculator curveCalculatorToManagePointsFor;

    public void FindActiveCurvePointsInChildHierarchyAndSetCurveCalculatorToUseThem()
    {
        List<Transform> allActiveCurvePointsInChildHierarchy = GetAllActiveCurvePointsInChildrenInOrderOfTransform(transform);

        curveCalculatorToManagePointsFor.SetCurvePoints(allActiveCurvePointsInChildHierarchy);
    }

    /// <summary>
    /// Recursively finds all <see cref="Transform"/>s in the child hierarchy of <paramref name="parent"/> that have a <see cref="CurvePoint"/> component attached to them, in order. The search is a depth-first search.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    List<Transform> GetAllActiveCurvePointsInChildrenInOrderOfTransform(Transform parent)
    {
        List<Transform> allActiveCurvePointsInChildren = new List<Transform>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.gameObject.activeInHierarchy)
            {
                if (child.GetComponent<CurvePoint>())
                    allActiveCurvePointsInChildren.Add(child);

                if (0 < child.childCount)
                    allActiveCurvePointsInChildren.AddRange(GetAllActiveCurvePointsInChildrenInOrderOfTransform(child));
            }
        }

        return allActiveCurvePointsInChildren;
    }
}
