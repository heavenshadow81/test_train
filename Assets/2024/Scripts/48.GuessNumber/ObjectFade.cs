using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class ObjectFade : MonoBehaviour
    {
        public List<Material> materials = new List<Material>();

        private void OnEnable()
        {
            FindMaterials();
        }

        // 자식들의 머티리얼을 찾는 함수
        public void FindMaterials()
        {
            if (materials.Count > 0)
            {
                materials.Clear();
            }

            // 현재 오브젝트와 자식 오브젝트들에서 Renderer 컴포넌트를 찾아 머티리얼을 추가
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                // Renderer에 있는 모든 머티리얼을 리스트에 추가
                materials.AddRange(renderer.materials);
            }
        }

        // 페이드 인 함수 (서서히 나타남)
        public void FadeIn(float duration, System.Action onComplete = null)
        {
            // 알파 값을 조정하면서 페이드 인
            StartCoroutine(Fade(0f, 1f, duration, () =>
            {
                // 페이드 인이 끝나면 항상 OnFadeInComplete 호출
                OnFadeInComplete();

                // 추가 콜백이 있다면 그 콜백도 실행
                onComplete?.Invoke();
            }));
        }

        // 페이드 아웃 함수 (서서히 사라짐)
        public void FadeOut(float duration, System.Action onComplete = null)
        {
            // 페이드 아웃 시작 전에 랜더링 모드를 Fade로 변경
            OnFadeOutcomplete();

            StartCoroutine(Fade(1f, 0f, duration, onComplete));
        }

        // 페이드 애니메이션 코루틴
        private IEnumerator Fade(float startAlpha, float endAlpha, float duration, System.Action onComplete = null)
        {
            float time = 0f;

            // 초기 알파 값을 설정
            foreach (Material mat in materials)
            {
                if (mat.HasProperty("_BaseColor")) // _BaseColor 속성이 존재하는 경우
                {
                    // Material의 BaseColor 속성에서 알파 값을 설정
                    Color baseColor = mat.GetColor("_BaseColor");  // Shader의 BaseColor 가져오기
                    baseColor.a = startAlpha;  // 알파 값 설정
                    mat.SetColor("_BaseColor", baseColor);  // 설정된 알파 값을 다시 Material에 적용
                }
                else // _BaseColor가 없는 경우, 기본 color를 사용
                {
                    Color color = mat.color;
                    color.a = startAlpha;  // 알파 값 설정
                    mat.color = color;  // 설정된 알파 값을 다시 Material에 적용
                }
            }

            // 페이드 애니메이션
            while (time < duration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);

                foreach (Material mat in materials)
                {
                    if (mat.HasProperty("_BaseColor")) // _BaseColor 속성이 존재하는 경우
                    {
                        // Material의 BaseColor 속성에서 알파 값을 조정
                        Color baseColor = mat.GetColor("_BaseColor");  // Shader의 BaseColor 가져오기
                        baseColor.a = alpha;  // 알파 값 설정
                        mat.SetColor("_BaseColor", baseColor);  // 설정된 알파 값을 다시 Material에 적용
                    }
                    else // _BaseColor가 없는 경우, 기본 color를 사용
                    {
                        Color color = mat.color;
                        color.a = alpha;
                        mat.color = color;
                    }
                }

                yield return null;
            }

            // 마지막 알파 값 보정
            foreach (Material mat in materials)
            {
                if (mat.HasProperty("_BaseColor")) // _BaseColor 속성이 존재하는 경우
                {
                    // Material의 BaseColor 속성에서 알파 값을 보정
                    Color baseColor = mat.GetColor("_BaseColor");
                    baseColor.a = endAlpha;
                    mat.SetColor("_BaseColor", baseColor);
                }
                else // _BaseColor가 없는 경우, 기본 color를 사용
                {
                    Color color = mat.color;
                    color.a = endAlpha;
                    mat.color = color;
                }
            }

            // 페이드 완료 후 콜백 함수 호출
            onComplete?.Invoke();
        }


        // 페이드 아웃 완료 후 호출되는 함수
        private void OnFadeInComplete()
        {
            // 페이드 아웃이 끝난 후 랜더링 모드를 다시 Opaque로 변경
            foreach (Material mat in materials)
            {
                SetMaterialToOpaqueMode(mat);
            }
        }

        private void OnFadeOutcomplete()
        {
            // 페이드 아웃이 끝난 후 랜더링 모드를 다시 Opaque로 변경
            foreach (Material mat in materials)
            {
                SetMaterialToFadeMode(mat);
            }
        }

        // 머티리얼의 랜더링 모드를 Fade로 설정하는 함수
        private void SetMaterialToFadeMode(Material mat)
        {
            mat.SetFloat("_Mode", 2); // Rendering Mode를 Fade로 설정 (2는 Fade)
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        // 머티리얼의 랜더링 모드를 Opaque로 설정하는 함수
        private void SetMaterialToOpaqueMode(Material mat)
        {
            mat.SetFloat("_Mode", 0); // Opaque 모드로 설정
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }
    }
}
