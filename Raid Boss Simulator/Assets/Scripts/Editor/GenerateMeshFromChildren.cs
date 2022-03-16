using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateMeshFromChildren : Editor
{
    [MenuItem("GameObject/Generate Mesh From Children")]
    public static void generateMesh()
    {
        if (EditorUtility.DisplayDialog("Generate Mesh From Children", "Do you really want to generate a mesh from the children of this object ?", "Generate Mesh", "Cancel"))
        {
            // You need a mesh filter component on the parent object for this to work
            GameObject currentObject = Selection.activeGameObject;

            resetTransform(currentObject);

            if (!currentObject.GetComponent<MeshFilter>())
            {
                currentObject.AddComponent<MeshFilter>();
            }

            MeshFilter[] meshFilters = currentObject.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];

            // Start at index 1 so that we don't use the parent's mesh, which should be empty
            for (int i = 1; i < meshFilters.Length; i++)
            {
                combine[i-1].mesh = meshFilters[i].sharedMesh;
                combine[i-1].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.SetActive(false);
                //meshFilters[i].gameObject.GetComponent<BoxCollider>().enabled = false;
            }

            currentObject.GetComponent<MeshFilter>().mesh = new Mesh();
            currentObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);

            string filepath = "Assets/" + currentObject.name + ".asset";
            Debug.Log(filepath);

            AssetDatabase.CreateAsset(currentObject.GetComponent<MeshFilter>().sharedMesh, filepath);
            AssetDatabase.SaveAssets();
        }
    }

    private static void resetTransform(GameObject g)
    {
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.identity;
        g.transform.localScale = Vector3.one;
    }
}
