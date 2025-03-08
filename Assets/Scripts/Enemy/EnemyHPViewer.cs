using UnityEngine;
using UnityEngine.UI;

public class EnemyHPViewer : MonoBehaviour
{
    private EnemyHp enemyHP;
    private Slider hpSlider;

    public void Setup(EnemyHp enemyHp)
    {
        this.enemyHP = enemyHp;
        hpSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        hpSlider.value=enemyHP.CurrentHP/enemyHP.MaxHP;
    }
}
