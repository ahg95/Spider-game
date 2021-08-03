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

        /// <summary>
        /// Imagine the given vector would lie inside a square with its center at (0,0). Now imagine a circle at the same position that perfectly fits into this square.
        /// This function returns a scaled-down version of the given vector that transforms it from the square domain to the circle domain.
        /// It can be used to transform playor input from a square domain to a circle domain to prevent strafing diagonally to move faster.
        /// </summary>
        /// <param name="squareDomainVector"></param>
        /// <returns></returns>
        public static Vector2 TransformSquareDomainVectorToCircleDomain(Vector2 squareDomainVector)
        {
            Vector2 circleDomainVector = squareDomainVector;

            float transformationDividend = ProjectVectorOntoUnitSquareBounds(squareDomainVector).magnitude;

            if (transformationDividend != 0)
                circleDomainVector = squareDomainVector / transformationDividend;

            return circleDomainVector;
        }

        /// <summary>
        /// Imagine a square with the bounds (-1, -1), (-1, 1), (1, -1), and (1, 1), and some two dimensional vector. This function returns a vector with the same direction, but which lies on the bounds of this square.
        /// </summary>
        /// <param name="vectorToProject"></param>
        /// <returns></returns>
        public static Vector2 ProjectVectorOntoUnitSquareBounds(Vector2 vectorToProject)
        {
            Vector2 projectedVector = Vector2.zero;

            if (vectorToProject != Vector2.zero)
            {
                float xAbs = Mathf.Abs(vectorToProject.x);
                float yAbs = Mathf.Abs(vectorToProject.y);

                // First we determine if the vectorToProject is further away from the unit square in the x dimension or in the y dimension. If we divide the vector by the distance in that dimension, we have projected the vector to the square bounds.
                if (xAbs <= yAbs && yAbs != 0)
                    projectedVector = vectorToProject / yAbs;
                else // Don't need to check here if xAbs != 0 since either 1) yAbs is 0 but not both are zero, meaning that xAbs is not zero, or 2) zAbs was smaller than xAbs which means that it has to be greater than zero
                    projectedVector = vectorToProject / xAbs;
            }

            return projectedVector;
        }



    }
}


