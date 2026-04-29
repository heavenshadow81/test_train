using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    public class MaterialColorFade : MonoBehaviour
    {
        private Dictionary<Material, Renderer> matDict = new Dictionary<Material, Renderer>();

        public void FadeColor(float time)
        {
            if (matDict.Count == 0)
            {
                Renderer[] rs = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < rs.Length; i++)
                {
                    Renderer r = rs[i];
                    Material[] ms = r.materials;
                    for (int j = 0; j < ms.Length; j++)
                    {
                        Material m = ms[j];
                        matDict[m] = r;
                    }
                }
            }

            var e = matDict.GetEnumerator();
            while (e.MoveNext())
            {
                Material mat = e.Current.Key;
                Renderer r = e.Current.Value;

                string namedColorValue = "_Color";
                Color tmp = mat.GetColor(namedColorValue);
                mat.SetColor(namedColorValue, Color.white * 1.5f);
                iTween.ColorTo(r.gameObject, iTween.Hash("namedcolorvalue", namedColorValue, "color", tmp, "time", time));
            }
        }

        void OnDestroy()
        {
            var e = matDict.GetEnumerator();
            while (e.MoveNext())
            {
                Material mat = e.Current.Key;
                Renderer r = e.Current.Value;

                iTween.Stop(r.gameObject, "ColorTo");
                Destroy(mat);
            }
            matDict.Clear();
        }
    }
}