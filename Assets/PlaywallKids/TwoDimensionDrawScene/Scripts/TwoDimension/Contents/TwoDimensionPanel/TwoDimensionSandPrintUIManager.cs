using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionSandPrintUIManager : MonoBehaviour
    {
        public CircleGauge gaugePrefab;

        Dictionary<int, CircleGauge> gaugeDic;
        ListPool<CircleGauge> gaugeList;
        Transform cachedTransform;

        void Awake()
        {
            cachedTransform = this.transform;
        }

        void OnEnable()
        {
            gaugeList = new ListPool<CircleGauge>(gaugePrefab);
            gaugeDic = new Dictionary<int, CircleGauge>();
        }

        void OnDisable()
        {
            gaugeList.Dispose();
            gaugeDic.Clear();

            gaugeList = null;
            gaugeDic = null;
        }

        public float GetValue(int _id)
        {
            if (!gaugeDic.ContainsKey(_id)) return 0f;
            return gaugeDic[_id].foreValue;
        }

        public void DisplayGauge(int _id, Vector3 _screenPos, float _value, Transform _parent = null)
        {
            CircleGauge _gauge = null;
            if (gaugeDic.ContainsKey(_id))
            {
                _gauge = gaugeDic[_id];
            }
            else
            {
                _gauge = gaugeList.GetComponent;
                _gauge.cachedTransform.parent = _parent == null ? this.transform : _parent;
                _gauge.cachedTransform.localScale = Vector3.one;
                _gauge.cachedTransform.localRotation = Quaternion.identity;
                _gauge.gameObject.SetActive(true);
                gaugeDic.Add(_id, _gauge);
            }
            _gauge.foreValue += _value;
            _gauge.cachedTransform.localPosition = _screenPos;
        }

        public void DisableGauge(int _id)
        {
            gaugeDic[_id].gameObject.SetActive(false);
            gaugeDic.Remove(_id);
        }
    }
}