using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{	
	public GameObject pieceKillEffect;
	
	GameObject currentTarget=null;
	private GameObject targetKill;
	public GameObject ball;	
	public GameObject tile;
	public GameObject myTile;
	public int rate = 10;
	private int numberPlayersPieces = 11;
	public bool ballSet = true;
	public GameObject[,] gameTiles = new GameObject[10,10];
	public GameObject[] gamePieces = new GameObject[100];
	public GameManager gameManager;
	public int selectedPiece;
	private PhotonView photonView;
	public int playerNumber;
	private int createdGrounds = 0;
	
	
	public GameObject groundOfSelected = null;
	
	void Start ()
	{	
		CreateGameBoard(10,10);
		photonView = (PhotonView) this.GetComponent<PhotonView>();
		ShuffleParts();
	}
	
	/*
	public void SetTarget(GameObject target){
		if(currentTarget != null && target!=currentTarget){
			currentTarget.SendMessage("Deactivate");
		}
		currentTarget=target;
		ballSet=false;
		ball = getSelectedPiece();
			}
	**/
	public void SetTarget(GameObject target){
		if(currentTarget != null && target!=currentTarget){
			currentTarget.SendMessage("Deactivate");
		}
		
		if(!hasPieceSelected()){
			return;	
		}
		
		if(target.GetComponent<GameTile>().isFrozen()){
			return;
		}
		
		int x = target.GetComponent<GameTile>().x;
		int z = target.GetComponent<GameTile>().z;
		
		currentTarget=target;
		ballSet=false;
		Vector3 position = target.transform.position;
		if(PhotonNetwork.connected){
			photonView.RPC("movePiece", PhotonTargets.All,currentTarget.transform.position,x,z);
		}else{
			networkView.RPC("movePiece",RPCMode.All,currentTarget.transform.position,x,z);	
		}
	}
	
	[RPC]
	public void movePiece(Vector3 targetPosition, int x,int z){
		ball = getSelectedPiece();
		tile = (GameObject) gameTiles[x,z];
		myTile = (GameObject) gameTiles[ball.GetComponent<GamePiece>().onMeX, ball.GetComponent<GamePiece>().onMeZ];
		Vector3 tilePosition = tile.gameObject.transform.position;
		if(canPerformMove(myTile,tile, ball.GetComponent<GamePiece>().pieceMaxMoves)){
			
			myTile.GetComponent<GameTile>().onMeId = 99999;
					
			tile.GetComponent<GameTile>().onMeId = ball.GetComponent<GamePiece>().id;
			ball.GetComponent<GamePiece>().onMeX = x;
			ball.GetComponent<GamePiece>().onMeZ = z;
			
			float travelTime = Vector3.Distance(ball.transform.position, targetPosition)/rate;
			iTween.MoveBy(ball,iTween.Hash("x",targetPosition.x-ball.transform.position.x,"easetype","easeinoutsine","time",travelTime));
			iTween.MoveBy(ball,iTween.Hash("z",targetPosition.z-ball.transform.position.z,"time",travelTime,"delay",travelTime,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
		}else{
			Debug.Log("Nao posso ir tao longe =(");	
			ball = null;
			tile = null;
			myTile = null;
			ballSet=true;
		}
	}
	
	
	
	void Reset(){
		ballSet=true;
	}
		
	void CreateGameBoard(uint cols, uint rows){
		GameObject block = (GameObject)Resources.Load("GameTile");
		
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				GameObject newBlock = (GameObject)Instantiate(block,new Vector3(i,0,j),Quaternion.identity);
				newBlock.name="Block: " + i + "," + j;
				newBlock.GetComponent<GameTile>().x = i;
				newBlock.GetComponent<GameTile>().z = j;
				newBlock.SendMessage("SetGameboard",this);
				
				Texture texture;
				if((j+i)%2 == 0){
					texture = (Texture)Resources.Load("textures/light_grass");
					newBlock.renderer.material.SetTexture("_MainTex",texture);	
					newBlock.GetComponent<GameTile>().defaultTexture = texture; 
				}else{
					texture = (Texture)Resources.Load("textures/strong_grass");
					newBlock.renderer.material.SetTexture("_MainTex",texture);
					newBlock.GetComponent<GameTile>().defaultTexture = texture; 
				}
			
				newBlock.transform.parent=transform;
				
				gameTiles[i,j] = newBlock;
			}
		}
		transform.position= new Vector3(-cols/2,0,-rows/2);
	}
	
	
	/***
	 * Verifica se o turno e meu antes de 
	 * fazer qualquer acao*/
	
	
	/***
	 * Pesquisar o que tem na tile
	 * nestas cordenadas, e retorna 
	 * sim ou nao
	 * */
	public bool thereIsSomethingHere(uint col, uint row){
		
		return false;
	}
	
	/***
	 * Pega o GameObject do que tem nessa tile
	 * */
	public GameObject whatIsHere(uint col, uint row){
		return new GameObject();
	}
	
	
	public void ShuffleParts(){
		//int lastZ=-5;
		//int lastX=-5;
		//int last_direction=-1;
		Vector3 ballPosition;
		int currentPlayerTime = 1;
		int createdObjects = 1;
		
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 9; j++) {

				if(createdObjects < numberPlayersPieces && currentPlayerTime == 1){
					if(createdObjects==10)
						currentPlayerTime = 2;
					
					
					
						GameObject ball = null;
						GameObject tile = gameTiles[i,j];
						ballPosition = tile.transform.position;
						tile.GetComponent<GameTile>().onMeId =createdObjects; 
						ballPosition.y = .5f;
						ball = (GameObject)Instantiate((GameObject)Resources.Load("GameBall"), ballPosition,Quaternion.identity);
						
						ball.GetComponent<GamePiece>().id = createdObjects;
						ball.GetComponent<GamePiece>().onMeX = i;
						ball.GetComponent<GamePiece>().onMeZ = j;
						ball.GetComponent<GamePiece>().myType = 1;
						ball.GetComponent<GamePiece>().maxRangeAtack = 1;
						
						
						if(createdObjects ==1){
							ball.GetComponent<GamePiece>().pieceMaxMoves = 7;
							ball.renderer.material.color=Color.black;
						}
						
						ball.GetComponent<GamePiece>().playerBelong = currentPlayerTime;
						ball.name= "Peca "+i.ToString() + " - "+ j.ToString();
						ball.SendMessage("SetGameboard",this);
						gamePieces[createdObjects] = ball;
						createdObjects++;
				}else{
				
					//after create all this pieces, is time to create the ground
					createGround(i,j);
				
				}
				
				
				
			}
		}
		createdGrounds = 0;
		for (int i = 9; i > 5; i--) {
			for (int j = 9; j > 0; j--) {

				if(createdObjects < (numberPlayersPieces+numberPlayersPieces) && currentPlayerTime == 2){
										
					GameObject ball = null;
					GameObject tile = gameTiles[i,j];
					ballPosition = tile.transform.position;
					tile.GetComponent<GameTile>().onMeId =createdObjects; 
					ballPosition.y = .5f;
					ball = (GameObject)Instantiate((GameObject)Resources.Load("GameBall"), ballPosition,Quaternion.identity);
					
					ball.GetComponent<GamePiece>().id = createdObjects;
					ball.GetComponent<GamePiece>().onMeX = i;
					ball.GetComponent<GamePiece>().onMeZ = j;
					ball.GetComponent<GamePiece>().myType = 1;
					ball.GetComponent<GamePiece>().maxRangeAtack = 1;
					
					ball.GetComponent<GamePiece>().playerBelong = currentPlayerTime;
					ball.name= "Peca "+i.ToString() + " - "+ j.ToString();
					ball.SendMessage("SetGameboard",this);
					gamePieces[createdObjects] = ball;
					createdObjects++;
				}else{
				
					//after create all this pieces, is time to create the ground
					createGround(i,j);
					
				}
			}
		}
		
	}
	
	void createGround(int x, int z){
		Debug.Log(createdGrounds);
		
		if(createdGrounds <= 5  ){
			//montanhas
			((GameObject)gameTiles[x,z]).GetComponent<GameTile>().changeType("montain");
			createdGrounds++;
		}else if(createdGrounds > 5 && createdGrounds < 8){
			((GameObject)gameTiles[x,z]).GetComponent<GameTile>().changeType("water");
			createdGrounds++;
		}else if(createdGrounds >= 8 && createdGrounds < 11){
			((GameObject)gameTiles[x,z]).GetComponent<GameTile>().changeType("forest");
			createdGrounds++;
		}
		
		
	}
	
	public GameObject getSelectedPiece(){
		return gamePieces[selectedPiece];	
	}
	public bool hasPieceSelected(){
		return gamePieces[selectedPiece]!=null;
	}
	public void selectPiece(GameObject pieceGameObject){
		int x = pieceGameObject.GetComponent<GamePiece>().onMeX;
		int z = pieceGameObject.GetComponent<GamePiece>().onMeZ;
		
		//Debug.Log(pieceGameObject.GetComponent<GamePiece>().playerBelong);
		
		
		if(hasPieceSelected() && (pieceGameObject.GetComponent<GamePiece>().playerBelong != playerNumber)){
			atackPiece(pieceGameObject);
			ballSet = true;
		}else{
			if(gameManager.getPlayerId() != pieceGameObject.GetComponent<GamePiece>().playerBelong)
				return;//nao eh tua peca
				
				
			GameObject tile = gameTiles[x,z];
			Debug.Log(tile.gameObject.transform.position.x.ToString() + "|" +tile.gameObject.transform.position.z.ToString() + tile.gameObject.name.ToString());
			
			int piece = pieceGameObject.GetComponent<GamePiece>().id;
			if(PhotonNetwork.connected){
				photonView.RPC("_selectPiece", PhotonTargets.All,piece);
			}else{
				networkView.RPC("_selectPiece",RPCMode.All,piece);	
			}	
		}
	}
	
	[RPC]
	void _selectPiece(int piece){
		if(hasPieceSelected())
			getSelectedPiece().renderer.material.color=Color.blue;
		this.selectedPiece = piece;	
		getSelectedPiece().renderer.material.color=Color.yellow;
	}
	
	
	
	GameObject me;
	void atackPiece(GameObject target){
		me = (GameObject) getSelectedPiece();
		//Debug.Log (me.gameObject.name.ToString()+">>aaaaaaaa");
		
		GameObject myTile = gameTiles[me.GetComponent<GamePiece>().onMeX, me.GetComponent<GamePiece>().onMeZ];
		GameObject targetTile = gameTiles[target.GetComponent<GamePiece>().onMeX, target.GetComponent<GamePiece>().onMeZ];
		
		int movesCanPerform = me.GetComponent<GamePiece>().maxRangeAtack;
		int myX 	= myTile.GetComponent<GameTile>().x;
		int myZ		= myTile.GetComponent<GameTile>().z;
		int targetX = targetTile.GetComponent<GameTile>().x;
		int targetZ	= targetTile.GetComponent<GameTile>().z;
		
		Debug.Log (myX.ToString() + "-" + myZ.ToString() + "|" + targetX.ToString() + "-"+targetZ.ToString());
		bool canAtack = false;
		
		if(myZ != targetZ && myX != targetX){
			canAtack = false;	
		}
		
		if(myZ == targetZ){
			//so calcular o x!
			if(myX < targetX){
				//	na direita
				if((myX + movesCanPerform) >= targetX){
					canAtack = true;
				}
			}else{
				//na esquerda
				if((myX - movesCanPerform) <= targetX){
					canAtack = true;
				}
			}
		}else if(myX == targetX){
			//so calcular o Z
			if(myZ < targetZ){
				//	a cima
				if((myZ + movesCanPerform) >= targetZ){
					canAtack = true;
				}
			}else{
				//a baixo
				if((myZ - movesCanPerform) <= targetZ){
					canAtack = true;
				}
			}
		}
		if(canAtack){
			if(PhotonNetwork.connected){
				photonView.RPC("_atackRpc", PhotonTargets.All,target.GetComponent<GamePiece>().id);
			}else{
				networkView.RPC("_atackRpc",RPCMode.All,target.GetComponent<GamePiece>().id);	
			}	
		}
		
	}
	
	[RPC]
	void _atackRpc(int pieceId){
		targetKill = (GameObject) gamePieces[pieceId];
		Vector3 positionExplode = targetKill.gameObject.transform.position;
		GameObject killedTile = getTileByPiece(targetKill);
		killedTile.GetComponent<GameTile>().onMeId = 99999;
				
		Destroy(targetKill.gameObject);
		
		gamePieces[pieceId] = null;
		Instantiate(pieceKillEffect, positionExplode,Quaternion.identity);
		
		SetTarget(killedTile);
	}
	
	public bool canPerformMove(GameObject myTile, GameObject targetTile, int movesCanPerform){
		int myX 	= myTile.GetComponent<GameTile>().x;
		int myZ		= myTile.GetComponent<GameTile>().z;
		int targetX = targetTile.GetComponent<GameTile>().x;
		int targetZ	= targetTile.GetComponent<GameTile>().z;
		
		if(myZ != targetZ && myX != targetX){
			return false;	
		}
		
		if(targetTile.GetComponent<GameTile>().onMeId != 99999){
			Debug.Log("tem alguma coisa na frente");
			/*
			if((gamePieces[targetTile.GetComponent<GameTile>().onMeId]).GetComponent<GamePiece>().playerBelong != playerNumber){
				Debug.Log("executar funcao de matar inimigo!!!!");
				return true;
			}*/
			return false;
		}
		
		if(myZ == targetZ){
			//so calcular o x!
			if(myX < targetX){
				//	na direita
				if((myX + movesCanPerform) >= targetX){
					return true;
				}
			}else{
				//na esquerda
				if((myX - movesCanPerform) <= targetX){
					return true;
				}
			}
		}else{
			//so calcular o Z
			if(myZ < targetZ){
				//	a cima
				if((myZ + movesCanPerform) >= targetZ){
					return true;
				}
			}else{
				//a baixo
				if((myZ - movesCanPerform) <= targetZ){
					return true;
				}
			}
		}
		return false;
	}
	
	public void hideMovementOptions(){
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
				gameTiles[i,j].GetComponent<GameTile>().hideOptions();
			}
		}
	}
	public void showMovementOptions(GameObject selectedPiece){
		Debug.Log("slecionei");
		GameObject tile = getTileByPiece(selectedPiece);
		if(tile==null)
			return;
		
		int x = tile.GetComponent<GameTile>().x;
		int z = tile.GetComponent<GameTile>().z;
		int movements = selectedPiece.GetComponent<GamePiece>().pieceMaxMoves;
		
		int xDir = x;
		int xEsq = x;
		int zDir = z;
		int zEsq = z;
		while(movements > 0){
			//direita
			
			xDir ++;
			xEsq --;
			zDir ++;
			zEsq --;
			
			try{
				((GameObject) gameTiles[xDir,z]).GetComponent<GameTile>().isOption();	
			}catch{}
			try{
				((GameObject) gameTiles[xEsq,z]).GetComponent<GameTile>().isOption();	
			}catch{}
			try{
				((GameObject) gameTiles[x,zDir]).GetComponent<GameTile>().isOption();	
			}catch{}
			try{
				((GameObject) gameTiles[x,zEsq]).GetComponent<GameTile>().isOption();
			}catch{}
				
						
			movements--;	
		}
		
	}
	
	
	GameObject getTileByPiece(GameObject piece){
		try{
			return (GameObject) gameTiles[piece.GetComponent<GamePiece>().onMeX, piece.GetComponent<GamePiece>().onMeZ];		
		}catch{
			return null;
		}
		
	}
	
	public void rpcChangeGrounds(int toX,int toZ){
		int fromX = groundOfSelected.GetComponent<GameTile>().x;
		int fromZ = groundOfSelected.GetComponent<GameTile>().z;
		groundOfSelected = gameTiles[toX, toZ];
		
		if(groundOfSelected.GetComponent<GameTile>().hasGround())
			return;
		
		if(PhotonNetwork.connected){
			photonView.RPC("_rpcChangeGrounds", PhotonTargets.All,fromX, fromZ, toX, toZ);
		}else{
			networkView.RPC("_rpcChangeGrounds",RPCMode.All,fromX, fromZ, toX, toZ);	
		}
		
	}
	
	[RPC]
	public void _rpcChangeGrounds(int fromX, int fromZ,int toX,int toZ){
		GameObject fromTile = gameTiles[fromX, fromZ];
		GameObject toTile	= gameTiles[toX, toZ];
		
		toTile.GetComponent<GameTile>().migrateTypes(fromTile);
	}
	
}

