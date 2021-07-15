using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePointManager : MonoBehaviour
{
    [SerializeField]
    CurveCalculator curveCalculatorToManagePointsFor;

    [SerializeField]
    Transform parentOfCurvePoints;

    private void OnEnable()
    {
        if (!parentOfCurvePoints)
            parentOfCurvePoints = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            Debug.Log("TEST");

        FindAllCurvePointsInChildrenAndUpdateCurveCalculator();
    }

    void FindAllCurvePointsInChildrenAndUpdateCurveCalculator()
    {
        curveCalculatorToManagePointsFor.SetCurvePoints(GetAllCurvePointsInChildrenInOrderOfTransform(parentOfCurvePoints));
    }

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
