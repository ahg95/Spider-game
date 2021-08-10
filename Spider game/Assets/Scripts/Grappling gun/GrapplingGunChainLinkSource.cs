using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnsgarsAssets
{
    [RequireComponent(typeof(ChainLinkSource), typeof(Sticky))]
    public class GrapplingGunChainLinkSource : MonoBehaviour
    {
        ChainLinkSource chainLinkSource;

        Sticky sticky;

        new Rigidbody rigidbody;

        public ChainLinkSource GetChainLinkSource()
        {
            if (!chainLinkSource)
                chainLinkSource = GetComponent<ChainLinkSource>();

            return chainLinkSource;
        }

        public Sticky GetSticky()
        {
            if (!sticky)
                sticky = GetComponent<Sticky>();

            return sticky;
        }

        public Rigidbody GetRigidbody()
        {
            if (!rigidbody)
                rigidbody = GetComponent<Rigidbody>();

            return rigidbody;
        }
    }
}