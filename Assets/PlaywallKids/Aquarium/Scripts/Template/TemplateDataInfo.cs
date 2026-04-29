using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    /// <summary>
    /// Aquarium template information class.
    /// </summary>
    public class TemplateDataInfo : MonoBehaviour
    {
        public string templateName;
        public string[] templates;
        public string[] patterns;
        public string[] effects;
        public Vector2[] areas;
        
        public void OnDrawGizmos()
        {
            Texture2D[] templateTexture = new Texture2D[templates.Length];
            Texture2D[] patternTexture = new Texture2D[patterns.Length];
            Texture2D[] effectTexture = new Texture2D[effects.Length];
            Texture2D[] areaTexture = new Texture2D[areas.Length];
            Texture2D tex = null;

            for (int i = 0; i < templates.Length; i++)
            {
                tex = Resources.Load<Texture2D>("data/Templates/Template/" + templateName + "/" + (i + 1) + "/3");
                if (tex == null)
                    tex = Resources.Load<Texture2D>("data/Templates/Template/" + templateName + "/" + (i + 1) + "/3_3");
                templateTexture[i] = tex;
            }
            for (int i = 0; i < patterns.Length; i++)
            {
                tex = Resources.Load<Texture2D>("data/Templates/Pattern/" + templateName + "/" + (i + 1) + "/3");
                if (tex == null)
                    tex = Resources.Load<Texture2D>("data/Templates/Pattern/" + templateName + "/" + (i + 1) + "/3_3");
                patternTexture[i] = tex;
            }
            for (int i = 0; i < effects.Length; i++)
            {
                tex = Resources.Load<Texture2D>("data/Templates/Effect/" + templateName + "/" + (i + 1) + "/3");
                if (tex == null)
                    tex = Resources.Load<Texture2D>("data/Templates/Effect/" + templateName + "/" + (i + 1) + "/3_3");
                effectTexture[i] = tex;
            }
            for (int i = 0; i < areaTexture.Length; i++)
            {
                tex = Resources.Load<Texture2D>("data/Templates/Area/" + templateName + "/" + (i + 1));
                areaTexture[i] = tex;
            }

            for (int i = 0; i < templateTexture.Length; i++)
            {
                if (templateTexture[i] == null) continue;
                Gizmos.color = Color.white * 0.5f;
                Gizmos.DrawGUITexture(new Rect(
                    transform.position.x - templateTexture[i].width * 0.5f,
                    transform.position.y + templateTexture[i].height * 0.5f,
                    templateTexture[i].width,
                    -templateTexture[i].height), templateTexture[i]);
            }
            for (int i = 0; i < patternTexture.Length; i++)
            {
                if (patternTexture[i] == null) continue;
                Gizmos.color = Color.white * 0.5f;
                Gizmos.DrawGUITexture(new Rect(
                    transform.position.x - patternTexture[i].width * 0.5f,
                    transform.position.y + patternTexture[i].height * 0.5f,
                    patternTexture[i].width,
                    -patternTexture[i].height), patternTexture[i]);
            }
            for (int i = 0; i < effectTexture.Length; i++)
            {
                if (effectTexture[i] == null) continue;
                Gizmos.color = Color.white * 0.5f;
                Gizmos.DrawGUITexture(new Rect(
                    transform.position.x - effectTexture[i].width * 0.5f,
                    transform.position.y + effectTexture[i].height * 0.5f,
                    effectTexture[i].width,
                    -effectTexture[i].height), effectTexture[i]);
            }
            for (int i = 0; i < areaTexture.Length; i++)
            {
                if (areaTexture[i] == null) continue;
                Gizmos.color = Color.white;
                Gizmos.DrawGUITexture(new Rect(
                    transform.position.x + (areas[i].x - areaTexture[i].width * 0.5f),
                    transform.position.y + (areas[i].y + areaTexture[i].height * 0.5f),
                    areaTexture[i].width,
                    -areaTexture[i].height), areaTexture[i]);
            }
        }
    }
}