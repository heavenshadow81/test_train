using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ML.T_Sports.DodgeBall
{
    using Common;

    /// <summary>
    /// 피구 게임 관리자
    /// </summary>
    public class DodgeBallGameManager : ContentsManagerBase
    {
        // public variables
        public DodgeBallPlayer[] prefabs;
        public Transform boidTarget;
        public DodgeBallShooter shooter;
        public AudioSource bgm;

        // game states
        public int numShootBalls = 0;
        public int score = 0;
        public float time = 0;

        // object pools
        private DodgeBallPlayer[] _players = new DodgeBallPlayer[10];
        public Vector3 lastHitPoint = Vector3.zero;

        // events
        public event Action onScore, onFail, onReset, onShoot;
        public event Action<bool> onPause;

        public override void Init()
        {
            // 인원 활성화
            InitProperty(ContentsPropertyType.Player, 1, 10, 10);

            // 오브젝트 생성
            _SpawnBoids();

            // BGM/SFX
            bgm.volume = GetSharedPropertyValue(ContentsPropertyType.BGM);

            // 기타
            shooter.enabled = false;
        }

        public override void Play()
        {
            if (!IsPlaying)
            {
                shooter.enabled = true;
                numShootBalls = 0;
                score = 0;
                time = 0;

                for (int i = 0; i < GetPropertyValueInt(ContentsPropertyType.Player); i++)
                {
                    if (_players[i] != null)
                        _players[i].Revive();
                }

                base.Play();
            }
        }

        public void Update()
        {
            if (IsPlaying)
                time += Time.deltaTime;
        }

        public override void Pause()
        {
            base.Pause();
            if (onPause != null)
                onPause(IsPaused);
        }

        public override void Stop()
        {
            if (IsPlaying)
            {
                base.Stop();
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.Player:
                    _SpawnBoids();
                    break;
            }
        }

        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch(type)
            {
                case ContentsPropertyType.BGM:
                    bgm.volume = newValue;
                    break;
                case ContentsPropertyType.SFX:
                    //SoundManager.instance.SetEFMVolume(Mathf.FloorToInt(newValue * 10));
                    break;
            }
        }

        private void _SpawnBoids()
        {
            int player = GetPropertyValueInt(ContentsPropertyType.Player);
            for (int i = 0; i < player; i++)
            {
                if (_players[i] == null)
                {
                    _players[i] = Instantiate(prefabs[i % prefabs.Length]);
                    _players[i].transform.parent = transform;
                    _players[i].gameObject.SetActive(false);
                }

                if (!_players[i].gameObject.activeSelf)
                {
                    _players[i].gameObject.SetActive(true);
                    _players[i].GetComponent<DodgeBallBoid>().target = boidTarget;

                    Vector3 pos = boidTarget.position;
                    pos.y = 0;
                    pos += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * 5.0f;
                    _players[i].transform.position = pos;

                    Vector3 camPos = Camera.main.transform.position;
                    camPos.y = _players[i].transform.position.y;
                    _players[i].transform.rotation = Quaternion.LookRotation((camPos - pos).normalized, Vector3.up);
                }

                _players[i].Revive();
            }

            for (int i = player; i < _players.Length; i++)
            {
                if (_players[i] != null)
                    _players[i].gameObject.SetActive(false);
            }

            if (onReset != null)
                onReset();
        }

        public void AddScore(int append, Vector3 hit = default(Vector3))
        {
            score += append;
            if (append > 0)
            {
                lastHitPoint = hit;

                if (onScore != null)
                    onScore();

                if (score >= GetPropertyValueInt(ContentsPropertyType.Player))
                    Stop();
            }
            else
            {
                if (onFail != null)
                    onFail();
            }
        }

        public void OnShoot()
        {
            numShootBalls++;
            if (onShoot != null)
                onShoot();
        }
    }
}