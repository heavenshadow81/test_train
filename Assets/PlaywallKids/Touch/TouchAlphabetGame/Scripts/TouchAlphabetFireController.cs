using UnityEngine;
using System.Collections;

public class TouchAlphabetFireController : MonoBehaviour {
    public TouchCameraManager camController;
    public Transform[] bulletSpawners;
    public GameObject bulletPrefab;
    public AudioClip sndLaser;
    CObjectList<GameObject> bulletList;
    Transform cachedTransform;

	float f_waitTime;
	float f_triggerTime;

    void Awake()
    {
        bulletList = new CObjectList<GameObject>(10,
            () =>
            {
                GameObject bullet  =  Instantiate(bulletPrefab) as GameObject;
                bullet.SetActive(false);
                return bullet;
            },
            (GameObject bullet) => {
                if (bullet == null) return false;
                return !bullet.activeInHierarchy; 
            }
            );
        cachedTransform = this.transform;
		f_waitTime = 5f;
    }

	void TouchInput()
	{
		for (int i = 0; i < CustomInput.touchCount; i++)
		{
			TouchInfo touch = CustomInput.GetTouch(i);
			
			if (touch.phase == TouchInfo.Phase.Begin)
			{
				Vector2 inputPos = camController.lensCalibration.GetDistoredScreenPosFromOriginal(touch.position);
				Ray ray = camController.gameCamera.ScreenPointToRay(inputPos);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 200f, 0x01<< ML.PlaywallKids.Common.LayerConstants.INTERACTION_OBJECT))
				{
					Vector3 dir = hitInfo.point - cachedTransform.position;
					dir.Normalize();
					for (int cnt = 0; cnt < bulletSpawners.Length; ++cnt)
					{
						GameObject bullet = bulletList.GetObject();
						bullet.SetActive(true);
						bullet.transform.position = bulletSpawners[cnt].position;
						bullet.transform.LookAt(hitInfo.collider.transform);
						BulletScript temp = bullet.AddComponent<BulletScript>();
						temp.dir = dir;
						temp.speed = 3f;
						if (sndLaser != null)
						{ AudioSource.PlayClipAtPoint(sndLaser, Vector3.zero); }
					}
				}
			}
		}
	}

	void AutoFire()
	{
		f_triggerTime += Time.deltaTime;
		if(f_triggerTime > f_waitTime)
		{
			f_triggerTime = 0;
			f_waitTime = Random.Range(0.2f, 0.5f);
			for (int cnt = 0; cnt < bulletSpawners.Length; ++cnt)
			{
				Vector3 target = new Vector3(Random.Range(0.3f + cnt * 0.2f,0.5f + cnt * 0.2f), Random.Range(0.2f,0.6f), 30f);
				target = camController.gameCamera.ViewportToWorldPoint(target);
			
				GameObject bullet = bulletList.GetObject();
				bullet.SetActive(true);
				bullet.transform.position = bulletSpawners[cnt].position;
				//bullet.transform.localRotation = bulletSpawners[cnt].localRotation;
				//bullet.transform.parent = this.transform;
                BulletScript temp = bullet.GetComponent<BulletScript>();
                if (temp  == null)
                { temp = bullet.AddComponent<BulletScript>(); }
				temp.usePosition = true;
				temp.dir = (target - cachedTransform.position).normalized;
				temp.speed = 300f;

				/*
				BezierMove bezier = bullet.AddComponent<BezierMove>();
				bezier.disableType = BezierMove.EType.DISABLE;
				bezier.usePosition = true;
				bezier.fSpeed = 0.12f;
				bezier.wayPoint0 = new Vector3( Random.Range(-3f,3f), Random.Range(-3f,3f), -3f);
				bezier.target = target;
*/
			}
			
			if (sndLaser != null)
			{ AudioSource.PlayClipAtPoint(sndLaser, Vector3.zero); }
		}
	}
         
    void LateUpdate()
    {
        if (camController == null) return;

		AutoFire();
    }

}
