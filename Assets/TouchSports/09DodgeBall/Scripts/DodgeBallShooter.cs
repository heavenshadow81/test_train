using UnityEngine;

namespace ML.T_Sports.DodgeBall
{
    /// <summary>
    /// 배구공을 쏘는 클래스
    /// </summary>
    public class DodgeBallShooter : MonoBehaviour
    {
        public GameObject ballPrefab;
        public float cooldown = 1.0f;
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
                    point.z = 4.0f;
                    Ray ray = Camera.main.ScreenPointToRay(point);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        // 거리에 따라 날라가는 속도가 어느정도 비례하게...
                        Shoot(hitInfo.point, Mathf.Max(0.333f, (hitInfo.point - transform.position).magnitude / 70 + Random.Range(-0.25f, 0.25f)));
                    }
                    else
                    {
                        // 지면과 충돌되지 않으면 공을 아주 멀리 날려버리기
                        point.z = 60;
                        var worldPos = Camera.main.ScreenToWorldPoint(point);
                        Shoot(worldPos, Mathf.Max(0.333f, (worldPos - transform.position).magnitude / 70 + Random.Range(-0.25f, 0.25f)));
                    }
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
            ((DodgeBallGameManager)Common.ContentsManagerBase.Current).OnShoot();
        }
    }
}