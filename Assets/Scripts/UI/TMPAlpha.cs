using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPAlpha : MonoBehaviour
{
   [SerializeField] private float lerpTime = 0.5f;
   private TextMeshProUGUI text;

   private void Awake()
   {
      text=GetComponent<TextMeshProUGUI>();
   }

   public void FadeOut()
   {
      StartCoroutine(AplhaLerp(1, 0));
   }

   public IEnumerator AplhaLerp(float start, float end)
   {
      float currentTime = 0f;
      float percent = 0f;
      while (percent < 1)
      {
         //lerpTime시간동안 while()반복문 실행
         currentTime+=Time.deltaTime;
         percent = currentTime/lerpTime;
         
         //text-textMeshPro의 폰트 투명돌,ㄹ start에서 end로 변경
         Color color = text.color;
         color.a = Mathf.Lerp(start, end, percent);
         text.color = color;
         
         yield return null;
      }
      
   }
}
