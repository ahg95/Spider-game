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

    [SerializeField]
    [Tooltip("The parent of the child hierarchy that will be searched for CurvePoints.")]
    GameObjectVariable gameObjectToSearchForCurvePoints;

    public void SearchAndRemoveCurvePoints()
    {
        List<Transform> curvePoints = GetAllCurvePointsInChildrenInOrderOfTransform(gameObjectToSearchForCurvePoints.RuntimeValue.transform);

        foreach (Transform curvePoint in curvePoints)
            curveCalculatorToManagePointsFor.RemoveCurvePoint(curvePoint);
    }

    public void SearchAndAddCurvePoints()
    {
        List<Transform> curvePoints = GetAllCurvePointsInChildrenInOrderOfTransform(gameObjectToSearchForCurvePoints.RuntimeValue.transform);

        foreach (Transform curvePoint in curvePoints)
            curveCalculatorToManagePointsFor.AddCurvePoint(curvePoint);
    }

    /// <summary>
    /// Recursively finds all <see cref="Transform"/>s in the child hierarchy of <see cref="gameObjectToSearchForCurvePoints"/> that have a <see cref="CurvePoint"/> component attached to them.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    List<Transform> GetAllCurvePointsInChildrenInOrderOfTransform(Transform parent)
    {
        List<Transform> allCurvePointsInChildren = new List<Transform>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.GetComponent<CurvePoint>())
                allCurvePointsInChildren.Add(child);

            if (0 < child.childCount)
                allCurvePointsInChildren.AddRange(GetAllCurvePointsInChildrenInOrderOfTransform(child));
        }

        return allCurvePointsInChildren;
    }
}
