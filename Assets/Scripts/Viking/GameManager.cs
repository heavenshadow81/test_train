using UnityEngine;
using UnityEngine.SceneManagement;
using static Settings;

public class GameManager : MonoBehaviour
{
    public GameObject Viking;
    public GameObject gameStartImg;
    public GameObject gameOverImg;


    public ZoZoBasePatton<GameManager> zozo;
    public EnumClass stateClass;
    public GameUI gameUI;
    public ScreenProsess screenProsess;

    public CharacterMoving characterMoving;
    private void Awake()
    {
        stateClass = new EnumClass();
        #region 공용 스테이트 패턴 

        ActionProcess.Enter_StateListener(null, null, null, null);

        zozo = new ZoZoBasePatton<GameManager>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
        #endregion
    }

    
    void Update()
    {
        if (zozo != null) zozo.MGR.Excute(() =>
        {
            if (Mathf.Abs(Viking.transform.rotation.eulerAngles.z - 180) < 1)
            {
                //gameOverImg.SetActive(true); 
                stateClass.resultState = GameResult.Fail;
                zozo.Change(GameState.GameResult);
            }

            characterMoving.Logic();

        });

    }
    public void Restart()
    { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    
    public void Quit()
    { Application.Quit(); }
    public void GoStart()
    { gameStartImg.SetActive(false); }
}
