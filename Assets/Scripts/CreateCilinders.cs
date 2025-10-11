using Random = UnityEngine.Random;
using UnityEngine;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CreateCilinders : MonoBehaviour
{

    public int seed = 1234;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Random.InitState(seed);
        var parentTransform = gameObject.transform;
        var parentPosition = parentTransform.position;
        var parentScale = parentTransform.localScale / 2;
        for (int i=0; i<100; i++){

            var x = Random.Range(-1.0f, 1.0f) * parentScale.x;
            var r = Random.Range(0.01f, 0.99f);
            var f = 0.1f;
            if (r < f) continue;
            var a1 = 2 / (f - 1);
            var a = a1 * a1;
            var b = -a * (f + 1);
            var c = 1 - a - b;
            var h = 2 - a * r * r - b * r - c;
            var z = Random.Range(-1.0f, 1.0f) * parentScale.z;
            var startingHeight = Random.Range(-0.5f, 0.5f);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cube.transform.position = parentPosition + new Vector3(x, startingHeight + h/2.0f, z);
            cube.transform.rotation = Quaternion.identity;
            cube.transform.localScale = new Vector3(3*r,h, 3 * r);
            cube.transform.parent = parentTransform;
            cube.name = "Cylinder-"+i;
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = Color.white;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
