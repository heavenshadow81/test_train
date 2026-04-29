using UnityEngine;

[ExecuteInEditMode]
public class FadeInOutCamera : MonoBehaviour
{
    #region Public variables
    [Range(0.0f, 1.0f)]
    public float opacity = 1.0f;
    public Shader fadeShader; // Custom/FadeShader
    #endregion

    #region Private variables
    private Material _mat;
    #endregion

    void Awake()
    {
        Shader shader = Shader.Find("Custom/FadeShader");
        if (shader == null)
            shader = fadeShader;

        if (shader != null)
        {
            _mat = new Material(shader);
            if (Application.isPlaying)
            {
                opacity = 0;
                ChangeColor();
            }
        }
    }

    void OnDestroy()
    {
        if (_mat != null)
        {
            DestroyImmediate(_mat);
            _mat = null;
        }
    }

    void Update()
    {
        ChangeColor();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_mat)
            Graphics.Blit(src, dest, _mat);
    }

    void ChangeColor()
    {
        if (_mat != null && _mat.HasProperty("_Color"))
        {
            Color color = _mat.color;
            color.a = 1.0f - opacity;
            _mat.color = color;
        }
    }
}