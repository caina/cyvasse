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
	private int numberPlayersPieces = 15;
	public bool ballSet = true;
	public GameObject[,] gameTiles = new GameObject[11,11];
	public GameObject[] gamePieces = new GameObject[100];
	public GameManager gameManager;
	public int selectedPiece;
	private PhotonView photonView;
	public int playerNumber;
	private int createdGrounds = 0;
	
	public string hintText="";
	
	public GameObject groundOfSelected = null;
	
	void Start ()
	{	
		CreateGameBoard(10,10);
		photonView = (PhotonView) this.GetComponent<PhotonView>();
		ShuffleParts();
	}
	
	
	void OnGUI()
    {
		GUILayout.BeginArea(new Rect(15, Screen.height-25, 250, 250));
			GUILayout.BeginHorizontal();
				GUILayout.Label(hintText);
	        GUILayout.EndHorizontal();
        GUILayout.EndArea();	
		
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
		ball = getSelectedPiece();
		
		Vector3 position = target.transform.position;
		
		
		tile = (GameObject) gameTiles[x,z];
		myTile = (GameObject) gameTiles[ball.GetComponent<GamePiece>().onMeX, ball.GetComponent<GamePiece>().onMeZ];
		//Vector3 tilePosition = tile.gameObject.transform.position;
		
		if(canPerformMove(ball,myTile,tile, ball.GetComponent<GamePiece>().pieceMaxMoves)){
			if(PhotonNetwork.connected){
				photonView.RPC("movePiece", PhotonTargets.All,currentTarget.transform.position,x,z,ball.GetComponent<GamePiece>().id);
			}else{
				networkView.RPC("movePiece",RPCMode.All,currentTarget.transform.position,x,z,ball.GetComponent<GamePiece>().id);	
			}
			gameManager.changeRound();
		}else{
			hintText="Personagem nao chega no destino.";
			Debug.Log("Nao posso ir tao longe =(");	
			ball = null;
			tile = null;
			myTile = null;
			ballSet=true;
		}
		
		/*
		if(PhotonNetwork.connected){
			photonView.RPC("movePiece", PhotonTargets.All,currentTarget.transform.position,x,z,ball.GetComponent<GamePiece>().id);
		}else{
			networkView.RPC("movePiece",RPCMode.All,currentTarget.transform.position,x,z,ball.GetComponent<GamePiece>().id);	
		}*/
	}
	
	[RPC]
	public void movePiece(Vector3 targetPosition, int x,int z, int objPos){
		ball = gamePieces[objPos];
		tile = (GameObject) gameTiles[x,z];
		myTile = (GameObject) gameTiles[ball.GetComponent<GamePiece>().onMeX, ball.GetComponent<GamePiece>().onMeZ];
		//Vector3 tilePosition = tile.gameObject.transform.position;
		
		myTile.GetComponent<GameTile>().onMeId = 99999;
					
		tile.GetComponent<GameTile>().onMeId = ball.GetComponent<GamePiece>().id;
		ball.GetComponent<GamePiece>().onMeX = x;
		ball.GetComponent<GamePiece>().onMeZ = z;
		
		float travelTime = Vector3.Distance(ball.transform.position, targetPosition)/rate;
		//iTween.MoveTo(ball, iTween.Hash("position", tile.transform.position, "islocal", true, "time", 1,"oncomplete","Reset"));
		//iTween.MoveBy(ball,iTween.Hash("x",targetPosition.x-ball.transform.position.x,"easetype","easeinoutsine","time",travelTime));
		//iTween.MoveBy(ball,iTween.Hash("z",targetPosition.z-ball.transform.position.z,"time",travelTime,"delay",0,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
		
		iTween.MoveBy(ball,iTween.Hash("x",targetPosition.x-ball.transform.position.x,"easetype","easeinoutsine","time",travelTime));
		iTween.MoveBy(ball,iTween.Hash("z",targetPosition.z-ball.transform.position.z,"time",travelTime,"delay",travelTime,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
		
	}
	
	/***
	 * 
	 * [RPC]
	public void movePiece(Vector3 targetPosition, int x,int z, int objPos){
		ball = gamePieces[objPos];
		tile = (GameObject) gameTiles[x,z];
		myTile = (GameObject) gameTiles[ball.GetComponent<GamePiece>().onMeX, ball.GetComponent<GamePiece>().onMeZ];
		Vector3 tilePosition = tile.gameObject.transform.position;
		if(canPerformMove(ball,myTile,tile, ball.GetComponent<GamePiece>().pieceMaxMoves)){
			
			myTile.GetComponent<GameTile>().onMeId = 99999;
					
			tile.GetComponent<GameTile>().onMeId = ball.GetComponent<GamePiece>().id;
			ball.GetComponent<GamePiece>().onMeX = x;
			ball.GetComponent<GamePiece>().onMeZ = z;
			
			float travelTime = Vector3.Distance(ball.transform.position, targetPosition)/rate;
			//iTween.MoveTo(ball, iTween.Hash("position", tile.transform.position, "islocal", true, "time", 1,"oncomplete","Reset"));
			//iTween.MoveBy(ball,iTween.Hash("x",targetPosition.x-ball.transform.position.x,"easetype","easeinoutsine","time",travelTime));
			//iTween.MoveBy(ball,iTween.Hash("z",targetPosition.z-ball.transform.position.z,"time",travelTime,"delay",0,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
			
			iTween.MoveBy(ball,iTween.Hash("x",targetPosition.x-ball.transform.position.x,"easetype","easeinoutsine","time",travelTime));
			iTween.MoveBy(ball,iTween.Hash("z",targetPosition.z-ball.transform.position.z,"time",travelTime,"delay",travelTime,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
		}else{
			hintText="Personagem nao chega no destino.";
			Debug.Log("Nao posso ir tao longe =(");	
			ball = null;
			tile = null;
			myTile = null;
			ballSet=true;
		}
	}*/
	
	
	
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
	
	private GameObject creatingTempObject;
	public void ShuffleParts(){
		//int lastZ=-5;
		//int lastX=-5;
		//int last_direction=-1;
		Vector3 ballPosition;
		int currentPlayerTime = 1;
		int createdObjects = 1;
		
		for (int i = 0; i < 5; i++) {
			for (int j = 1; j < 10; j++) {

				if(createdObjects < numberPlayersPieces){

					GameObject ball = null;
					GameObject tile = gameTiles[i,j];
					ballPosition = tile.transform.position;
					tile.GetComponent<GameTile>().onMeId =createdObjects; 
					ballPosition.y = .15f;
						
					creatingTempObject = getGamePieceByPosition(createdObjects,1);
					ball = (GameObject)Instantiate(creatingTempObject, ballPosition,Quaternion.identity);
					
					ball.GetComponent<GamePiece>().id = createdObjects;
					ball.GetComponent<GamePiece>().onMeX = i;
					ball.GetComponent<GamePiece>().onMeZ = j;
					
					
					ball.GetComponent<GamePiece>().playerBelong = currentPlayerTime;
					ball.name= "Peca "+i.ToString() + " - "+ j.ToString();
					ball.SendMessage("SetGameboard",this);
					gamePieces[createdObjects] = ball;
					createdObjects++;
				}else{
					currentPlayerTime = 2;
					//after create all this pieces, is time to create the ground
					createGround(i,j);
				
				}
				
				
				
			}
		}	
		
		createdGrounds = 0;
		for (int i = 9; i > 5; i--) {
			for (int j = 9; j > 0; j--) {

				if(createdObjects < (numberPlayersPieces*2) && currentPlayerTime == 2){
										
					GameObject ball = null;
					GameObject tile = gameTiles[i,j];
					ballPosition = tile.transform.position;
					tile.GetComponent<GameTile>().onMeId =createdObjects; 
					ballPosition.y = .15f;
					
					creatingTempObject = getGamePieceByPosition(createdObjects-numberPlayersPieces,2);
					ball = (GameObject)Instantiate(creatingTempObject, ballPosition,Quaternion.identity);
					//ball = (GameObject)Instantiate((GameObject)Resources.Load("GameBall"), ballPosition,Quaternion.identity);
					
					ball.GetComponent<GamePiece>().id = createdObjects;
					ball.GetComponent<GamePiece>().onMeX = i;
					ball.GetComponent<GamePiece>().onMeZ = j;
					//ball.GetComponent<GamePiece>().myType = 1;
					
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
	
	private GameObject getGamePieceByPosition(int pos, int player){
		if(player==2)
			Debug.Log ("posicao: "+pos.ToString());
		
		if(pos >= 0 && pos < 2){
			return (GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/Plebe");
		}else if(pos >=2 && pos < 4){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/Lancer");
		}else if(pos >=4 && pos < 6){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/LightHorse");
		}else if(pos >=6 && pos < 7){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/HeavyHorse");	
		}else if(pos >=7 && pos < 8){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/Elephant");	
		}else if(pos >=8 && pos < 11){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/CrossBown");	
		}else if(pos >=11 && pos < 12){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/Catapult");
		}else if(pos >=12 && pos < 13){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/Trabuco");
		}else if(pos >=13 && pos < 14){
			return (GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/Dragon"); 
		}else if(pos >=14 && pos < 15){
			return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/King");	
		}
		//em caso de merda...
		return	(GameObject)Resources.Load("@GamePieces/Player"+player.ToString()+"/Plebe");
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
	
	public GameObject getSelectedPiece(int piece){
		return gamePieces[piece];	
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
						
			this.selectedPiece = piece;	
			
			//gamePieces[piece].GetComponent<GamePiece>().isUp = true;
			for(int i=0; i< gamePieces.Length; i++){
				if(gamePieces[i]!=null){
					if(gamePieces[i].GetComponent<GamePiece>().playerBelong == playerNumber && gamePieces[i].GetComponent<GamePiece>().isUp){
						iTween.MoveTo(gamePieces[i], new Vector3(gamePieces[i].transform.position.x,.15f,gamePieces[i].transform.position.z),.2f);	
						gamePieces[i].GetComponent<GamePiece>().isUp = false;
					}
				}
			}
			
			if(PhotonNetwork.connected){
				photonView.RPC("_selectPiece", PhotonTargets.All,piece);
			}else{
				networkView.RPC("_selectPiece",RPCMode.All,piece);	
			}	
		}
		
	}
	
	//-------------------------
	// MOSTRA QUE PECA ESTA SELECIONADA, COLOCANDO ELA UM POUCO PRA CIMA
	// E AS OUTRAS COLOCANDO NO LOCAL DEVIDO.
	[RPC]
	void _selectPiece(int piece){
		
		gamePieces[piece].GetComponent<GamePiece>().isUp = true;
		iTween.MoveTo((GameObject)gamePieces[piece], new Vector3(gamePieces[piece].transform.position.x,.5f,gamePieces[piece].transform.position.z),.2f);
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
		int montainsCrosseds = 0;
		if(myZ == targetZ){
			//so calcular o x!
			if(myX < targetX){
				//	na direita
				if((myX + movesCanPerform) >= targetX){
					canAtack = true;
					while(myX <= targetX){
						if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
							montainsCrosseds++;
							canAtack = false;	
						}
						myX++;
					}
				}
			}else{
				//na esquerda
				if((myX - movesCanPerform) <= targetX){
					canAtack = true;
					while(myX >= targetX){
						if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
							montainsCrosseds++;
							canAtack = false;	
						}
						myX--;
					}
				}
			}
		}else if(myX == targetX){
			//so calcular o Z
			if(myZ < targetZ){
				//	a cima
				if((myZ + movesCanPerform) >= targetZ){
					canAtack = true;
					while(myZ <= targetZ){
						if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
							montainsCrosseds++;
							canAtack = false;	
						}
						myZ++;
					}
				}
			}else{
				//a baixo
				if((myZ - movesCanPerform) <= targetZ){
					canAtack = true;
					while(myZ >= targetZ){
						if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
							montainsCrosseds++;
							canAtack = false;	
						}
						myZ--;
					}
				}
			}
		}
		
		//poder de atravessar montanhas
		if(montainsCrosseds>0){
			if(me.GetComponent<GamePiece>().montainsCanCross >= montainsCrosseds)
				canAtack = true;
		}
		
	 	GameObject targetPiece = (GameObject) gamePieces[targetTile.GetComponent<GameTile>().onMeId];
		if(me.GetComponent<GamePiece>().powerLevel < targetPiece.GetComponent<GamePiece>().powerLevel){
			hintText="Voce e muito fraco para isso.";
			canAtack=false;
		}
		
		if(me.GetComponent<GamePiece>().myType.Equals("dragon")){
			if(targetTile.GetComponent<GameTile>().getPieceType().Equals("forest")){
				hintText="Dragoes nao podem atacar entre a floresta.";
				canAtack = false;
			}
		}
		
		if(canAtack){
			if(PhotonNetwork.connected){
				photonView.RPC("_atackRpc", PhotonTargets.All,target.GetComponent<GamePiece>().id);
			}else{
				networkView.RPC("_atackRpc",RPCMode.All,target.GetComponent<GamePiece>().id);	
			}
			SetTarget(getTileByPiece(target));
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
	}
	
	public bool canPerformMove(GameObject character,GameObject myTile, GameObject targetTile, int movesCanPerform){
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
					if(character.GetComponent<GamePiece>().myType!="dragon"){
						//verificar montanhas proximas
						while(myX <= targetX){
							if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
								return false;	
							}
							myX++;
						}	
					}
					return true;
				}
			}else{
				//na esquerda
				if((myX - movesCanPerform) <= targetX){
					if(character.GetComponent<GamePiece>().myType!="dragon"){
						//verificar montanhas proximas
						while(myX >= targetX){
							if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
								return false;	
							}
							myX--;
						}	
					}
					return true;
				}
			}
		}else{
			//so calcular o Z
			if(myZ < targetZ){
				//	a cima
				if((myZ + movesCanPerform) >= targetZ){
					if(character.GetComponent<GamePiece>().myType!="dragon"){
						//verificar montanhas proximas
						while(myZ <= targetZ){
							if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
								return false;	
							}
							myZ++;
						}	
					}
					return true;
				}
			}else{
				//a baixo
				if((myZ - movesCanPerform) <= targetZ){
					if(character.GetComponent<GamePiece>().myType!="dragon"){
						//verificar montanhas proximas
						while(myZ >= targetZ){
							if(gameTiles[myX,myZ].GetComponent<GameTile>().getPieceType().Equals("montain")){
								return false;	
							}
							myZ--;
						}	
					}
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
		//Debug.Log("slecionei");
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
		Debug.Log(toX.ToString()+" () "+toZ.ToString());
		if(groundOfSelected.GetComponent<GameTile>().hasGround())
			return;
		
		if(PhotonNetwork.connected){
			photonView.RPC("_rpcChangeGrounds", PhotonTargets.All,fromX, fromZ, toX, toZ);
		}else{
			networkView.RPC("_rpcChangeGrounds",RPCMode.All,fromX, fromZ, toX, toZ);	
		}
		
	}
	
	public void unselectPiece(){
		/*
		if(PhotonNetwork.connected){
			photonView.RPC("_selectPiece", PhotonTargets.All,0);
		}else{
			networkView.RPC("_selectPiece",RPCMode.All,0);	
		}*/	
	}
	
	[RPC]
	public void _rpcChangeGrounds(int fromX, int fromZ,int toX,int toZ){
		GameObject fromTile = gameTiles[fromX, fromZ];
		GameObject toTile	= gameTiles[toX, toZ];
		
		toTile.GetComponent<GameTile>().migrateTypes(fromTile);
	}
	
}

