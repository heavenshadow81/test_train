using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// The singleton class which manages instantiated templates (character, freedrawing, etc) 
    /// and limits maximum number of templates in Dragon Park.
    /// </summary>
    public class SimpleInstantiatedTemplateControl : MonoBehaviour
    {
        #region Private variables
        private Dictionary<int, GameObject> _currentTemplateForUserDict = new Dictionary<int, GameObject>();
        private List<GameObject> _templateQueue = new List<GameObject>();
        #endregion

        #region Constants
        // Maximum number of templates
        public const int maxTemplateCount = 20;
        #endregion

        #region Properties
        private static SimpleInstantiatedTemplateControl __sharedInstance = null;
        public static SimpleInstantiatedTemplateControl sharedInstance
        {
            get
            {
                if (__sharedInstance == null)
                {
                    __sharedInstance = FindObjectOfType<SimpleInstantiatedTemplateControl>();
                    if (__sharedInstance == null)
                    {
                        GameObject go = new GameObject("SimpleInstantiatedTemplateControl");
                        go.transform.position = Vector3.zero;

                        __sharedInstance = go.AddComponent<SimpleInstantiatedTemplateControl>();
                    }
                }
                return __sharedInstance;
            }
        }
        #endregion

        #region Unity methods
        public void Start()
        {
            if (sharedInstance == this)
            {
                DontDestroyOnLoad(sharedInstance.gameObject);
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name.Contains("DragonPark"))
            {
                _currentTemplateForUserDict.Clear();
                _templateQueue.Clear();
            }
        }
        #endregion

        #region Managing templates
        /// <summary>
        /// Returns current instantiated template of user.
        /// </summary>
        public static GameObject GetCurrentTemplate(int userId)
        {
            GameObject go = null;
            sharedInstance._currentTemplateForUserDict.TryGetValue(userId, out go);
            return go;
        }

        /// <summary>
        /// Returns recently instantiated templates' identifier.
        /// </summary>
        public static string[] GetRecentTemplateIdentifiers(int count)
        {
            int cnt = sharedInstance._templateQueue.Count;
            List<string> identifiers = new List<string>();

            while (count-- > 0)
            {
                if (cnt-- > 0)
                {
                    GameObject go = sharedInstance._templateQueue[cnt];
                    if (go != null)
                    {
                        Template3D template = go.GetComponent<Template3D>();
                        if (template != null)
                        {
                            identifiers.Add(template.identifier);
                        }
                        else
                        {
                            Template3D[] templates = go.GetComponentsInChildren<Template3D>();
                            foreach (Template3D t in templates)
                            {
                                identifiers.Add(t.identifier);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return identifiers.ToArray();
        }

        /// <summary>
        /// Registers instantiated template in the queue.
        /// </summary>
        public static void Register(GameObject go, int userId)
        {
            if (go != null)
            {
                // if queue count is larger than max count, remove the oldest template
                if (sharedInstance._templateQueue.Count >= maxTemplateCount)
                {
                    // current user's templates
                    List<GameObject> _currentTemplates = new List<GameObject>(sharedInstance._currentTemplateForUserDict.Values);

                    // enumerate queues
                    for (int i = 0; i < sharedInstance._templateQueue.Count; i++)
                    {
                        GameObject templateGO = sharedInstance._templateQueue[i];

                        if (templateGO == null)
                        {
                            // null check
                            sharedInstance._templateQueue.RemoveAt(i--);
                        }
                        else if (!_currentTemplates.Contains(templateGO))
                        {
                            // remove oldest template (which not referenced by any user)
                            sharedInstance._templateQueue.RemoveAt(i);
                            Destroy(templateGO);
                            break;
                        }
                    }
                }
            }

            // set references!
            sharedInstance._currentTemplateForUserDict[userId] = go;
            sharedInstance._templateQueue.Add(go);
        }

        /// <summary>
        /// Loads template using identifier and queues instantiated object.
        /// </summary>
        public static Template3D LoadTemplate(string identifier)
        {
            // Load new template
            Template3D newTemplate = ResourceManager.LoadTemplate3D(identifier);

            // if queue count is larger than max count, remove the oldest template
            if (sharedInstance._templateQueue.Count >= maxTemplateCount)
            {
                // current user's templates
                List<GameObject> _currentTemplates = new List<GameObject>(sharedInstance._currentTemplateForUserDict.Values);

                // enumerate queues
                for (int i = 0; i < sharedInstance._templateQueue.Count; i++)
                {
                    GameObject t = sharedInstance._templateQueue[i];

                    // skip when enumerated template is recent of any users.
                    if (!_currentTemplates.Contains(t))
                    {
                        sharedInstance._templateQueue.RemoveAt(i);
                        Destroy(t.gameObject);
                        break;
                    }
                }
            }

            // clean main texture
            newTemplate.CleanMainTexture();

            // set references!
            sharedInstance._currentTemplateForUserDict[newTemplate.userId] = newTemplate.gameObject;
            sharedInstance._templateQueue.Add(newTemplate.gameObject);

            // return new template object
            return newTemplate;
        }
        #endregion
    }
}
