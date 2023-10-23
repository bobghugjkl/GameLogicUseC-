using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlyerShutting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    //只需要知道是那种武器即可
    private WeaponManager weaponManager;
    private PlayerWeapon currentWeapon;
    private float shootCoolDownTime = 0f;//距离上次开枪用了多久
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    //导入引用
    private PlayerController playerController;
    private Player player;
    //记录开了多少枪
    private int autoshootCount = 0;
    //枚举
    enum HitEffectMaterial
    {
        Metal,
        Stone,
    }
    // Start is called before the first frame update
    void Start()
    {
        //这是获取子类的,camera在player的子元素里面
        cam = GetComponentInChildren<Camera>();
        //动态获取
        weaponManager = GetComponent<WeaponManager>();
        playerController = GetComponent<PlayerController>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //返回相邻两次操作时间
        shootCoolDownTime += Time.deltaTime;
        if (!IsLocalPlayer) return;
        //先看看他们是谁，射击之前将引用获取
        currentWeapon = weaponManager.GetCurrentWeapon();


        if(currentWeapon.shootRate <= 0)//单发
        {
            //获取用户输入
            if (Input.GetButtonDown("Fire1")&& shootCoolDownTime>=currentWeapon.shootCoolDownTime&&player.IsDead()==false)
            {
                autoshootCount = 0;
                Shoot();
                shootCoolDownTime = 0f;//重置冷却
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && player.IsDead() == false)
            {
                autoshootCount = 0;
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.shootRate);

            } else if (Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }
    private void OnHit(Vector3 pos,Vector3 normal,HitEffectMaterial material)//击中特效
    {
        GameObject hitEffectPrefab;
        if (material == HitEffectMaterial.Metal)
        {
            hitEffectPrefab = weaponManager.GetCurrentGraphics().metalHitEffectPrefab;
        }
        else
        {
            hitEffectPrefab = weaponManager.GetCurrentGraphics().stoneHitEffectPrefab;
        }
        //动态创建出来,Instantiate实例化,反方向喷射
        GameObject hitEffectObject = Instantiate(hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        ParticleSystem particleSystem = hitEffectObject.GetComponent<ParticleSystem>();
        //直接触发
        particleSystem.Emit(1);
        particleSystem.Play();
        Destroy(hitEffectObject,1f);
    }
    //这个的网络通信
    [ServerRpc]
    private void OnHitServerRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        if (!IsHost)
        {
            OnHit(pos, normal, material);

        }
        OnHitClientRpc(pos, normal, material);
    }
    [ClientRpc]
    private void OnHitClientRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        OnHit(pos, normal, material);
    }
    private void OnShoot(float recoilForce)
    {
        //展示攻击特效和逻辑声音等
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        //展示音效
        weaponManager.GetAudioSource().Play();
        //后坐力只加在localplayer上
        if (IsLocalPlayer)
        {
            playerController.AddRecoilForce(recoilForce);
        }
    }
    //网络方面
    [ServerRpc]
    private void OnShootServerRpc(float recoilForce)
    {
        if (IsHost)
        {
            OnShoot(recoilForce);
        }
        
        OnShootClientRpc(recoilForce);
    }
    [ClientRpc]
    private void OnShootClientRpc(float recoilForce)
    {
        OnShoot(recoilForce);
    }

    private void Shoot()
    {
        autoshootCount++;
        float recoilForce = currentWeapon.recoilForce;

        if(autoshootCount <= 3)
        {
            recoilForce *= 0.2f;
        }

        OnShootServerRpc(recoilForce);
        //被击中物体的类
        RaycastHit hit;

        //物理类的射线函数,把击中物体的信息放在hit里面,mask分层，把队友分在一层，对手分在一层，可以防止击伤队友
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            //返回击中的名字
            //Debug.Log(hit.collider.name);
            //ShootServerRpc(hit.collider.name);
            if(hit.collider.tag == PLAYER_TAG)
            {
                ShootServerRpc(hit.collider.name, currentWeapon.damage);
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Metal);
            }
            else
            {
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Stone);
            }
        }
    }
    //加上这个注解可以让服务器和客户端通信,函数后缀必须为ServerRpc,必须继承networkbehavior类
    [ServerRpc]
    private void ShootServerRpc(string name,int damage)
    {
        Player player = GameManager.Singleton.GetPlayer(name);
        player.TakeDamage(damage);
    }
}
