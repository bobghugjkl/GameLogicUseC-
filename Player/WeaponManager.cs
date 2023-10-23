using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon;
    [SerializeField]
    private PlayerWeapon secondaryWeapon;
    [SerializeField]
    //将武器渲染,生成出来挂在weaponholder里面
    private GameObject weaponHolder;

    private AudioSource currentAudioSource;

    //开一个对象维护哪种武器
    private PlayerWeapon currentWeapon;
    //当前特效
    private WeaponGraphics currentGraphics;

    // Start is called before the first frame update
    void Start()
    {
        //一开始先装备主武器
        EquipWeapon(primaryWeapon);
    }
    //一个赋值函数装备武器
    public void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;

        //切武器时卸掉
        if(weaponHolder.transform.childCount > 0)
        {
            //把儿子立即删掉
            Destroy(weaponHolder.transform.GetChild(0).gameObject);
        }

        //实例化枪，传入初始坐标角度
        GameObject weaponObject = Instantiate(currentWeapon.graphics,weaponHolder.transform.position,weaponHolder.transform.rotation);
        //挂到上面
        weaponObject.transform.SetParent(weaponHolder.transform);

        //把武器特效取出来
        currentGraphics = weaponObject.GetComponent<WeaponGraphics>();
        currentAudioSource = weaponObject.GetComponent<AudioSource>();
        //特判，如果是自己射击的是2D音效
        if(IsLocalPlayer)
        {
            currentAudioSource.spatialBlend = 0f;
        }
    }
    //接口，返回当前武器
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
    //动态返回当前特效
    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }
    public AudioSource GetAudioSource()
    {
        return currentAudioSource;
    }

    //赋值函数求武器
    public void ToggleWeapon()
    {
        if(currentWeapon == primaryWeapon)
        {
            EquipWeapon(secondaryWeapon);
        }
        else
        {
            EquipWeapon(primaryWeapon);
        }
    }
    [ClientRpc]
    private void ToggleWeaponClientRpc()
    {
        ToggleWeapon();
        
    }
    [ServerRpc]
    private void ToggleWeaponServerRpc()
    {
        if (!IsHost)
        {
            ToggleWeapon();
        }
        //切枪信息同步服务器
        //ToggleWeapon();
        //先切好再换client
        ToggleWeaponClientRpc();
    }
    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleWeaponServerRpc();
            }
        }
    }
}
