using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.SportsMiniGame.KinectSkating;
public class ExplosionIce : MonoBehaviour {
    public Rigidbody[] MyIceRig;
    public Transform[] MyIceTr;
    public GameObject[] MyIceobj;

    public EFMsPlayer[] efms;
    public Transform MyTr;
    public Vector3[] Vect;

    public Vector3[] StartPositoins;
    public QuizZoneGUI quizone;
    public float power;
    // Use this for initialization
    void Start ()
    {
        InitValues();
    }
    public void InitValues()
    {
        Vect = new Vector3[MyIceTr.Length];
        StartPositoins = new Vector3[MyIceTr.Length];
        MyIceRig = new Rigidbody[MyIceTr.Length];
        MyIceobj = new GameObject[MyIceTr.Length];
        efms = new EFMsPlayer[MyIceTr.Length];
        MyTr = this.transform;
        for (int i = 0; i < MyIceTr.Length; i++)
        {
            Vect[i] = (MyTr.position + MyIceTr[i].position) / 2;
            StartPositoins[i] = MyIceTr[i].position;
            MyIceRig[i] = MyIceTr[i].GetComponent<Rigidbody>();
            efms[i] = MyIceTr[i].GetComponent<EFMsPlayer>();
            MyIceobj[i] = MyIceTr[i].gameObject;
        }
    }
    public void Explosion()
    {
        quizone.HolographicsAnimation.Play("Holographic_Reset");
        for (int i = 0; i < MyIceTr.Length; i++)
        {
            MyIceRig[i].AddForce(new Vector3(Random.Range(-1f,1f), Random.Range(-0.5f, 0.5f), Random.Range(1f, 2f)) * power);
            efms[i].SetVolume(Random.Range(0.3f, 0.7f));
            efms[i].PlayRandomClips();
        }
        Invoke("ResetObj", 3);
    }
    public void ResetObj()
    {
        for (int i = 0; i < MyIceTr.Length; i++)
        {
            MyIceRig[i].isKinematic = true;
            MyIceRig[i].useGravity = false;
            MyIceobj[i].SetActive(false);
            MyIceTr[i].position = StartPositoins[i];
            MyIceTr[i].rotation = Quaternion.identity;
        }
        for (int i = 0; i < MyIceTr.Length; i++)
        {
            MyIceRig[i].isKinematic = false;
            MyIceobj[i].SetActive(true);
            MyIceRig[i].useGravity = true;
        }
    }
}
