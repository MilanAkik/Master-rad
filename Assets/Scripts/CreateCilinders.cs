using UnityEditor;
using Random = UnityEngine.Random;
using UnityEngine;
using Assets.Scripts.Generators;
using Assets.Scripts.Models;

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
        var generatorParameters = new GeneratorParameters { CylinderCount = count, RandomSeed = seed };
        var vectors = generator.getCylinders(generatorParameters);
        var f = 0.1f;
        foreach (var vec in vectors)
        {
            var x = vec.x * parentScale.x;
            var y = vec.y * parentScale.y;
            var z = vec.z * parentScale.z;
            var r1 = vec.w;
            if (r1 < f) continue;
            var a1 = 2 / (f - 1);
            var a = a1 * a1;
            var b = -a * (f + 1);
            var c = 1 - a - b;
            var h = 2f - a * r1 * r1 - b * r1 - c;
            var r = 1f + r1 * (parentScale.x + parentScale.z) / 4.0f;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            cube.transform.position = parentPosition + new Vector3(x, y + h / 2.0f, z);
            cube.transform.rotation = Quaternion.identity;
            cube.transform.localScale = new Vector3(r, h, r);
            cube.transform.parent = parentTransform;
            cube.name = $"Cylinder({x},{y},{z})";
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
