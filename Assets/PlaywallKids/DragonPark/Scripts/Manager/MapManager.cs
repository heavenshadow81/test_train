using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// [Legacy] 맵(장면) 전환 스크립트
/// </summary>
public class MapManager : MonoBehaviour
{
    private static GameObject _dragonPark;
    /// <summary>
    /// Gets the current "DragonPark" game object.
    /// This is the default map.
    /// </summary>
    public static GameObject dragonPark
    {
        get
        {
            if (_dragonPark == null)
            {
                _dragonPark = GameObject.Find("Map_DragonPark");
            }
            return _dragonPark;
        }
    }

    private static GameObject _findPet;
    /// <summary>
    /// Gets the "FindPet" map used in motion game.
    /// </summary>
    public static GameObject findPet
    {
        get
        {
            if (_findPet == null)
            {
                _findPet = GameObject.Find("Map_FindPet");
            }
            return _findPet;
        }
    }

    public static IEnumerator LoadFindPet()
    {
        AsyncOperation ar = SceneManager.LoadSceneAsync("FindPet", LoadSceneMode.Additive);
        yield return ar;
    }

    public static IEnumerator LoadIntro()
    {
        AsyncOperation ar = SceneManager.LoadSceneAsync("SpaceIntro", LoadSceneMode.Additive);
        yield return ar;
    }
}
