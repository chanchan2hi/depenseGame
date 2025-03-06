using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;

    public void SetUp(Transform target)
    {
        movement2D = GetComponent<Movement2D>();
    
        if (movement2D == null)
        {
            Debug.LogError("Movement2D component is missing on the projectile.");
            return;
        }
    
        this.target = target; //타워가 설정해준 타겟
    }


    private void Update()
    {
        if (target != null) //target이 존재하면
        {
            //발사체를 target의 위치로 이동
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return; // 적이 아닌 대상과 부딪히면
        if (collision.transform != target) return; //현재 target인 적이 아닐 때
    
        // Enemy 컴포넌트를 가져와서 OnDie() 호출
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnDie(); // 적 사망 함수 호출
        }
        else
        {
            Debug.LogWarning("Collision object does not have an Enemy component.");
        }
    
        Destroy(gameObject); // 발사체 삭제
    }

}
