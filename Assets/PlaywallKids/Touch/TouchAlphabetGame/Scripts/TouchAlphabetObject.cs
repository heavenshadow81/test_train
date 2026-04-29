using UnityEngine;
using System.Collections;

public class TouchAlphabetObject : MonoBehaviour
{


    void OnEnable()
    {
        BezierMove bezier = this.GetComponent<BezierMove>();
        if (bezier == null) bezier = this.gameObject.AddComponent<BezierMove>();
        bezier.usePosition = false;
        bezier.wayPoint0 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        bezier.wayPoint1 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        StartCoroutine(FindRenderProcess());
    }

    [HideInInspector]
    public int alphabetType
    {
        get
        {
            int type = 0;
            SkinnedMeshRenderer mesh = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (mesh != null)
            {
                type = mesh.sharedMesh.name[0];
            }

            return type;
        }
    }

    IEnumerator FindRenderProcess()
    {
        Renderer render = this.GetComponentInChildren<Renderer>();
        while (render == null)
        {
            render = this.GetComponentInChildren<Renderer>();
            yield return new WaitForEndOfFrame();
        }
        Material mat = render.material;
        //mat.color = (new TwoDimensionNeonPanel.ColorTable()).GetRandomColor();
        mat.SetColor("_Color_One", GetRandomColor());

        render.material = mat;
    }

    Color GetRandomColor()
    {
        Color[] c = new Color[]{
            new Color(0f, 0.329f, 1f ),
            new Color(0f, 1f,     0.965f),
            new Color(1f, 0.706f, 0f),
            new Color(1f, 0f,     0.4f),
            new Color(1f, 0f,     0.965f),
            new Color(1f, 0f,     0f),
            new Color(0.047f, 1f, 0f),
            new Color(0.635f, 0f, 1f),
            new Color(1f,     1f, 1f),
            new Color(1f, 0.918f, 0f),
        };
        return c[Random.Range(0, c.Length)];
    }
}