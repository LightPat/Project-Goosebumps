using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshFiltersIntoColliders : Editor
{
    [MenuItem("GameObject/Convert child meshes into separate colliders")]
    public static void generateChildColliders()
    {
        if (EditorUtility.DisplayDialog("Add colliders to children", "Do you really want to generate colliders on the children of this object?", "Generate child colliders", "Cancel"))
        {
            GameObject currentObject = Selection.activeGameObject;

            MeshFilter[] meshFilters = currentObject.GetComponentsInChildren<MeshFilter>();

            int count = 0;
            foreach (Transform child in currentObject.transform)
            {
                MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = meshFilters[count].sharedMesh;
                mc.convex = true;

                count++;
            }
        }
    }
}
