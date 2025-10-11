using UnityEngine;

public class ContainerPreview : MonoBehaviour
{
    public Color colour = Color.green;
    public bool displayOutline = true;

    void OnDrawGizmosSelected()
    {
        if (displayOutline)
        {
            Gizmos.color = colour;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}
