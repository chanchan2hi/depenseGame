using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private EnemySpawner enemySpawner; //현재 맵에 존재하는 적 리스트 정보를 얻기위해,,
    [SerializeField] private int towerBuildGold = 50; //타워 건설에 사용되는 골드
    [SerializeField] private PlayerGold playerGold; //타워 건설 시 골드 감소를 위해 
    public void SpawnTower(Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();
        
        //타워 건설 가능 여부 확인
        //1.타워를 건설할 만큼 돈이 없으면 타워건설x
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }
        //2. 현재 타일의 위치에 이미 타워가 건설되어있으면 타워 건설x
        if (tile.IsBuildTower == true)
        {
            return;
        }
        //타워가 건설되어 있음으로 설정
        tile.IsBuildTower = true;
        //타워 건설에 필요한 골드만큼 ㄱ마소
         playerGold.CurrentGold -= towerBuildGold;
        //선택한 타일의 위치에 타워 건설
        GameObject clone =Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().SetUp(enemySpawner);
    }
}
