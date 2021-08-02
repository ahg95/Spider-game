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
    }
}


