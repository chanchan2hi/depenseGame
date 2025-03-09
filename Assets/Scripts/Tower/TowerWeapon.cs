using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum WeaponType{Cannon=0,Laser,}
public enum WeaponState{SearchTarget=0, TryAttackCannon,TryAttackLaser,}
public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]private TowerTemplate towerTemplate; //타워 정보 (공격력,공격속도 등)
    [SerializeField]private Transform spawnPoint; //발사체 생성 위치
    [SerializeField] private WeaponType weaponType;
    
    [Header("Cannon")]
    [SerializeField]private GameObject projectilePrefab; //발사체 프리팹

    [Header("Laser")] 
    [SerializeField] private LineRenderer lineRenderer; //레이저로 사용되는 선(LineRenderer)
    [SerializeField] private Transform hitEffect; //타격 효과
    [SerializeField] private LayerMask targetLayer; //광선에 부딪히는 레이어 설정 
    
    private int level = 0; //타워 레벨
    private WeaponState weaponState = WeaponState.SearchTarget; //타워 무기의 상태
    private Transform attackTarget = null; //공격 대상
    private SpriteRenderer spriteRenderer; //타워 오브젝트 이미지 변경용 
    private EnemySpawner enemySpawner; //게임에 존재하는 적 정보 획득용
    private PlayerGold playerGold; //플레이어의 골드 정보 획득 및 설정 
    private Tile ownerTile; //현재 타워가 배치되어있는 타일 

    // public float Damage => attackDamage;
    // public float Rate => attackRate;
    // public float Range => attackRange;
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate=>towerTemplate.weapon[level].rate;
    public float Range=>towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public void SetUp(EnemySpawner enemySpawner,PlayerGold playerGold,Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;
        
        //최초 상태를 WeaponState.SearchTarget으로 설정
        ChangeState(WeaponState.SearchTarget);

    }

    public void ChangeState(WeaponState newState)
    {
        //이전에 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());
        //상태 변경
        weaponState=newState;
        //새로운 상태 재생
        StartCoroutine(weaponState.ToString());
        
    }

    public void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    public void RotateToTarget()
    {
        //원점으로부터 거리와 수평축으로부터의 각도를 이용해 위치를 구하는 극 좌표계 이용
        //각도 = arctan(y/x)
        //x,y 변위값 구하기 
        float dx= attackTarget.position.x - transform.position.x;
        float dy =attackTarget.position.y - transform.position.y;
        //x,y변위값을 바탕으로 각도 구ㅏ하기
        //각도가 radian 단위이기 때문에 Mathf.Rad2Deg을 통해 도 단위를 구함
        float degree = Mathf.Atan2(dy, dx)*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
        
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            attackTarget = FindClosetAttackTarget();
            if (attackTarget != null)
            {
                if (weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if (weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
                
            }

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            //target공격이 가능한지검사
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
           yield return new WaitForSeconds(towerTemplate.weapon[level].rate);
            //4.공격(발사체 생성
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        //레이저,레이저 타격 효과 활성화
        EnableLaser();

        while (true)
        {
            //target을 공격하는게 가능한지 검사
            if (IsPossibleToAttackTarget() == false)
            {
                //레이저,레이저 타격 효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;

            }

            SpawnLaser();
            yield return null;
        }
    }

    private Transform FindClosetAttackTarget()
    {
        //제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
        float closetDistSqr=Mathf.Infinity;
        for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            //현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
            if (distance <= towerTemplate.weapon[level].range && distance <= closetDistSqr)
            {
                closetDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        //타겟이 있는지 검사(다른 발사체에 의해 제거,goal지점까지 이동해 삭제 등)
        if (attackTarget == null)
        {
            return false;
        }
        //타겟이 공격범위 내에 있는지 검사(범위 벗어나면 새로운 적 탐색)
        float distance = Vector3.Distance(attackTarget.position,transform.position);
        if (distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }
        return true;
    }

    private void SpawnProjectile()
    {
       GameObject clone= Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
       //생성ㅇ된 발사체에게 공격대상(attackTarget)정보 제공
      // clone.GetComponent<Projectile>().SetUp(attackTarget,attackDamage);
      clone.GetComponent<Projectile>().SetUp(attackTarget,towerTemplate.weapon[level].damage);
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
        
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
        
    }

    private void SpawnLaser()
    {
        Vector3 directrion = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, directrion,
                                                       towerTemplate.weapon[level].range, targetLayer);
        
        //같은 방향으로 여러개의 광선을 쏴서 그 중 현재 attackTarget과 동일한 오브젝트 검출
        for (int i = 0; i < hit.Length; ++i)
        {
            if (hit[i].transform == attackTarget)
            {
                //선의 시작지점
                lineRenderer.SetPosition(0,spawnPoint.position);
                //선의 목표지점
                lineRenderer.SetPosition(1,new Vector3(hit[i].point.x,hit[i].point.y,0)+Vector3.back);
                //타격 효과 위치 설정
                hitEffect.position =hit[i].point;
                //적 체력 감소(1초에 damage만큼 감소)
                attackTarget.GetComponent<EnemyHp>().TakeDamage(towerTemplate.weapon[level].damage*Time.deltaTime);
            }
        }
    }

    public bool Upgrade()
    {
        //타워 업그레이드에 필요한 골드가 충분한지 검사
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false;
        }
        //타워 레벨 증가
        level++;
        //타워 외형 변경(Sprite)
        spriteRenderer.sprite=towerTemplate.weapon[level].sprite;
        //골드 차감
        playerGold.CurrentGold-=towerTemplate.weapon[level].cost;

        //무기 속성이 레이저이면
        if (weaponType == WeaponType.Laser)
        {
            //레벨에 따라 레이저의 굵기 설정
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
        return true;
    }

    public void Sell()
    {
        //골드 증가
        playerGold.CurrentGold +=towerTemplate.weapon[level].sell;
        //현재 타일에 다시 타워 건설이 가능하도록 설정
        ownerTile.IsBuildTower = false;
        //타워 파괴
        Destroy(gameObject);
    }
}
