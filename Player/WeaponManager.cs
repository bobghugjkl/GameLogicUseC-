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
    //��������Ⱦ,���ɳ�������weaponholder����
    private GameObject weaponHolder;

    private AudioSource currentAudioSource;

    //��һ������ά����������
    private PlayerWeapon currentWeapon;
    //��ǰ��Ч
    private WeaponGraphics currentGraphics;

    // Start is called before the first frame update
    void Start()
    {
        //һ��ʼ��װ��������
        EquipWeapon(primaryWeapon);
    }
    //һ����ֵ����װ������
    public void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;

        //������ʱж��
        if(weaponHolder.transform.childCount > 0)
        {
            //�Ѷ�������ɾ��
            Destroy(weaponHolder.transform.GetChild(0).gameObject);
        }

        //ʵ����ǹ�������ʼ����Ƕ�
        GameObject weaponObject = Instantiate(currentWeapon.graphics,weaponHolder.transform.position,weaponHolder.transform.rotation);
        //�ҵ�����
        weaponObject.transform.SetParent(weaponHolder.transform);

        //��������Чȡ����
        currentGraphics = weaponObject.GetComponent<WeaponGraphics>();
        currentAudioSource = weaponObject.GetComponent<AudioSource>();
        //���У�������Լ��������2D��Ч
        if(IsLocalPlayer)
        {
            currentAudioSource.spatialBlend = 0f;
        }
    }
    //�ӿڣ����ص�ǰ����
    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
    //��̬���ص�ǰ��Ч
    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }
    public AudioSource GetAudioSource()
    {
        return currentAudioSource;
    }

    //��ֵ����������
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
        //��ǹ��Ϣͬ��������
        //ToggleWeapon();
        //���к��ٻ�client
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
