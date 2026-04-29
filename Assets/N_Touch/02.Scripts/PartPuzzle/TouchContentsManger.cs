using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//터치 콘텐츠 범주

//터치 콘텐츠
public enum TouchAnimal
{
	Mammal,
	Insect,
	Dino,
	Sea
}
//터치 뭐니?
public enum TouchType
{
	Part,
	Shadow,
}

public class TouchContentsManger : MonoBehaviour
{
	#region 변수 및 기타 객체
	//배경음
	[SerializeField]
	AudioSource BackgroundSound, effectSound;
	//난이도
	public TouchDifficulty Difficulty = TouchDifficulty.Easy;
	//사람 숫자
	[SerializeField]
	public int Players = 2;
	//시간
	[SerializeField]
	float Time;

	//난이도 선택에 따른 설정
	[SerializeField]
	GameObject[] difficulties;
	//플레이어 UI들
	List<PlayerManager> playerUIs = new List<PlayerManager>();


	//몇번째 퍼즐조각?
	public IDictionary<int, bool> order = new Dictionary<int, bool>();
	//최대 시간
	//싱글톤
	static TouchContentsManger _instance;
	//싱글톤
	public static TouchContentsManger Instance
	{
		get { return _instance; }
	}
	public TouchAnimal contents;
	//플레이중
	public bool Playing { get; set; }
	//터치 타입
	public TouchType contentsType;

	//동물 퍼즐 조각에 사용할 동물 배열 > 나타나는 순서 랜덤으로 지정하기 위한 것+플레이어 공통으로 쓰기 위한 것
	public int[] shufflepart;
	//결과 동그라미 가새표..
	public Sprite[] resultoxImage;

	//그림자 퍼즐에 사용할 그림Image
	public Sprite[] shadowmammalImages, shadowoceanImages, shadowInsectImages, shadowdinoImages;
	//소리 재생에 사용할 것
	[SerializeField]
	AudioClip[] animalaudioClip, insectaudioClip, dinoaudioClip, oceanaudioClip, truefalseClip;

	[SerializeField]
	string[] animalName, insectName, dinoName, oceanName;

	#endregion
	#region 이벤트
	//이벤트 설정
	event System.EventHandler SetOperator;
	
	//이벤트 실행 함수
	void Operate()
	{
		SetOperator?.Invoke(this, System.EventArgs.Empty);
	}
	#endregion

	#region 함수들

	#region 공통
	void Awake()
	{
		//싱글톤 객체 설정
		if (!_instance)
		{
			_instance = this;
		}
		//이벤트 설정
		switch (contentsType)
		{
			case TouchType.Part:
				SetOperator += ExchangeImage;

				break;
			case TouchType.Shadow:
				SetOperator += ShadowSet;

				break;
		}
		
	}
	//옵션 창: 취소바로 누르면 아무것도 뜨지 않는 것 방지
	void Start()
    {

		//초기화
		Init();
		//플레이어 UI 창 비활성화
		difficulties[0].transform.parent.gameObject.SetActive(false);

        int select = (int)Difficulty;
        print(difficulties[select].transform.GetChild(0).gameObject);
        print(difficulties[(int)Difficulty].transform.childCount);
    }


	//플레이어 설정
	public void PlayerSet()
	{
		//플레이어들(난이도에 따라 달라지므로 매번 시작시마다 지워주자) 
		playerUIs.Clear();
		//난이도에 해당하는 플레이어들 집어넣기
		for (int i = 0; i < difficulties[(int)Difficulty].transform.childCount; i++)
		{
			playerUIs.Add(difficulties[(int)Difficulty].transform.GetChild(i).GetComponent<PlayerManager>());
			print(playerUIs);
		}
	}

	//어려운 정도 설정
	public void Select_Difficulty(int difficulty)
	{
		//
		switch (difficulty)
		{
			case 0:
				Difficulty = TouchDifficulty.Easy;
				break;
			case 1:
				Difficulty = TouchDifficulty.Normal;
				break;
			case 2:
				Difficulty = TouchDifficulty.Hard;
				break;
		}
		//그 이전에 활성화된 것 있으면 다 비활성화하고
		for (int i = 0; i < difficulties.Length; i++)
		{
			difficulties[i].SetActive(false);
		}
		//해당 난이도에 해당하는 부분만 활성화
		difficulties[difficulty].SetActive(true);
	}
	//사람 숫자 조절
	public void Select_Players(int number)
	{

		int select = (int)Difficulty;

		//오브젝트 활성화
		for (int i = 0; i < difficulties[select].transform.childCount; i++)
		{
			difficulties[select].transform.GetChild(i).gameObject.SetActive(i<Players);

		}
	}

	//옵션 창에서 설정
	public void OptionDifficult(int touch)
	{
		Difficulty = (TouchDifficulty)touch;
	}
	//옵션 창에서 설정
	public void OptionPlayer(int number)
	{
		Players = number;
	}
	//드롭다운 설정
	public void DropdownDifficult(Dropdown dropdown)
	{
		Difficulty = (TouchDifficulty)dropdown.value;
	}
	public void DropdownPlayer(Dropdown dropdown)
	{
		Players = dropdown.value + 1;
	}
    //소리 설정
    public AudioClip SetTFClip(bool state)
    {
        AudioClip clip = (state) ? truefalseClip[0] : truefalseClip[1];
        return clip;
    }
    //콘텐츠 초기화
    public void Init()
    {
		
		//시간을 초기화하고
		Time = 0;
		//난이도에 맞게..!
		Select_Difficulty((int)Difficulty);
		//그리고 사람 설정하기
		Select_Players(Players);
		//플레이어들 ...
		PlayerSet();
		//그림 바꾸기
		Operate();
		order.Clear();
	}
	//배열 섞기
	T[] ShuffleArray<T>(T[] array, int seed)
	{
		//랜덤한 값 얻기....
		System.Random prng = new System.Random(seed);

		for (int i = 0; i < array.Length - 1; i++)
		{
			int randomIndex = prng.Next(i, array.Length);
			T tempItem = array[randomIndex];
			array[randomIndex] = array[i];
			array[i] = tempItem;
		}

		return array;
	}

	//오디오 설정 하고 재생하기까지_동물 이름...!
	public void AudioPlay(int audioindex)
    {
        switch (contents)
        {
			case TouchAnimal.Mammal:
				effectSound.clip = animalaudioClip[audioindex];
				break;
			case TouchAnimal.Insect:
				effectSound.clip = insectaudioClip[audioindex];
				break;
			case TouchAnimal.Dino:
				effectSound.clip = dinoaudioClip[audioindex];
				break;
			case TouchAnimal.Sea:
				effectSound.clip = oceanaudioClip[audioindex];
				break;
        }
		effectSound.Play();
    }
	//맞춤/틀림할 때 바꿀 클립
	public AudioClip CorrectWrong(bool state)
    {
		AudioClip clip = (state) ? truefalseClip[0] : truefalseClip[1];
		return clip;
    }
	//동물 이름지정!
	public string AnimalNameSet(int animalindex)
    {
		string tempname = "";
        switch (contents)
        {
			case TouchAnimal.Mammal:
				tempname = animalName[animalindex];
				break;
			case TouchAnimal.Insect:
				tempname = insectName[animalindex];
				break;
			case TouchAnimal.Dino:
				tempname = dinoName[animalindex];
				break;
			case TouchAnimal.Sea:
				tempname = oceanName[animalindex];
				break;
        }
		return tempname;
    }

	#endregion

	#region 부분 퍼즐

	//조각 콘텐츠 설정하기
	void ExchangeImage(object sender, System.EventArgs e)
    {
		
		//리스트 생성
		List<int> animalpart = new List<int>();
		//8마리 있음!> 나중에 여러 마리로 늘어나면....
		for (int i = 0; i < 8; i++)
		{
			animalpart.Add(i);
		}

		//섞어서 5마리만 검출하기
		//shufflepart = ShuffleArray(animalpart.ToArray(), Random.Range(0, 60)).Take(5).ToArray();
		shufflepart = ShuffleArray(animalpart.ToArray(), Random.Range(0, 60)).ToArray();

		for (int i = 0; i<playerUIs.Count; i++)
        {
			//각각 UI 초기화
			playerUIs[i].Init();
        }
    }
    #endregion

    #region 그림자 컨텐츠

    //그림자 컨텐츠 설정하기
    void ShadowSet(object sender, System.EventArgs e)
    {
		int diff = (int)Difficulty;
		

		for (int i = 0; i < playerUIs.Count; i++)
		{
			//각각 UI 초기화
			playerUIs[i].Init();
		}
	}
    #endregion

    #endregion
}
