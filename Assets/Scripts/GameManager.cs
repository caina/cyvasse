using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";
	public bool isFirstPlayer = true;
	private Chat chat;
	public Board gameBoard;
	
	private int playerId = 1;
	public int playerRound = 0;
	public int playerTime;
	
	public bool isLocal = false;
	private PhotonView photonView;
	private bool onMountPhase = true;
	
	private int playersReady = 0;
	private bool imReady = false;
	public GameObject gameGate;
	private bool canPlay = true;
	
	public int rounds;

	
	void Start(){
		photonView = (PhotonView) this.GetComponent<PhotonView>();	
	}
		
    void OnJoinedRoom()
    {
        StartGame();
		chat =  this.GetComponent<Chat>();
		SendChatMessage("Entrou na sala");
    }
	
	void SendChatMessage(string message){
		if(PhotonNetwork.connected){
			chat.chatInput = message;
			chat.SendChat(PhotonTargets.All);	
		}
		
	}
	
	/***
	 * Muda o turno atual.
	 * Comeca sempre com o player 1*/
	/*
	public void nextRound(){
		photonView.RPC("changeRound",PhotonTargets.All);
	}*/
	
	public void changeRound(){
		if(onMountPhase)
			return;
		
		gameBoard.returnPiecesNaturalState();
		if(PhotonNetwork.connected){
			photonView.RPC("_changeRound", PhotonTargets.All);
		}else{
			networkView.RPC("_changeRound",RPCMode.All);
		}
	}
	//usado apenas para comecar o jogo
	[RPC]
	void startRounds(){
		playerRound = 1;
		
		Color lightColor = new Color(188/255.0f,53/255.0f,53/255.0f,0);
		if(playerRound == 1){
			lightColor = new Color(53/255.0f,166/255.0f,188/255.0f,0);
		}
		
		GameObject[] lights =  GameObject.FindGameObjectsWithTag("Lights");
		foreach(GameObject light in lights){
			iTween.ColorTo(light,lightColor,.3f);	
		}
	}
	
	[RPC]
	void _changeRound(){
		playerRound = playerRound == 1?2:1;
		
		Color lightColor = new Color(188/255.0f,53/255.0f,53/255.0f,0);
		if(playerRound == 1){
			lightColor = new Color(53/255.0f,166/255.0f,188/255.0f,0);
		}
		
		GameObject[] lights =  GameObject.FindGameObjectsWithTag("Lights");
		foreach(GameObject light in lights){
			iTween.ColorTo(light,lightColor,.3f);	
		}
		
		rounds++;
	}
	
	/***
	 * Compara a rodada atual com o 
	 * identificador do jogador.
	 * */
	public bool isMyRound(){
		if(onMountPhase) 
			return true;
		
		return playerId == playerRound;
	}
    
    IEnumerator OnLeftRoom()
    {
        //Easy way to reset the level: Otherwise we'd manually reset the camera

        //Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;

        Application.LoadLevel(Application.loadedLevel);

    }
	
    public void StartGame(){
		Camera.main.farClipPlane = 1000; 
		if(PhotonNetwork.connected){
			photonView.RPC("PlayerConnected", PhotonTargets.All);
		}else{
			networkView.RPC("PlayerConnected",RPCMode.All);
		}
		
    }
	
	[RPC]
	void PlayerConnected(){
		if(isFirstPlayer){
			isFirstPlayer = false;
			playerTime = 0;
			gameBoard.playerNumber = 1;
		}else{
		/*
			Camera.main.transform.position = new Vector3(0f,6.84f,-5.7f);
			Vector3 rot = transform.rotation.eulerAngles;
			Camera.main.transform.rotation = Quaternion.Euler(53.65f,rot.x,rot.y);	
			
			*/
			gameBoard.playerNumber = 2;
			playerTime = 1;
			RequestshuffleParts();
		
		}
		//RequestshuffleParts();
	}
	
	
	void RequestshuffleParts(){
		//gameBoard.ShuffleParts();
		SendChatMessage("embaralharemos as pecas! preparem-se!");
		
	}
	
	private int playerCount = 1;
    void OnPlayerConnected(NetworkPlayer player) {
        playerCount++;
		playerId++;
		if(playerCount==2){
			if(PhotonNetwork.connected){
				photonView.RPC("letsBegin", PhotonTargets.All);
			}else{
				networkView.RPC("letsBegin",RPCMode.All);
			}
			
		}
    }
	
	[RPC]
	void letsBegin(){
		canPlay = true;
		Camera.main.farClipPlane = 1000; 
		if(playerId==2){
			Camera.main.transform.position = new Vector3(8.040245f,8.194799f,-0.0770278f);
			Camera.main.transform.rotation = Quaternion.Euler(41.20966f, 270.1824f, 359.9654f);
		}
		startRounds();
		// YUNO funciona?!!?!?!?
		//Camera.main.gameObject.GetComponent<MouseOrbit>().enabled = true;
		
	}
	
    void OnGUI()
    {
		if(!canPlay & hasConnection()){
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
		        GUILayout.Label("Aguardando o oponente");
	        GUILayout.EndArea();
		}
		
		GUILayout.BeginArea(new Rect(0, 0, 150, 500));
			GUILayout.BeginHorizontal();
				if(!imReady && hasConnection() && canPlay){
					if(GUILayout.Button ("Estou Pronto")){
						imReady = true;
						playerIsReady();
					}
				}
	        GUILayout.EndHorizontal();
        GUILayout.EndArea();	
		
        
		
		if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        if (GUILayout.Button("Leave& QUIT"))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
	
	public bool hasConnection(){
		if(PhotonNetwork.connected){
			return true;
		}else{
			return !(Network.peerType == NetworkPeerType.Disconnected);
		}
		
	}
	
	void OnPhotonPlayerDisconnected(PhotonPlayer player){
    	SendChatMessage("Player Desconectado: " + player);
	}

    void OnDisconnectedFromPhoton()
    {
        SendChatMessage("Disconectado do servidor");
		Debug.LogWarning("OnDisconnectedFromPhoton");
    }
    void OnFailedToConnectToPhoton()
    {
       // Debug.LogWarning("OnFailedToConnectToPhoton");
    }
	
	public bool isOnMountPhase(){
		return onMountPhase;
	}
	
	void playerIsReady(){
		
		//MouseOrbit orbit = cam.gameObject.GetComponent<MouseOrbit>();
		//orbit.target = rotationExio.transform;
		//cam.GetComponent<MouseOrbit>().enabled = true;
		
		if(PhotonNetwork.connected){
			photonView.RPC("_playerIsReady", PhotonTargets.All);
		}else{
			networkView.RPC("_playerIsReady",RPCMode.All);
		}
	}
	
	[RPC]
	void _playerIsReady(){
		playersReady++;
		if(playersReady == 2){
			Destroy(gameGate);
			onMountPhase = false;
			
		}
		//TODO COLOCAR NO IF DEPOIS DE TERMINAR OS TESTESS
		Camera cam = (Camera)FindObjectOfType(typeof(Camera));
		cam.gameObject.AddComponent("MouseOrbit");
		gameBoard.hintText = "Use o botao direito para girar o cenario!";
	}
	
	public int getPlayerId(){
		return playerId;	
	}
  
}
