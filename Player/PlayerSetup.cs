using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Netcode;
using UnityEngine;
//需要网络的包
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentsToDisable;
    private Camera sceneCamera;

    // Start is called before the first frame update
    //执行网络连接的用法
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
            //本地玩家出现则关掉另一个摄像机
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                //如果有则让它不活跃
                sceneCamera.gameObject.SetActive(false);
            }
        }
        



        //将当前玩家加入到GameManager里面
        //取出名字
        string name = "Player" + GetComponent<NetworkObject>().NetworkObjectId.ToString();
       // transform.name = name;
        Player player = GetComponent<Player>();


        player.Setup();
        //singleton模式不需要static模式
        GameManager.Singleton.RegisterPlayer(name, player);
    }
    
    
    private void DisableComponents()
    {
        //不是本地玩家就全部禁用
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    //玩家退出
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        //玩家消失
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
