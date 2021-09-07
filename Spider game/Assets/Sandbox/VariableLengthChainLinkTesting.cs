using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    public class VariableLengthChainLinkTesting : MonoBehaviour
    {
        public VariableLengthChainLink ChainLink;
        public ChainLinkHook hook;

        [Range(0.5f, 2)]
        public float length = 1f;

        float previousLengthValue;

        private void Start()
        {
            ChainLink.AttachToChainLinkHook(hook);
        }

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
}