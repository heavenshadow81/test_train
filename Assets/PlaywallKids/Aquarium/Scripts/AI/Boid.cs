using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    /// <summary>
    /// 아쿠아리움 군체 오브젝트 (boids)
    /// 기능 테스트중
    /// http://dogfeet.github.io/articles/2012/flocking-algorithm.html
    /// </summary>
    public class Boid : MonoBehaviour
    {
        // 종류
        public enum Type
        {
            Agent = 0,
            Obstacle = 1
        }
        public Type type;
        public Vector3 velocity;
        public float neighbourRadius = 5.0f;
        public float desiredSeparation = 1.0f;
        public float maxSpeed = 15.0f;
        public float radius = 1.0f;
        public Vector3 pos;
        public Vector3 acc;

        // 따라가야 할 대상을 지정한다. 없을 경우 가만히 있는다.
        public Transform target;

        /*
         * Boid는 Separation(분리), Alignment(정렬), Cohesion(응집) 동작으로 구성됨.
         * 각 계수를 조절하여 각 동작의 영향력을 조정한다.
         */
        public float separationFactor = 0.33f;
        public float alignmentFactor = 0.33f;
        public float cohesionFactor = 0.33f;

        /* Agent 목록 */
        public static List<Boid> list = new List<Boid>();
        public static List<Boid> obstacles = new List<Boid>();

        /* 
         * 모든 이웃을 검사하기엔 시간이 너무 오래 걸리므로 (time complexity가 O(n^2))
         * 임의로 정해진 갯수만큼 추출하여 검사가 필요함.
         */
        private int _maxNumberOfNeighbors = 16;

        private static List<Boid> _neighbors = new List<Boid>();
        private float _time = 0;

        public void OnEnable()
        {
            if (type == Type.Agent)
                list.Add(this);
            else
                obstacles.Add(this);

            pos = transform.position;

            // 군체의 업데이트 주기를 분산시키기 위해 적용한 trick.
            // 모든 군체가 일시에 업데이트함으로 인한 프레임 드랍 현상을 방지하기 위함.
            _time = 0.166f - Random.Range(0, 1.0f);
        }

        public void OnDisable()
        {
            list.Remove(this);
        }

        public void SetMaxNumberOfNeighbors(int newVal)
        {
            _maxNumberOfNeighbors = Mathf.Clamp(newVal, 16, 128);
        }

        public IEnumerator Interaction()
        {
            if (type == Type.Obstacle) yield break;
            type = Type.Obstacle;
            list.Remove(this);
            obstacles.Add(this);
            radius = radius * 2;
            yield return new WaitForSeconds(1.5f);
            type = Type.Agent;
            obstacles.Remove(this);
            radius = radius / 2;
            list.Add(this);
        }

        public void Update()
        {
            if (target != null)
                velocity = Vector3.Lerp(velocity, (target.transform.position - pos).normalized * maxSpeed, Time.deltaTime);

            // 일정한 주기가 되었을 때 군체 업데이트.
            _time += Time.deltaTime;
            if (_time >= 0.166f)
            {
                _time = 0;
                _neighbors.Clear();

                for (int i = 0; i < _maxNumberOfNeighbors; i++)
                    _neighbors.Add(list[Random.Range(0, list.Count)]);
                for (int i = 0; i < obstacles.Count; i++)
                    _neighbors.Add(obstacles[i]);

                Vector3 acceleration = Flock(_neighbors);
                acc = Vector3.Lerp(acc, acceleration, Time.deltaTime * 13.0f);
                if (type == Type.Obstacle) acc = Vector3.zero;
            }

            Vector3 delta = (velocity + acc) * Time.deltaTime;
            delta = Vector3.ClampMagnitude(delta, 2 * maxSpeed * Time.deltaTime);

            pos = transform.position;
            transform.position += delta;
            velocity += acc * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((delta + transform.forward * 0.0001f).normalized, Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * 6.5f)), Time.deltaTime * 6.5f);
        }

        public Vector3 Flock(List<Boid> neighbors)
        {
            if (type == Type.Agent)
            {
                Vector3 separation = Separate(neighbors) * separationFactor;
                Vector3 alignment = Align(neighbors) * alignmentFactor;
                Vector3 cohesion = Cohere(neighbors) * cohesionFactor;
                return separation + alignment + cohesion;
            }
            return Vector3.zero;
        }

        // 분리 (Separation)
        public Vector3 Separate(List<Boid> neighbors)
        {
            Vector3 mean = Vector3.zero;
            Vector3 mean2 = Vector3.zero;
            int num = 0, num2 = 0;

            for (int i = 0; i < neighbors.Count; i++)
            {
                Boid b = neighbors[i];
                if (b != this)
                {
                    Vector3 dir = (b.pos - pos);
                    float distance = dir.magnitude;
                    if (distance == 0)
                    {
                        mean += Random.insideUnitSphere;

                        if (b.type == Type.Agent)
                        {
                            mean += Random.insideUnitSphere;
                            num++;
                        }
                        else
                        {
                            mean2 += Random.insideUnitSphere * maxSpeed;
                            num2++;
                        }
                    }
                    else if (distance < desiredSeparation + b.radius)
                    {
                        Vector3 m = -dir;
                        float div = distance * Mathf.Max(0.1f, distance);
                        float mul = (b.radius * radius);
                        m *= mul / div;

                        if (b.type == Type.Agent)
                        {
                            mean += m;
                            num++;
                        }
                        else
                        {
                            mean2 += m;
                            num2++;
                        }
                    }
                }
            }

            if (num > 0)
                mean /= num;
            if (num2 > 0)
            {
                mean2 /= num2;
                mean2 = mean2.normalized * maxSpeed * 2.0f;
            }

            if (num2 > 0)
                return Vector3.Lerp(mean2, mean, num2 / (float)Mathf.Max(10, num));
            return mean;
        }

        public Vector3 Align(List<Boid> neighbors)
        {
            Vector3 mean = Vector3.zero;
            int num = 0;

            for (int i = 0; i < neighbors.Count; i++)
            {
                Boid b = neighbors[i];
                if (b != this && b.type == Type.Agent)
                {
                    Vector3 dir = (b.pos - pos);
                    if (dir.sqrMagnitude < neighbourRadius * neighbourRadius)
                    {
                        mean += b.velocity;
                        num++;
                    }
                }
            }
            if (num > 0)
                mean /= num;
            return mean;
        }

        public Vector3 Cohere(List<Boid> neighbors)
        {
            Vector3 sum = Vector3.zero;
            int num = 0;

            for (int i = 0; i < neighbors.Count; i++)
            {
                Boid b = neighbors[i];
                if (b != this && b.type == Type.Agent)
                {
                    Vector3 dir = (b.pos - pos);
                    if (dir.sqrMagnitude < neighbourRadius * neighbourRadius)
                    {
                        sum += b.pos;
                        num++;
                    }
                }
            }

            if (num > 0)
                return SteerTo(sum / num);
            return Vector3.zero;
        }

        public Vector3 SteerTo(Vector3 dest)
        {
            Vector3 desired = (dest - pos);
            float d = desired.magnitude;

            if (d > 0)
            {
                //desired.Normalize();
                Vector3 steer = desired - velocity;
                return steer;
            }

            return Vector3.zero;
        }
    }
}