using UnityEngine;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]private TowerTemplate[] towerTemplate; //타워 정보(공격력,공격속도 등) 
    [SerializeField] private EnemySpawner enemySpawner; //현재 맵에 존재하는 적 리스트 정보를 얻기위해,,
    [SerializeField] private PlayerGold playerGold; //타워 건설 시 골드 감소를 위해 
    [SerializeField] private SystemTextViewer systemTextViewer; //돈 부족, 건설 불가와 같은 시스템 메시지 출력
    private bool isOnTowerButton=false; //타워 건설 버튼을 눌렀는지 체크 
    private GameObject followTowerClone = null; // 임시 타워 사용 완료시 삭제를 위해 저장하는 변수 
    private int towerType; //타워 속성 
    
    //타워 건설 가능 여부 확인
    public void ReadyToSpawnTower(int type)
    {
        towerType = type;
        //버튼을 중복해서 누르는 것을 방지하기 위해 필요
        if (isOnTowerButton == true)
        {
            return;
        }
        //타워를 건설할 돈 없으면 건설x
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            systemTextViewer.PrintText(SystemType.Money);
            return; 
        }
        //타워 건설 버튼을 눌렀다고 설정
        isOnTowerButton = true;
        //마우스 따라다니는 임시 타워 생성
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        //타워 건설을 취소할수 있는 코루틴 함수 시작
        StartCoroutine("OnTowerCancelSystem");
    }
    public void SpawnTower(Transform tileTransform)
    {
        
        //타워 건설 버튼을 눌렀을 때만 타워 건설 가능
        if (isOnTowerButton == false)
        {
            return;
        }
        //타일이 왜 .true로 안바뀌일까? 근데 건설은안되긴함.. 
        Tile tile = tileTransform.GetComponent<Tile>();
        //2. 현재 타일의 위치에 이미 타워가 건설되어있으면 타워 건설x
        if (tile.IsBuildTower ==true)
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
        //다시 타워 건설 버튼을 눌러서 타워를 건설하도록 변수 설정
        isOnTowerButton = false;
        //타워가 건설되어 있음으로 설정
        tile.IsBuildTower = true;
        Debug.Log("타워 건설!");
        //타워 건설에 필요한 골드만큼 ㄱ마소
       playerGold.CurrentGold-=towerTemplate[towerType].weapon[0].cost;
        //선택한 타일의 위치에 타워 건설 (타일보다 z축 -1의 위치에 배치)
        Vector3 position = tileTransform.position + Vector3.back;
       GameObject clone = Instantiate(towerTemplate[towerType].TowerPrefab, position, Quaternion.identity);
        //타워 무기에 enemySpawner 정보전달 
        clone.GetComponent<TowerWeapon>().SetUp(enemySpawner,playerGold,tile);
        //타워를 배치했기 때문에 임시타워 삭제
        Destroy(followTowerClone);
        //타워 건설을 취소할수있는 코루틴 함수 중지
        StopCoroutine("OnTowerCancelSystem");
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            //esc 또는 마우스 오른쪽 버튼을 눌렀을 떄 타워 건설 취소
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton=false;
                //임시타워 삭제
                Destroy(followTowerClone);
                break;
            }
            yield return null;
        }
    }
}
