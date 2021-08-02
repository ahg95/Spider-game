using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnsgarsAssets {

    public static class Utility
    {

        // Credits: https://forum.unity.com/threads/prefabutility-getcorrespondingobjectfromoriginalsource-not-working-as-intended.702386/
        public static GameObject GetPrefabInstanceOfGameObject(GameObject gameObject)
        {
            string pathToPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            return PrefabUtility.GetCorrespondingObjectFromSourceAtPath(gameObject, pathToPrefab);
        }

        /// <summary>
        /// Returns true if the given <see cref="LayerMask"/> regards the specified <paramref name="layer"/>.
        /// </summary>
        /// <param name="mask"> The mask to check for if it regards the given <paramref name="layer"/></param>
        /// <param name="layer"> The number of the layer to check for. </param>
        /// <returns></returns>
        public static bool LayerMaskContainsLayer(LayerMask mask, int layer)
        {
            return IntegerHasBitSetAtIndex(mask.value, layer);
        }

        // First, we shift the bit that we want to investigate to the rightmost position, which determines if the integer is even or odd. Then we set every other bit to '0' by doing a bitwise AND operation ('&') with the number one,
        // which has a '0' on all bits except for the rightmost. If this number is equal to one, only then the bit was set.
        /// <summary>
        /// Returns true if the bit at the specified <paramref name="index"/> position of the given <paramref name="integer"/> is equal to 1. An <paramref name="index"/> of 0 corresponds to the rightmost bit which determines if the number is even or odd.
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IntegerHasBitSetAtIndex(int integer, int index) {
            /* First, the bits in the specified integer are shifted to the right index times, which moves the bit to be checked to the rightmost position. The resulting number is then used in a bit-wise AND operation with the number one, which has all bits set to 0 except for the rightmost bit.
             * This operation results in a number that has all bits set to 0 except for the rightmost bit, which is set to 1 if and only if the bit to check is set to 1. If this is the case, the resulting integer has a value of 1, which we check for.
             */
            return ((integer >> index) & 1) == 1;
        }
    }
}


