using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType{Kill=0, Arrive}
public class Enemy : MonoBehaviour
{
   private int wayPointCount; //이동 경로 개수
   private Transform[] wayPoints; //이동 경로 정보
   private int currentIndex = 0; //현재 목표지점 인덱스
   private Movement2D movement2D; //오브젝트 이동 제어
   private EnemySpawner enemySpawner; // 적의 삭제를 본인이 하지 않고 EnemySpawner에 알려서 삭제 
   

   //적의 이동경로를 매개변수로 호출 
   public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
   {
      movement2D = GetComponent<Movement2D>();
    
      if (enemySpawner == null)
      {
         Debug.LogError("EnemySpawner is null in Setup!");
         return;
      }
    
      this.enemySpawner = enemySpawner; // enemySpawner를 설정해야 합니다.
    
      // 이동 경로 설정
      wayPointCount = wayPoints.Length;
      this.wayPoints = wayPoints;
    
      // 적의 위치를 첫 번째 wayPoint 위치로 설정
      transform.position = wayPoints[currentIndex].position;
    
      // 이동/목표지점 설정 코루틴 함수 시작
      StartCoroutine(OnMove());
   }


   private IEnumerator OnMove()
   {
      //다음 이동 방향 설정 
      NextMoveTo();
      while (true)
      {
         //적 오브젝트 회전
         transform.Rotate(Vector3.forward*10);
         //적의 현재 위치와 목표 위치의 거리가 0.02*movement2d.movespeed보다 작을 때 조건문 실행 
         //movement2d.movespeed를 곱하는 이유: 속도가 빠르면 한 프레임에 0.02보다 크게 움직여 탈주할수있음
         //vector3.distance(a,b) : 두 점a,b 사이의 거리를 구하는 메소드
         if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
         {
            //다음 이동방향 설정
            NextMoveTo();
         }
         yield return null;
      }
   }

   private void NextMoveTo()
   {
      //아직 이동할 waypoints가 남아있다면
      if (currentIndex < wayPoints.Length - 1)
      {
         //적의 위치를 정확하게 목표 위치로 설정
         transform.position = wayPoints[currentIndex].position;
         //이동방향 설정-> 다음 목표지점(waypoints)
         currentIndex++;
         Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
         movement2D.MoveTo(direction);
      }
      //현재 위치가 마지막 waypoints이면
      else
      {
         //적 오브젝트 삭제
         // Destroy(gameObject);
         OnDie(EnemyDestroyType.Arrive);
      }
   }

   public void OnDie(EnemyDestroyType type)
   {
      //EnemySpawner에서 리스트로 적 정보를 관리하기 때문에 Destory()를 직접하지 않고
      //EnemySpawner에게 본인이 삭제될 때 필요한 처리를 하도록 DestroyEnemy()함수 호출
      enemySpawner.DestroyEnemy(type,this);
   }
}
