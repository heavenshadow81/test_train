using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.T_Sports.DodgeBall
{
    public class DodgeBallBoid : MonoBehaviour
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
        public Vector2 boundingBoxOrigin, boundingBoxSize;

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
        public static List<DodgeBallBoid> list = new List<DodgeBallBoid>();
        public static List<DodgeBallBoid> obstacles = new List<DodgeBallBoid>();
        private static List<DodgeBallBoid> _neighbors = new List<DodgeBallBoid>();
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
            Vector3 targetPos = transform.position;
            if (target != null)
            {
                targetPos = target.transform.position;
                targetPos.y = pos.y;
                velocity = Vector3.Lerp(velocity, (targetPos - pos).normalized * maxSpeed, Time.deltaTime);
            }

            // 일정한 주기가 되었을 때 군체 업데이트.
            _time += Time.deltaTime;
            if (_time >= 0.166f)
            {
                _time = 0;
                _neighbors.Clear();
                
                _neighbors.AddRange(list);
                _neighbors.AddRange(obstacles);

                Vector3 acceleration = Flock(_neighbors);
                acc = Vector3.Lerp(acc, acceleration, Time.deltaTime * 13.0f);
                acc.y = 0;
                if (type == Type.Obstacle) acc = Vector3.zero;
            }

            Vector3 delta = (velocity + acc) * Time.deltaTime;
            delta = Vector3.ClampMagnitude(delta, maxSpeed * Time.deltaTime);

            pos = transform.position;
            velocity += acc * Time.deltaTime;

            if ((targetPos - pos).magnitude <= radius * 1.5f)
            {
                delta = Vector3.Lerp(delta, Vector3.zero, Time.deltaTime * 13.0f);
                transform.position += delta * (targetPos - pos).magnitude / (radius * 2.0f);
                if (boundingBoxSize.sqrMagnitude > 0)
                {
                    pos = transform.position;
                    pos.x = Mathf.Clamp(pos.x, boundingBoxOrigin.x - boundingBoxSize.x / 2, boundingBoxOrigin.x + boundingBoxSize.x / 2);
                    pos.z = Mathf.Clamp(pos.z, boundingBoxOrigin.y - boundingBoxSize.y / 2, boundingBoxOrigin.y + boundingBoxSize.y / 2);
                    transform.position = pos;
                }
                Vector3 camPos = Camera.main.transform.position;
                camPos.y = transform.position.y;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((camPos - pos).normalized, transform.up), Time.deltaTime * 6.5f);
            }
            else
            {
                transform.position += delta;
                if (boundingBoxSize.sqrMagnitude > 0)
                {
                    pos = transform.position;
                    pos.x = Mathf.Clamp(pos.x, boundingBoxOrigin.x - boundingBoxSize.x / 2, boundingBoxOrigin.x + boundingBoxSize.x / 2);
                    pos.z = Mathf.Clamp(pos.z, boundingBoxOrigin.y - boundingBoxSize.y / 2, boundingBoxOrigin.y + boundingBoxSize.y / 2);
                    transform.position = pos;
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((delta + transform.forward * 0.0001f).normalized, Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * 6.5f)), Time.deltaTime * 6.5f);
            }
        }

        public Vector3 Flock(List<DodgeBallBoid> neighbors)
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
        public Vector3 Separate(List<DodgeBallBoid> neighbors)
        {
            Vector3 mean = Vector3.zero;
            Vector3 mean2 = Vector3.zero;
            int num = 0, num2 = 0;

            for (int i = 0; i < neighbors.Count; i++)
            {
                DodgeBallBoid b = neighbors[i];
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

        public Vector3 Align(List<DodgeBallBoid> neighbors)
        {
            Vector3 mean = Vector3.zero;
            int num = 0;

            for (int i = 0; i < neighbors.Count; i++)
            {
                DodgeBallBoid b = neighbors[i];
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

        public Vector3 Cohere(List<DodgeBallBoid> neighbors)
        {
            Vector3 sum = Vector3.zero;
            int num = 0;

            for (int i = 0; i < neighbors.Count; i++)
            {
                DodgeBallBoid b = neighbors[i];
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