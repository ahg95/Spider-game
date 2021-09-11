using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnsgarsAssets;

public class ChainLinkTester : MonoBehaviour
{
    public Transform VectorParameter;

    public void OrientHookPositionTowardsVectorParameter()
    {
        GetComponent<ChainLink>().OrientHookPositionTowards(VectorParameter.position);
    }
}
