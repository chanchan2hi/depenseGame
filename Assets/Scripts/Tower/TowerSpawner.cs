using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]private TowerTemplate towerTemplate; //타워 정보(공격력,공격속도 등) 
  //  [SerializeField] private GameObject towerPrefab;
    [SerializeField] private EnemySpawner enemySpawner; //현재 맵에 존재하는 적 리스트 정보를 얻기위해,,
   // [SerializeField] private int towerBuildGold = 50; //타워 건설에 사용되는 골드
    [SerializeField] private PlayerGold playerGold; //타워 건설 시 골드 감소를 위해 
    [SerializeField] private SystemTextViewer systemTextViewer; //돈 부족, 건설 불가와 같은 시스템 메시지 출력
    public void SpawnTower(Transform tileTransform)
    {
        //타워 건설 가능 여부 확인
        //1.타워를 건설할 만큼 돈이 없으면 타워건설x
        //if (towerBuildGold > playerGold.CurrentGold)
        if(towerTemplate.weapon[0].cost >playerGold.CurrentGold)
        {
            //골드가 부족해서 타워건설이 불가능 
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        //타일이 왜 .true로 안바뀌일까? 근데 건설은안되긴함.. 
        Tile tile = tileTransform.GetComponent<Tile>();
        //2. 현재 타일의 위치에 이미 타워가 건설되어있으면 타워 건설x
        if (tile.IsBuildTower == true)
        {
            Debug.Log("타워가 있음 ");
            //현재 위치에 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }
        else
        {
            // 타워 건설 상태로 변경
            tile.IsBuildTower = true;
            Debug.Log("타워 건설 후 IsBuildTower 값: " + tile.IsBuildTower); // 타워 건설 후 값을 확인
        }
        //타워가 건설되어 있음으로 설정
        tile.IsBuildTower = true;
        //타워 건설에 필요한 골드만큼 ㄱ마소
       //  playerGold.CurrentGold -= towerBuildGold;
       playerGold.CurrentGold-=towerTemplate.weapon[0].cost;
        //선택한 타일의 위치에 타워 건설 (타일보다 z축 -1의 위치에 배치)
        Vector3 position = tileTransform.position + Vector3.back;
       // GameObject clone =Instantiate(towerPrefab, position, Quaternion.identity);
       GameObject clone = Instantiate(towerTemplate.TowerPrefab, position, Quaternion.identity);
        //타워 무기에 enemySpawner 정보전달 
        clone.GetComponent<TowerWeapon>().SetUp(enemySpawner,playerGold,tile);
    }
}
