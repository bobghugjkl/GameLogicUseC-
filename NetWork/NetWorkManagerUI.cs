
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using System.Net;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Networking;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;

public class NetWorkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button refreshButton;
    [SerializeField]
    private Button buildButton;
    [SerializeField]
    private GameObject roomButtonPrefab;
    [SerializeField]
    private Canvas menuUI;
    [SerializeField] private ushort serverPort = 7777;
    private List<Button> rooms = new List<Button>();

    private int buildRoomPort = -1;

    // Start is called before the first frame update
    void Start()
    {

        setConfig();
        initButtons();
        RefreshRoomList();
    }
    private void setConfig()
    {
        var args = System.Environment.GetCommandLineArgs();//返回所有的命令行参数

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-port")
            {
                int port = int.Parse(args[i + 1]);
                var transport = GetComponent<UnityTransport>();

            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-lauch-as-server")
            {
                //作为服务器
                NetworkManager.Singleton.StartServer();
                DestroyAllButtons();
            }
        }

    }
    private void initButtons()
    {
        refreshButton.onClick.AddListener(() =>
        {
            RefreshRoomList();
        });
        
        
    }

    private void RefreshRoomList()
    {
        StartCoroutine(RefreshRoomListRequest("http://121.41.59.128:8080/fps/get_room_list/"));
    }

    IEnumerator RefreshRoomListRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            //解析
            var resp = JsonUtility.FromJson<GetRoomListResponse>(uwr.downloadHandler.text);
            foreach (var room in rooms)
            {
                room.onClick.RemoveAllListeners();
                Destroy(room.gameObject);
            }
            rooms.Clear();

            int k = 0;
            foreach (var room in resp.rooms)
            {
                GameObject buttonObj = Instantiate(roomButtonPrefab, menuUI.transform);
                buttonObj.transform.localPosition = new Vector3(-21, 92 - k * 60, 0);
                Button button = buttonObj.GetComponent<Button>();
                button.GetComponentInChildren<TextMeshProUGUI>().text = room.name;
                button.onClick.AddListener(() =>
                {
                    var transport = GetComponent<UnityTransport>();
                    // transport.ConnectPort = transport.ServerListenPort = room.port;
                    //transport.GetComponentInChildren<Port>().tabIndex = room.port;
                    NetworkManager.Singleton.StartClient();
                    DestroyAllButtons();
                });
                rooms.Add(button);
                k++;
            }

        }
        
    }

    private void BuildRoom()
    {
        StartCoroutine(BuildRoomRequest("http://121.41.59.128/:8080/fps/build_room/"));
    }

    IEnumerator BuildRoomRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {

            var resp = JsonUtility.FromJson<BuildRoomResponse>(uwr.downloadHandler.text);
            if (resp.error_message == "success")
            {
                var transport = GetComponent<UnityTransport>();
                transport.GetComponentInChildren<Port>().tabIndex = transport.GetComponentInChildren<Port>().tabIndex = resp.port;
                buildRoomPort = resp.port;
                NetworkManager.Singleton.StartClient();
                DestroyAllButtons();
            }
        }
    }

    private void RemoveRoom()
    {
        StartCoroutine(RemoveRoomRequest("http://121.41.59.128/:8080/fps/remove_room/?port=" + buildRoomPort.ToString()));
    }

    IEnumerator RemoveRoomRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            var resp = JsonUtility.FromJson<RemoveRoomResponse>(uwr.downloadHandler.text);

            if (resp.error_message == "success")
            {

            }
        }
    }

    

    private void DestroyAllButtons()
    {
        refreshButton.onClick.RemoveAllListeners();
        buildButton.onClick.RemoveAllListeners();
        Destroy(refreshButton.gameObject);
        Destroy(buildButton.gameObject);
        foreach (var room in rooms)
        {
            room.onClick.RemoveAllListeners();
            Destroy(room.gameObject);
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
