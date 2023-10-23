using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Netcode;
using UnityEngine;
//��Ҫ����İ�
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentsToDisable;
    private Camera sceneCamera;

    // Start is called before the first frame update
    //ִ���������ӵ��÷�
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    
    
        if (!IsLocalPlayer)
        {
            SetLayerMaskForAllChildren(transform, LayerMask.NameToLayer("Remote Player"));
/*
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
*/
            DisableComponents();
        }
        else
        {
            SetLayerMaskForAllChildren(transform, LayerMask.NameToLayer("Player"));
            //������ҳ�����ص���һ�������
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                //���������������Ծ
                sceneCamera.gameObject.SetActive(false);
            }
        }
        



        //����ǰ��Ҽ��뵽GameManager����
        //ȡ������
        string name = "Player" + GetComponent<NetworkObject>().NetworkObjectId.ToString();
       // transform.name = name;
        Player player = GetComponent<Player>();


        player.Setup();
        //singletonģʽ����Ҫstaticģʽ
        GameManager.Singleton.RegisterPlayer(name, player);
    }
    
    
    private void DisableComponents()
    {
        //���Ǳ�����Ҿ�ȫ������
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    //����˳�
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        //�����ʧ
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        GameManager.Singleton.UnRegisterPlayer(transform.name);
    }
   
    private void SetLayerMaskForAllChildren(Transform transform,LayerMask layerMask)
    {
        transform.gameObject.layer = layerMask;
        for(int i = 0; i < transform.childCount; i++)
        {
            SetLayerMaskForAllChildren(transform.GetChild(i), layerMask);
        }
    }



}
