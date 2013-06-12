using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{	
	GameObject currentTarget=null;
	public GameObject ball;	
	public int rate = 10;
	public bool ballSet = true;
	public GameObject[,] gameTiles = new GameObject[10,10];
	public GameObject[] gamePieces = new GameObject[45];
	public GameManager gameManager;
	public int selectedPiece;
	private PhotonView photonView;
	
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
		
		
		currentTarget=target;
		ballSet=false;
		Vector3 position = target.transform.position;
		if(PhotonNetwork.connected){
			photonView.RPC("movePiece", PhotonTargets.All,currentTarget.transform.position);
		}else{
			networkView.RPC("movePiece",RPCMode.All,currentTarget.transform.position);	
		}
	}
	
	[RPC]
	public void movePiece(Vector3 targetPosition){
		ball = getSelectedPiece();
		float travelTime = Vector3.Distance(ball.transform.position, targetPosition)/rate;
		iTween.MoveBy(ball,iTween.Hash("x",targetPosition.x-ball.transform.position.x,"easetype","easeinoutsine","time",travelTime));
		iTween.MoveBy(ball,iTween.Hash("z",targetPosition.z-ball.transform.position.z,"time",travelTime,"delay",travelTime,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
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
				newBlock.SendMessage("SetGameboard",this);
				Color blockColor;
				if((j+i)%2 == 0){
					blockColor=Color.black;
				}else{
					blockColor=Color.white;
				}
				newBlock.renderer.material.color=blockColor;
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
		int lastZ=-5;
		int lastX=-5;
		int last_direction=-1;
		Vector3 ballPosition;
		for(int i=0; i<20; i++){
			GameObject ball = null;
			//linha, altura, colunas
			//5 / 1 = player 2
			//-5 / -1 = Player 1
			//z = colunas
			// x = linhas (-5 a 4)
			if(i<20){
				//p1	
				
				ballPosition = new Vector3(lastX,.5f,lastZ);
				if(last_direction<0){
					lastZ++;
					if(lastZ==-1)
						last_direction = 1;
				}else{
					lastZ--;
					if(lastZ==-5)
						last_direction = -1;
				}
			}else{
				//p2
				
				ballPosition = new Vector3(lastX,.5f,lastZ);
				
			}
			
			ball = (GameObject)Instantiate((GameObject)Resources.Load("GameBall"), ballPosition,Quaternion.identity);
			
			
			ball.name= i.ToString();
			ball.SendMessage("SetGameboard",this);
			gamePieces[i] = ball;
			/*
			GameObject gamePiece = (GameObject)Instantiate(block,new Vector3(i,0,j),Quaternion.identity);
			gamePiece.name="Block: " + i + "," + j;
			newBlock.SendMessage("SetGameboard",this);
			Color blockColor;
			if((j+i)%2 == 0){
				blockColor=Color.black;
			}else{
				blockColor=Color.white;
			}
			newBlock.renderer.material.color=blockColor;
			newBlock.transform.parent=transform;
			
			gameTiles[i,j] = newBlock;
			*/
		}
		
	}
	
	public GameObject getSelectedPiece(){
		return gamePieces[selectedPiece];	
	}
	public bool hasPieceSelected(){
		return selectedPiece!=null;
	}
	public void selectPiece(int piece){
		if(PhotonNetwork.connected){
			photonView.RPC("_selectPiece", PhotonTargets.All,piece);
		}else{
			networkView.RPC("_selectPiece",RPCMode.All,piece);	
		}	
	}
	
	[RPC]
	void _selectPiece(int piece){
		this.selectedPiece = piece;	
	}
}

