using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlyerShutting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    //ֻ��Ҫ֪����������������
    private WeaponManager weaponManager;
    private PlayerWeapon currentWeapon;
    private float shootCoolDownTime = 0f;//�����ϴο�ǹ���˶��
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    //��������
    private PlayerController playerController;
    private Player player;
    //��¼���˶���ǹ
    private int autoshootCount = 0;
    //ö��
    enum HitEffectMaterial
    {
        Metal,
        Stone,
    }
    // Start is called before the first frame update
    void Start()
    {
        //���ǻ�ȡ�����,camera��player����Ԫ������
        cam = GetComponentInChildren<Camera>();
        //��̬��ȡ
        weaponManager = GetComponent<WeaponManager>();
        playerController = GetComponent<PlayerController>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //�����������β���ʱ��
        shootCoolDownTime += Time.deltaTime;
        if (!IsLocalPlayer) return;
        //�ȿ���������˭�����֮ǰ�����û�ȡ
        currentWeapon = weaponManager.GetCurrentWeapon();


        if(currentWeapon.shootRate <= 0)//����
        {
            //��ȡ�û�����
            if (Input.GetButtonDown("Fire1")&& shootCoolDownTime>=currentWeapon.shootCoolDownTime&&player.IsDead()==false)
            {
                autoshootCount = 0;
                Shoot();
                shootCoolDownTime = 0f;//������ȴ
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
    private void OnHit(Vector3 pos,Vector3 normal,HitEffectMaterial material)//������Ч
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
        //��̬��������,Instantiateʵ����,����������
        GameObject hitEffectObject = Instantiate(hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        ParticleSystem particleSystem = hitEffectObject.GetComponent<ParticleSystem>();
        //ֱ�Ӵ���
        particleSystem.Emit(1);
        particleSystem.Play();
        Destroy(hitEffectObject,1f);
    }
    //���������ͨ��
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
        //չʾ������Ч���߼�������
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        //չʾ��Ч
        weaponManager.GetAudioSource().Play();
        //������ֻ����localplayer��
        if (IsLocalPlayer)
        {
            playerController.AddRecoilForce(recoilForce);
        }
    }
    //���緽��
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
        //�������������
        RaycastHit hit;

        //����������ߺ���,�ѻ����������Ϣ����hit����,mask�ֲ㣬�Ѷ��ѷ���һ�㣬���ַ���һ�㣬���Է�ֹ���˶���
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            //���ػ��е�����
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
    //�������ע������÷������Ϳͻ���ͨ��,������׺����ΪServerRpc,����̳�networkbehavior��
    [ServerRpc]
    private void ShootServerRpc(string name,int damage)
    {
        Player player = GameManager.Singleton.GetPlayer(name);
        player.TakeDamage(damage);
    }
}
