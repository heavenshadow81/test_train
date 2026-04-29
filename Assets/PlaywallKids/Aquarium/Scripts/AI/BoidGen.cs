using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public class BoidGen : MonoBehaviour
    {
        public Boid agent;
        public int count = 1000;
        public GameObject target1, target2;

        void Start()
        {
            if (QualitySettings.GetQualityLevel() < 3)
                count /= 2;

            for(int i = 0; i < count-1; i++)
            {
                Boid b = Instantiate(agent, transform, true);
                b.name = agent.name;
                b.separationFactor = Random.Range(0.75f, 1.5f) * b.separationFactor;
                b.alignmentFactor = Random.Range(0.25f, 1.0f) * b.alignmentFactor;
                b.cohesionFactor = Random.Range(0.25f, 1.0f) * b.cohesionFactor;
                b.maxSpeed = Random.Range(0.7f, 1.0f) * b.maxSpeed;
                b.target = i < (count / 2) ? target1.transform : target2.transform;
            }
            iTween.MoveTo(target1, iTween.Hash("path", iTweenPath.GetPath("F1"), "time", 90,
                "orienttopath", true, "easetype", "linear", "looptype", "loop"));
            iTween.MoveTo(target2, iTween.Hash("path", iTweenPath.GetPath("F2"), "time", 90,
                "orienttopath", true, "easetype", "linear", "looptype", "loop"));
        }
    }
}