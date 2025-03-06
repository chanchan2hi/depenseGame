using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
   [SerializeField] private GameObject enemyPrefab; //적 프리팹
   [SerializeField] private GameObject enemyHPSliderPrefab; // 적 체력을 나타내는 slider UI프리팹
   [SerializeField] private Transform canvasTransform; //UI를 표현하는 Canvas오브젝트의 Transform.
   [SerializeField] private float spawnTime; // 적 생성 주기
   [SerializeField] private Transform[] waypoints; // 현재 스테이지의 이동 경로
   [SerializeField] private PlayerHP playerHp; //플레이어의 체력 컴포넌트  
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

         SpawnEnemyHPSlider(clone); //적 체력을 나타내는 Slider UI 생성 및 설정 
         yield return new WaitForSeconds(spawnTime); //spawnTime 시간동안 대기 
      }
   }

   
   public void DestroyEnemy(EnemyDestroyType type, Enemy enemy)
   {
      //적이 목표지점까지 도착했을 떄
      if (type == EnemyDestroyType.Arrive)
      {
         //플레이어의 체력 -1
         playerHp.TakeDamage(1);
      }
      //리스트에서 사망하는 적 정보 삭제
      enemyList.Remove(enemy);
      //적 오브젝트 삭제
      Destroy(enemy.gameObject);
   }
   private void SpawnEnemyHPSlider(GameObject enemy)
   {
     //적 체력을 나타내는 SliderUI 생성
     GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
     //Slider UI오브젝트를 parent("canvas")의 자식으로 설정, 
     //UI는 캔버스의 자식오브젝트로 설정되어 있어야 화면에 보인다!
     sliderClone.transform.SetParent(canvasTransform);
     //계층 설정으로 바뀐 크기를 다시 (1,1,1)로 설정
     sliderClone.transform.localPosition = Vector3.one;
     
     //SliderUI가 쫓아다닐 대상을 본인으로 설정 
     sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
     //Slider UI에 자신의 체력 정보를 표시하도록 설정 
     sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHp>());
   }

}
