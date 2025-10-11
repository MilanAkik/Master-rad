using UnityEditor;
using Random = UnityEngine.Random;
using UnityEngine;
using System;
using Assets.Scripts.Generators;
using System.Collections.Generic;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CreateCilinders : MonoBehaviour
{

    public int seed = 1234;
    public int count = 100;
    [SerializeField] private CylinderGenerator generator;

#if UNITY_EDITOR
    private bool isPendingRegenerate = false;
#endif

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!isPendingRegenerate)
        {
            isPendingRegenerate = true;
            EditorApplication.delayCall += RegenerateChildrenSafe;
        }
#endif
    }

#if UNITY_EDITOR
    private void RegenerateChildrenSafe()
    {
        if (this == null) return;
        isPendingRegenerate = false;
        ClearChildrenSafe();

        Random.InitState(seed);
        var parentTransform = gameObject.transform;
        var parentPosition = parentTransform.position;
        var parentScale = parentTransform.localScale / 2;
        var vectors = generator.getCylinders(count, seed);
        var f = 0.1f;
        foreach (var vec in vectors)
        {
            var x = vec.x * parentScale.x;
            var y = vec.y;
            var z = vec.z * parentScale.z;
            var r = vec.w;
            if (r < f) continue;
            var a1 = 2 / (f - 1);
            var a = a1 * a1;
            var b = -a * (f + 1);
            var c = 1 - a - b;
            var h = 2 - a * r * r - b * r - c;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cube.transform.position = parentPosition + new Vector3(x, y + h / 2.0f, z);
            cube.transform.rotation = Quaternion.identity;
            cube.transform.localScale = new Vector3(3 * r, h, 3 * r);
            cube.transform.parent = parentTransform;
            cube.name = $"Cylinder({x},{y},{z})";
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            //if (cubeRenderer != null) cubeRenderer.material.color = Color.white;
        }

    }

    private void ClearChildrenSafe()
    {
        while (transform.childCount > 0)
        {
            var child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
    }
#endif

    // Update is called once per frame
    void Update()
    {
        
    }
}
