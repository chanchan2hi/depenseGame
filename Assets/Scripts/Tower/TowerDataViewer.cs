using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerDataViewer : MonoBehaviour
{
    //타워 정보 UI 
    [SerializeField] private Image imageTower;
    [SerializeField] private TMP_Text textDamage;
    [SerializeField] private TMP_Text textRate;
    [SerializeField] private TMP_Text textRange;
    [SerializeField] private TMP_Text textLevel;
    [SerializeField] private TowerAttackRange towerAttackRange;
    [SerializeField] private Button buttonUpgrade;
    [SerializeField] private SystemTextViewer systemTextViewer;
    private TowerWeapon currentTower;
    
    
    private void Awake()
    {
        OffPanel();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffPanel();
        }
    }

    public void OnPanel(Transform towerWeapon)
    {
        //출력해야하는 타워 정보를 받아와 저장
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
        //타워 정보 panel on
        gameObject.SetActive(true);
        UpdateTowerData();
        //타워 오브젝트 주변에 표시되는 타워 공격범위 SPrite On
        towerAttackRange.OnAttackRange(currentTower.transform.position,currentTower.Range);
    }

    public void OffPanel()
    {
        //타워 정보 Panel OFF
        Debug.Log("TowerDataViewer::OffPanel");
        gameObject.SetActive(false);
        //타워공격범위 sprite Off
        towerAttackRange.OffAttackRange();
    }

    private void UpdateTowerData()
    {
        imageTower.sprite = currentTower.TowerSprite;
        textDamage.text = "Damage: " + currentTower.Damage;
        textRate.text = "Rate: " + currentTower.Rate;
        textRange.text = "Range: " + currentTower.Range;
        textLevel.text = "Level: " + currentTower.Level;
        
        //업그레이드가 불가능해지면(최대 레벨) 버튼 비활성화
        buttonUpgrade.interactable=currentTower.Level<currentTower.MaxLevel? true:false;
    }

    public void OnClickEventTowerUpgrade()
    {
        //타워 업그레이드 시도(성공:t, 실패 ;f)
        bool isSuccess = currentTower.Upgrade();
        if (isSuccess == true)
        {
            //타워가 업그레이드 되었기 때문에 타워 정보 갱신
            UpdateTowerData();
            // 타워 주변에 보이는 공격범위도 갱신
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            //타워 업그레이드에 필요한 비용 부족하다고 출력 
            systemTextViewer.PrintText(SystemType.Money);
        }
    }

    public void OnClickEventTowerSell()
    {
        //타워 판매
        currentTower.Sell();
        //선택한 타워가 사라져서 Panl,공격범위 OFF
        OffPanel();
    }
}
