using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableLengthChainLinkTesting : MonoBehaviour
{
    public VariableLengthChainLink ChainLink;

    [Range(0.5f, 2)]
    public float length = 1f;

    float previousLengthValue;

    // Update is called once per frame
    void Update()
    {
        if (length != previousLengthValue)
        {
            ChainLink.SetEffectiveLength(length);
            Debug.Log("Executed");
        }

    }

    private void LateUpdate()
    {
        previousLengthValue = length;
    }
}
