using UnityEngine;
using System.Collections;

public class Board : Photon.MonoBehaviour
{	
	GameObject currentTarget=null;
	public GameObject ball;
	public int rate = 10;
	public bool ballSet = true;
	public GameObject[,] gameTiles = new GameObject[10,10];
	public GameObject[] gamePieces = new GameObject[11];
	public GameManager gameManager;
	
	void Start ()
	{	
		CreateGameBoard(10,10);
		
	}
	
	public void SetTarget(GameObject target){
		if(currentTarget != null && target!=currentTarget){
			currentTarget.SendMessage("Deactivate");
		}
		currentTarget=target;
		ballSet=false;
		Vector3 position = target.transform.position;
		
		photonView.RPC("movePiece", PhotonTargets.All,position);
	}
	
	[RPC]
	public void movePiece(Vector3 targetPosition){
		Debug.Log(targetPosition.x);
		float travelTime = Vector3.Distance(targetPosition, targetPosition)/rate;
		iTween.MoveBy(ball,iTween.Hash("x",targetPosition.x-targetPosition.x,"easetype","easeinoutsine","time",travelTime));
		iTween.MoveBy(ball,iTween.Hash("z",targetPosition.z-targetPosition.z,"time",travelTime,"delay",travelTime,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
		
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
		ball = (GameObject) PhotonNetwork.Instantiate("GameBall",new Vector3(0,.5f,0),Quaternion.identity,0);
		for(int i=0; i<11; i++){
			
			
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
}

