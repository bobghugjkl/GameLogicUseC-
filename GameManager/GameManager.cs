using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;


    [SerializeField]
    public MatchingSettings MatchingSettings;
    

    //�������Ͽ�һ���ֵ���ÿ����ҵĶ�Ӧ��ϵ
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    //private static string info;
   
    //һ���ʼ����������awake��������
    private void Awake()
    {
        Singleton = this;
    }
    //����һ�����
    public void RegisterPlayer(string name,Player player)
    {
        //������,ע��
        player.transform.name = name;
        players.Add(name, player);
    }
    public void UnRegisterPlayer(string name)
    {
        //ɾ��
        players.Remove(name);
    }
    //��ȡһ�����
    public Player GetPlayer(string name)
    {
        return players[name];
    }

    //ÿ֡���ٵ���һ��,���𻭻�
    private void OnGUI()
    {
        //��������
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f));
        //����չʾ
        GUILayout.BeginVertical();
        GUI.color = Color.red;
        //����ĳһ������
        foreach (string name in players.Keys)
        {
            Player player = GetPlayer(name);
            //���
            GUILayout.Label(name+"-"+player.GetHealth());

        }
        //�պ�
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
