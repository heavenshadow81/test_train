using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum PipeDirection
{
    North,
    South,
    West,
    East
}

public class Pipe_PC : MonoBehaviour
{
    public PipeDirection[] connectDirections; // 연결 가능한 방향 모두 담는 배열
    public PipeDirection frontDirection;  // 정방향 연결 
    public PipeDirection startDirection; // 시작 방향
    public PipeDirection endDirection; // 끝 방향

    public List<Image> pipeImages; // 파이프에 대한 이미지 배열

    // 파이프 연결 확인 메서드
    public void CheckConnection(Pipe_PC currentPipe)
    {
        // 현재 파이프와 연결 가능한 파이프 방향 찾기
        PipeDirection connectableDirection = OppositeDirection(currentPipe.endDirection);

        // 찾은 파이프 방향이 연결 가능한 방향 배열에 담겨있다면
        if (connectDirections.Contains(connectableDirection))
        {
            // 연결한 파이프가 정방향인지 체크
            if (connectableDirection == frontDirection)
            {
                // 시작 방향 정방향으로 설정
                startDirection = connectableDirection;

                // connectDirections 에 남은 하나를 끝방향으로 설정
                List<PipeDirection> remainingDirections = connectDirections.ToList();
                remainingDirections.Remove(connectableDirection); // 연결된 방향 제거

                if (remainingDirections.Count > 0)
                {
                    // 남은 방향이 있으면 끝 방향으로 설정
                    endDirection = remainingDirections[0];
                }
            }
            else
            {
                // 시작 방향을 연결 가능한 방향으로 설정
                startDirection = connectableDirection;

                // connectDirections 에 남은 하나를 끝방향으로 설정
                List<PipeDirection> remainingDirections = connectDirections.ToList();
                remainingDirections.Remove(connectableDirection); // 연결된 방향 제거

                if (remainingDirections.Count > 0)
                {
                    // 남은 방향이 있으면 끝 방향으로 설정
                    endDirection = remainingDirections[0];
                }

                // pipeImages의 순서를 역순으로 바꾸고 흐름도 반대로 바꾸기
                pipeImages.Reverse();
                ChangeFillOrigin();
            }

            WaterFlowManager_PC.Instance.isConnectable = true;
        }
        else
        {
            WaterFlowManager_PC.Instance.isConnectable = false;
        }
    }

    public void ChangeFillOrigin()
    {
        foreach (Image image in pipeImages)
        {
            image.fillOrigin = 3; // Bottom으로 변경
        }
    }

    public PipeDirection OppositeDirection(PipeDirection direction)
    {
        switch (direction)
        {
            case PipeDirection.North:
                return PipeDirection.South;
            case PipeDirection.South:
                return PipeDirection.North;
            case PipeDirection.East:
                return PipeDirection.West;
            case PipeDirection.West:
                return PipeDirection.East;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}
