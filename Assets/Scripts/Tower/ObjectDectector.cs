using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDectector : MonoBehaviour
{
  [SerializeField]private TowerSpawner towerSpawner;
  [SerializeField]private TowerDataViewer towerDataViewer;
  private Camera mainCamera;
  private Ray ray;
  private RaycastHit hit;

  private void Awake()
  {
    //"mainCamera"태그를 가지고 있는 오브젝트 탐색 후 카메라 컴포넌트 정보 전달
    //GameObject.FindGameObjectTag("mainCamera").GetComponent<Camera>(); 와동일
    mainCamera = Camera.main;
  }

  
  private void Update()
  {
    // 마우스 왼쪽 버튼을 눌렀을 떄
    if (Input.GetMouseButtonDown(0))
    {
      //카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생선
      //ray.origin: 광선의 시작위치(=카메라위치)
      //ray.direction :광선의 진행방향
      ray = mainCamera.ScreenPointToRay(Input.mousePosition);
      //2D모니터를 통해 3D월드의 오브젝트를 마우스로 선택하는 방법
      //광선에 부딪히는 오브젝트를 검출하여 hit에 저장
      if (Physics.Raycast(ray, out hit, Mathf.Infinity))
      {
       // Debug.Log("태그 전");
        //광선에 부딪힌 오브젝트의 태그가 "TILE"이면
        if (hit.transform.CompareTag("Tile"))
        {//
         // Debug.Log("태그 후");
         //타워를 생성하는 SpawnerTower()호출
         towerSpawner.SpawnTower(hit.transform);
        }
        //타워를 선택하면 해당 타워 정보를 출력하는 타워정보창 ON
        else if (hit.transform.CompareTag("Tower"))
        {
          towerDataViewer.OnPanel(hit.transform);
          
        }
      }
    }
  }
}
