using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSourceContainer : MonoBehaviour
{
    static CharacterSourceContainer _instance;

    public static CharacterSourceContainer Instance { get => _instance; }

    [System.Serializable]
    public class Character
    {
        [Tooltip("캐릭터 이름")]
        public string CharacterName;

        [Tooltip("캐릭터 오브젝트 프래펩")]
        public GameObject CharacterPrefabs;

        [Tooltip("캐릭터 오브젝트 메쉬")]
        public Mesh CharacterMeshs;

        [Tooltip("캐릭터 매테리얼")]
        public Material[] materials;
    }

    [System.Serializable]
    public class TilePosition
    {
        [Tooltip("Row Number")]
        public int rowNum;

        [Tooltip("맵타일 프래펩")]
        public GameObject[] tilePrefabs;

        [Tooltip("맵타일 트랜스폼")]
        public Vector3[] tilePositions;
    }

    [System.Serializable]
    public class mapList
    {
        public TilePosition[] map;
    }

    [Tooltip("플레이어 캐릭터 오브젝트")]
    public GameObject [] playerObject;

    [Tooltip("GameScene 플레이어 시작 위치")]
    public GameObject [] playerStartPosition;

    [Tooltip("TTS 사운드")]
    public AudioClip[] buttonTTS;

    // 캐릭터 이름, 프래펩, 매테리얼 가지고 있는 파라미터
    public Character[] characters;

    // 현재 난이도에 해당하는 맵
    public TilePosition[] tiles;

    // easy,normal emptymap 값을 들고있음
    public mapList[] easyMaps, normalMaps;

    public int[] startRows, startColumns;

    public Material[] FaceMaterials;

    public GameObject[] monsters;

    public int totalRow, totalColumn;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }
    }

    void Start()
    {
        // 플레이어 캐릭터 오브젝트 배열 크기 = 플레이 인원 수
        playerObject = new GameObject[TotalParameter.Instance.persons];
    }

    // 플레이인원, 난이도에 따라 불러오는 맵 할당
    public void Tiles()
    {
        // 플레이 인원 1명
        if (TotalParameter.Instance.persons == 1)
        {
            // easy 모드- easyEmptyMap 할당
            if (ContentsOptions.GetDifficult() == Difficult.Easy)
            {
                print("1인 Easy");
                tiles = easyMaps[0].map;

                totalRow = easyMaps[0].map.Length;
                totalColumn = tiles[0].tilePrefabs.Length;

                for (int i = 0; i < easyMaps[0].map.Length; i++)
                {
                    for (int j = 0; j < tiles[i].tilePrefabs.Length; j++)
                    {
                        tiles[i].tilePositions[j] = tiles[i].tilePrefabs[j].transform.position;
                    }
                }

            }
            // normal 모드 - - NormalEmptyMap 할당
            else if (ContentsOptions.GetDifficult() == Difficult.Normal)
            {
                print("1인 Normal");
                tiles = normalMaps[0].map;

                totalRow = normalMaps[0].map.Length;
                totalColumn = tiles[0].tilePrefabs.Length;

                for (int i = 0; i < normalMaps[0].map.Length; i++)
                {
                    for (int j = 0; j < tiles[i].tilePrefabs.Length; j++)
                    {
                        tiles[i].tilePositions[j] = tiles[i].tilePrefabs[j].transform.position;
                    }
                }
            }
        }
        // 플레이 인원 2명 - NormalEmptyMap 할당
        else if(TotalParameter.Instance.persons == 2)
        {
            print("2인 Normal");
            tiles = normalMaps[0].map;

            totalRow = normalMaps[0].map.Length;
            totalColumn = tiles[0].tilePrefabs.Length;

            for (int i = 0; i < normalMaps[0].map.Length; i++)
            {
                for (int j = 0; j < tiles[i].tilePrefabs.Length; j++)
                {
                    tiles[i].tilePositions[j] = tiles[i].tilePrefabs[j].transform.position;
                }
            }
        }
    }
}
