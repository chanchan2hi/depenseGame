using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTMPViewer : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI textPlayerHP; //플레이어 체력 text
  [SerializeField] private PlayerHP playerHP;
  [SerializeField] private TextMeshProUGUI textPlayerGold;
  [SerializeField] private PlayerGold playerGold; 
  [SerializeField] private TextMeshProUGUI textWave;
  [SerializeField] private WaveSystem waveSystem; //웨이브 정보 ;
  [SerializeField] private TextMeshProUGUI textEnemyCount; //현재 적 숫자/최대 적 숫자
  [SerializeField] private EnemySpawner enemySpawner; //적 정보 

  private void Update()
  {
    textPlayerHP.text = playerHP.CurrentHP+"/"+playerHP.MaxHP;
    textPlayerGold.text = playerGold.CurrentGold.ToString();
    textWave.text = waveSystem.CurrentWave+"/"+waveSystem.MaxWave;
    textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
  }
}
