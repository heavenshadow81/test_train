using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KartRider
{
    public class MapCloser : MonoBehaviour
    {
        [SerializeField] string map;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("KartBody"))
            {
                MapClose(map);
                gameObject.SetActive(false);

                //print("∏ ªË¡¶");
                
            }   
        }

        public void MapClose(string mapName)
        {
            Scene scene = SceneManager.GetSceneByName(mapName);

            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(mapName);          
            }   
        }
    }
    

}
