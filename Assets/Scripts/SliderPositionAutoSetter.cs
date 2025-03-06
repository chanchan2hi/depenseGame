using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
   [SerializeField] private Vector3 distance = Vector3.down * 20.0f;
   private Transform targetTransform;
   private RectTransform rectTransform;

   public void Setup(Transform target)
   {
      //SliderUI가 쫓아다닐 target 설정 
      targetTransform = target;
      //RectTransform정보 얻어오기
      rectTransform = GetComponent<RectTransform>();
   }

   private void LateUpdate()
   {
      //적이 파괴되어 쫓아다닐 대상이 사라지면 Slider UI도 삭제
      if (targetTransform == null)
      {
         Destroy(gameObject);
         return;
      }
      //오브젝트의 위치가 갱신된 이후에 SliderUI도 함께 위치를 설정하도록 하기 위해
      //LateUpdate()에서 호출한다.
      //오브젝트의 월드 좌표를 기준으로 화면에서의 좌표 값을 구함
      Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetTransform.position);
      //화면 내에서 좌표 +distance 만큼 떨어진 위치를 SliderUI의 위치로 지정
      rectTransform.position = screenPoint + distance;
   }
   
}
