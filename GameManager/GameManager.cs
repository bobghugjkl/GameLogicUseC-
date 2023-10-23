using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;


    [SerializeField]
    public MatchingSettings MatchingSettings;
    

    //服务器上开一个字典找每名玩家的对应关系
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    //private static string info;
   
    //一般初始化变量都在awake函数里面
    private void Awake()
    {
        Singleton = this;
    }
    //加入一个玩家
    public void RegisterPlayer(string name,Player player)
    {
        //重命名,注册
        player.transform.name = name;
        players.Add(name, player);
    }
    public void UnRegisterPlayer(string name)
    {
        //删除
        players.Remove(name);
    }
    //获取一名玩家
    public Player GetPlayer(string name)
    {
        return players[name];
    }

    //每帧至少调用一次,负责画画
    private void OnGUI()
    {
        //区域作画
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f));
        //竖排展示
        GUILayout.BeginVertical();
        GUI.color = Color.red;
        //遍历某一个集合
        foreach (string name in players.Keys)
        {
            Player player = GetPlayer(name);
            //输出
            GUILayout.Label(name+"-"+player.GetHealth());

        }
        //闭合
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
