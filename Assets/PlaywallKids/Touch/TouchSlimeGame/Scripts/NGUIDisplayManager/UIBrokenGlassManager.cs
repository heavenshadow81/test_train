using UnityEngine;
using ML.PlaywallKids.Common;

public class UIBrokenGlassManager : MonoBehaviour {
    public GameObject prefab;
    ListPool<MeshRenderer> list;
    System.Collections.Generic.List<MeshRenderer> activatedList;

    const string TINT_COLOR = "_TintColor";

    void OnEnable()
    {
        list = new ListPool<MeshRenderer>(prefab.GetComponent<MeshRenderer>());
        activatedList = new System.Collections.Generic.List<MeshRenderer>();
    }
    
    void OnDisable()
    {
        for(int i = 0 ; i< activatedList.Count ; ++i)
            Destroy(activatedList[i]);
        activatedList = null;
    }

    void OnDestroy()
    {
        list.Dispose();
    }

    void FixedUpdate()
    {
        if (activatedList.Count > 0)
        {
            for (int i = 0, len = activatedList.Count; i < len; )
            {
                MeshRenderer _renderer = activatedList[i];
                Color _c = _renderer.materials[0].GetColor(TINT_COLOR);
                _c.a -= Time.deltaTime * 0.35f;
                _renderer.materials[0].SetColor(TINT_COLOR, _c);
                if(_c.a > 0)
                {
                    activatedList[i] = _renderer;
                    ++i;
                }else
                {
                    _renderer.gameObject.SetActive(false);
                    activatedList.RemoveAt(i);
                    --len;
                }
            }
        }
    }

    public void Display(Camera _uiCam,  Vector2 _viewPort)
    {
        Vector2 _pos = ScreenUtil.ViewportToNGUIScreen(_viewPort);
        MeshRenderer _renderer = list.GetComponent;
        _renderer.materials[0].SetColor(TINT_COLOR, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        Transform _glass = _renderer.transform;
        _glass.parent = transform;
        _glass.localPosition = _pos;
        _glass.localRotation = Quaternion.identity;
        _glass.localScale = new Vector3(500, 500,1);
        _glass.gameObject.layer = gameObject.layer;
        _glass.gameObject.SetActive(true);
        activatedList.Add(_renderer);
    }
}
