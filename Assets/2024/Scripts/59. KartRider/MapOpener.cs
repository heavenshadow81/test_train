using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KartRider
{
    public class MapOpener : MonoBehaviour
    {
        [SerializeField] string map;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("KartBody"))
            {
                MapOpen(map);
                gameObject.SetActive(false);
                //print("¸Ê¿ÀÇÂ");
            }
        }

        public void MapOpen(string mapName)
        {
            SceneManager.LoadSceneAsync(mapName,LoadSceneMode.Additive);           
        }
    }
    

}
