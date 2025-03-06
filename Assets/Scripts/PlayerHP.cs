using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private Image imageScreen; //빨간이미지 
    [SerializeField] private float maxHP = 20; //최대 체력
    private float currentHP; //현재 체력
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP; //현재 체력을 최대 체력과 같게 설정
    }

    public void TakeDamage(float damage)
    {
        //현재 체력을 damaㅎㄷakszma rkath
        currentHP -= damage;
        
        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");
        
        //체력이 0이되면 게임오버
        if (currentHP <= 0)
        {
            
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        //전체화면 크리고 배치된 image색상을 color변수에저장, 투명도 40
        Color color = imageScreen.color;
        color.a = 0.4f;
        imageScreen.color = color;
        
        //투명도가 0%가 될때까지 감소
        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            imageScreen.color = color;
            
            yield return null;
        }
    }
    
    
    
}
