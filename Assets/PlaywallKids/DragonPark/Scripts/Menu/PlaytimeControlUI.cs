using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This class controls the playtime UI and always laid out on the top.
/// </summary>
public class PlaytimeControlUI : MonoBehaviour
{
    #region Public variables
    // time selection
    public GameObject timeSelectionPanel;
    public GameObject timeCounterPanel;
    public GameObject timeList;
    public GameObject timeSelector;

    // counter
    public Graphic clock;
    public Text counterRemainedTimeText;
    public AudioSource endAlarmAudio;
    #endregion

    #region Private variables
    private float _time = 300.0f;
    private float _remainedTime = 0.0f;

    private IEnumerator _counterCoroutine = null;
    #endregion

    #region Methods
	void Start ()
    {
        // This object should be alive always on the top
        DontDestroyOnLoad(gameObject);

        // Show time selection panel default.
        ShowTimeSelectionPanel();
	}

    private void _SelectTime(GameObject sender, float time)
    {
        if (time < 10.0f) return;
        _time = time;

        // Hide the time list
        timeList.SetActive(false);

        if (sender != null)
        {
            // Change the selector's text as selected time's text value if the text of the sender is exist
            Text selectorText = timeSelector.GetComponentInChildren<Text>();
            Text selectedText = sender.GetComponentInChildren<Text>();
            if (selectedText == null) selectedText = sender.GetComponent<Text>();

            if (selectorText != null && selectedText != null)
                selectorText.text = selectedText.text;
        }
    }
    #endregion

    #region Actions
    public void ShowTimeSelectionPanel()
    {
        // Initialize game objects
        timeSelectionPanel.SetActive(true);
        timeCounterPanel.SetActive(false);
        timeList.SetActive(false);

        // Stops the counter coroutine if it's running
        if (_counterCoroutine != null)
        {
            StopCoroutine(_counterCoroutine);
            _counterCoroutine = null;
        }

        // Reset releated objets
        clock.color = Color.white;
        clock.rectTransform.localRotation = Quaternion.identity;

        if (endAlarmAudio != null)
        {
            endAlarmAudio.Stop();
            endAlarmAudio.enabled = false;
        }
    }

    public void ShowTimeList()
    {
        timeList.SetActive(!timeList.activeSelf);
    }

    // 5 min
    public void SelectTime300(GameObject sender)
    {
        _SelectTime(sender, 10.0f);
    }

    // 10 min
    public void SelectTime600(GameObject sender)
    {
        _SelectTime(sender, 600.0f);
    }

    // 15 min
    public void SelectTime900(GameObject sender)
    {
        _SelectTime(sender, 900.0f);
    }

    // 20 min
    public void SelectTime1200(GameObject sender)
    {
        _SelectTime(sender, 1200.0f);
    }

    // Start counting
    public void StartCounter()
    {
        if (_counterCoroutine == null)
        {
            _counterCoroutine = _PerformStartCounter();
            StartCoroutine(_counterCoroutine);
        }
    }

    private IEnumerator _PerformStartCounter()
    {
        _remainedTime = _time;

        timeSelectionPanel.SetActive(false);
        timeCounterPanel.SetActive(true);

        // Start!
        float clockRotZ = 0.0f;
        float clockRotSpeed = 10.0f;
        bool isRotatingCW = false;

        while (_remainedTime > 0.0f)
        {
            _remainedTime -= Time.deltaTime;

            // Gets the seconds and minutes from remained time
            int second = Mathf.CeilToInt(_remainedTime);
            int minute = second / 60;
            second %= 60;

            // Shows the remained time
            if (_remainedTime % 1.0f > 0.5f)
                counterRemainedTimeText.text = string.Format("{0:00}:{1:00}", minute, second);
            else
                counterRemainedTimeText.text = string.Format("{0:00} {1:00}", minute, second);

            // Rotate the clock graphic (image).
            clockRotZ += Time.deltaTime * clockRotSpeed * (isRotatingCW ? 1.0f : -1.0f);
            if (clockRotZ >= 10.0f)
            {
                clockRotZ = 10.0f - (clockRotZ - 10.0f);
                isRotatingCW = false;
            }
            else if (clockRotZ <= -10.0f)
            {
                clockRotZ = -10.0f - (clockRotZ + 10.0f);
                isRotatingCW = true;
            }
            clock.rectTransform.localRotation = Quaternion.Euler(0, 0, clockRotZ);

            yield return null;
        }

        // End
        if (endAlarmAudio != null)
        {
            endAlarmAudio.enabled = true;
            endAlarmAudio.loop = true;
            endAlarmAudio.Play();
        }

        while (true)
        {
            clock.color = new Color(1.0f, 0.5f, 0.5f);
            clock.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(-30.0f, 30.0f));

            yield return new WaitForSeconds(0.15f);
        }
    }
    #endregion
}
