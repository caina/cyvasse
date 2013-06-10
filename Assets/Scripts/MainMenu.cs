using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour{
	
	private string roomName = "myRoom";
    private Vector2 scrollPos = Vector2.zero;
	private bool isOffline = false;
	private bool isLocal = false;
	private string ip = "localhost";
	private int porta = 25000;
	public GameManager gameManager;
	
    void Awake()
    {
		gameManager = (GameManager) this.GetComponent<GameManager>();
		
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));

        //Set camera clipping for nicer "main menu" background
		//Camera.main.transform.rotation = 10;
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

	}

    void OnGUI(){
		
		if(isLocal)
			return;//chega dessa putaria da feevale bloquear o photon
		
		if (!PhotonNetwork.connected && !isOffline)
        {
            ShowConnectingGUI();
            return;//Wait for a connection
        }else if(isOffline){
			showOfflineGUI();	
			return;
		}	


        if ((PhotonNetwork.room != null))
            return; //Only when we're not in a Room


        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
        GUILayout.Label("Encontre ou crie uma sala");

        //Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Nome:", GUILayout.Width(150));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
        if (GUI.changed)//Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);


        //Join room by title
        GUILayout.BeginHorizontal();
        GUILayout.Label("Entrar na Sala:", GUILayout.Width(150));
        roomName = GUILayout.TextField(roomName);
        if (GUILayout.Button("Entrar"))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        GUILayout.EndHorizontal();

        //Create a room (fails if exist!)
        GUILayout.BeginHorizontal();
        GUILayout.Label("Criar Sala:", GUILayout.Width(150));
        roomName = GUILayout.TextField(roomName);
        if (GUILayout.Button("Criar"))
        {
            PhotonNetwork.CreateRoom(roomName, true, true, 2);
        }
        GUILayout.EndHorizontal();

        //Join random room
        GUILayout.BeginHorizontal();
        GUILayout.Label("Entrear em uma sala aleatoria:", GUILayout.Width(150));
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("..Nenhuma sala disponivel...");
        }
        else
        {
            if (GUILayout.Button("GO"))
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        GUILayout.Label("Lista de Salas:");
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("..Nenhuma sala disponivel..");
        }
        else
        {
            //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
                if (GUILayout.Button("Entrar"))
                {
                    PhotonNetwork.JoinRoom(game.name);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();
    }
	
	void OnFailedToConnectToPhoton(DisconnectCause cause) {
		this.GetComponent<Chat>().enabled = false;
        isOffline = true;	
	}
	
	void showOfflineGUI(){
		//Debug.Log(isLocal);
		if(Network.peerType == NetworkPeerType.Disconnected){
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
		        GUILayout.Label("Nao e possivel contatar o servidor Photon, conecte por LAN");
					GUILayout.BeginHorizontal();
				        GUILayout.Label("IP:", GUILayout.Width(50));
				        ip = GUILayout.TextField(ip);
						porta = int.Parse(GUILayout.TextField(porta.ToString()));
						if (GUILayout.Button("Entrar")){
							Network.Connect(ip,porta);
		                }
						if(GUILayout.Button ("Criar Servidor")){
							Network.InitializeServer(25,porta, false);
						}
		        GUILayout.EndHorizontal();
	        GUILayout.EndArea();	
		}else{
			Debug.Log("Connected to server");
			isLocal = true;
			isOffline = false;
			gameManager.isLocal = isLocal;
			gameManager.StartGame();
		}
	}


    void ShowConnectingGUI(){
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connectando com o Photon.");
        GUILayout.Label("Se falhar ao conectar, conecte usando a LAN.");
		

        GUILayout.EndArea();
    }
}
