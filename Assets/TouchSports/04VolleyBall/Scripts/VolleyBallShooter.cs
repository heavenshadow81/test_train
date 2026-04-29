using UnityEngine;

namespace ML.T_Sports.VolleyBall
{
    /// <summary>
    /// 배구공을 쏘는 클래스
    /// </summary>
    public class VolleyBallShooter : MonoBehaviour
    {
        public GameObject ballPrefab;
        public float cooldown = 1.0f;
        public Vector2 timeRange = new Vector2(0.25f, 0.8f);
        public AudioClip shootClip;

        private float _cooldown = 0.0f;

        public void Update()
        {
            if (Common.ContentsManagerBase.Current != null && Common.ContentsManagerBase.Current.IsPlaying)
            {
                // 터치 이벤트
                for (int i = 0; i < TouchModule.TouchModuleInput.touchCount; i++)
                {
                    if (TouchModule.TouchModuleInput.GetTouch(i).phase == TouchPhase.Began)
                    {
                        _Check(TouchModule.TouchModuleInput.GetTouch(i).position);
                    }
                }

                _cooldown += Time.deltaTime;
            }
            else
            {
                _cooldown = cooldown;
            }
        }

        private void _Check(Vector3 point)
        {
            // 대기 시간 검사
            if (_cooldown >= cooldown)
            {
                _cooldown = 0;

                // 스크린 영역 검사
                if (point.x >= 0 && point.x <= Screen.width && point.y >= 0 && point.y <= Screen.height)
                {
                    float time = Random.Range(timeRange.x, timeRange.y);

                    point.z = -Camera.main.transform.position.z + Random.Range(0.0f, 2.0f);
                    Vector3 to = Camera.main.ScreenToWorldPoint(point);

                    Vector3 from = transform.position;
                    from.y = Random.Range(2.0f, 3.0f);
                    if(point.y >= Screen.height * 0.38f && point.y <= Screen.height * 0.56f)
                    {
                        from.y = Random.Range(2.8f, 3.0f);
                        time = Random.Range(timeRange.x, Mathf.Lerp(timeRange.x, timeRange.y, Mathf.Pow(Random.Range(0.0f, 1.0f), 5.0f) * 0.5f));
                    }
                    transform.position = from;

                    Shoot(to, time);
                }
            }
        }

        public void Shoot(Vector3 to, float time)
        {
            Vector3 offset = to - transform.position;

            float g = -Physics.gravity.magnitude;
            float h = offset.y;
            float v_x = offset.x / time;
            float v_y = h / time - 0.5f * g * time;
            float v_z = offset.z / time;

            GameObject ball = Instantiate(ballPrefab);
            ball.transform.position = transform.position;

            Rigidbody rigid = ball.GetComponent<Rigidbody>();
            if (rigid == null) rigid = ball.AddComponent<Rigidbody>();
            rigid.AddForce(new Vector3(v_x, v_y, v_z), ForceMode.VelocityChange);
            rigid.AddTorque(Vector3.left * 10, ForceMode.VelocityChange);

            AudioSource.PlayClipAtPoint(shootClip, Camera.main.transform.position, Common.ContentsManagerBase.Current.GetSharedPropertyValue(Common.ContentsPropertyType.SFX));
            VolleyBallGameManager.instance.OnShoot();
        }
    }
}