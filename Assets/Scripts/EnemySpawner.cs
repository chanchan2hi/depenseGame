using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
   [SerializeField] private GameObject enemyPrefab; //적 프리팹
   [SerializeField] private float spawnTime; // 적 생성 주기
   [SerializeField] private Transform[] waypoints; // 현재 스테이지의 이동 경로
   private List<Enemy> enemyList;  //현재 맵에 존재하는 모든 적의 정보
   
   //적의 생성과 삭제는 EnemySpawner에서 하기 때문에 Set은 필요 없다.
   public List<Enemy> EnemyList => enemyList;

   private void Awake()
   {
      //적 리스트 메모리 할당
      enemyList = new List<Enemy>();
      //적 생성 코루틴 함수 호출
      StartCoroutine("SpawnEnemy");
   }

   private IEnumerator SpawnEnemy()
   {
      while (true)
      {
         GameObject clone = Instantiate(enemyPrefab); // 적 오브젝트 생성 
         Enemy enemy = clone.GetComponent<Enemy>(); // 방금 생성된 적의 Enemy 컴포넌트
         //this는 나 자신(자신의 EnemySpawner정보) 
         enemy.Setup(this,waypoints);//웨이포인트 정보를 매개변수로  Setup()호출
         enemyList.Add(enemy);//리스트에 방금 생성된 적 정보 저장 
         yield return new WaitForSeconds(spawnTime); //spawnTime 시간동안 대기 
      }
   }

   public void DestroyEnemy(Enemy enemy)
   {
      //리스트에서 사망하는 적 정보 삭제
      enemyList.Remove(enemy);
      //적 오브젝트 삭제
      Destroy(enemy.gameObject);
   }

}
