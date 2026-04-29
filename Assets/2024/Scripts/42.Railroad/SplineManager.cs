using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Railroad
{
    public class SplineManager : MonoBehaviour
    {
        [Header("수동으로 SplineContainer 리스트 설정")]
        public SplineContainer mergedSplineContainer;  // 병합된 스플라인을 담을 SplineContainer
        public List<SplineContainer> splineContainers = null; // Inspector에서 설정할 SplineContainer 리스트

        // 새로운 SplineContainer를 리스트의 끝에서 두 번째 위치에 추가하는 함수
        public void AddSplineContainerAtSecondLastIndex(SplineContainer newContainer)
        {
            splineContainers.Insert(splineContainers.Count - 1, newContainer);
        }

        // 모든 스플라인을 병합하여 하나의 SplineContainer에 담기
        public void MergeAllSplinesIntoOne()
        {
            if (mergedSplineContainer == null)
            {
                return;
            }

            // 병합할 SplineContainer 초기화
            UnityEngine.Splines.Spline mergedSpline = mergedSplineContainer.Spline;
            mergedSpline.Clear();

            // Inspector에서 설정한 스플라인 컨테이너 리스트를 순회하며 병합
            foreach (var splineContainer in splineContainers)
            {
                // 각 SplineContainer의 Spline을 가져와서 병합
                UnityEngine.Splines.Spline currentSpline = splineContainer.Spline;
                AddSplinePoints(currentSpline, mergedSpline, splineContainer, includeEndPoint: true);
            }
        }

        // Spline의 포인트를 병합하여 연결
        private void AddSplinePoints(UnityEngine.Splines.Spline sourceSpline, UnityEngine.Splines.Spline targetSpline, SplineContainer sourceContainer, bool includeEndPoint)
        {
            int pointCount = sourceSpline.Count;

            // 포인트를 순차적으로 순회하여 추가
            for (int i = 0; i < pointCount; i++)
            {
                // 마지막 포인트를 포함할지 여부에 따라 포인트 추가
                if (i == pointCount - 1 && !includeEndPoint) continue;

                // 소스 스플라인의 포인트를 대상 스플라인에 추가 (월드 좌표계 변환)
                BezierKnot knot = sourceSpline[i];

                // Knot의 위치와 탄젠트를 월드 좌표계로 변환
                Vector3 positionWorld = sourceContainer.transform.TransformPoint(knot.Position);
                Vector3 tangentInWorld = sourceContainer.transform.TransformPoint(knot.Position + knot.TangentIn) - positionWorld;
                Vector3 tangentOutWorld = sourceContainer.transform.TransformPoint(knot.Position + knot.TangentOut) - positionWorld;

                // 월드 좌표계를 병합될 스플라인의 로컬 좌표계로 변환
                Vector3 positionLocal = mergedSplineContainer.transform.InverseTransformPoint(positionWorld);
                Vector3 tangentInLocal = mergedSplineContainer.transform.InverseTransformVector(tangentInWorld);
                Vector3 tangentOutLocal = mergedSplineContainer.transform.InverseTransformVector(tangentOutWorld);

                // 새로운 Knot 생성 (정상 방향으로 탄젠트 설정)
                BezierKnot newKnot = new BezierKnot(positionLocal, tangentInLocal, tangentOutLocal, knot.Rotation);

                targetSpline.Add(newKnot);
            }
        }
    }
}
