using UnityEditor;
using UnityEngine;

public class ForceReserializeHelper : MonoBehaviour {

    [MenuItem("Tools/Force Reserialize Assets")]
    public static void ForceReserializeAssets() {
        AssetDatabase.ForceReserializeAssets();
    }
}