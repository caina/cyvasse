using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {
	
	public Board gameBoard;
	//public enum types {piece = 1, tile = 2};
	public int pieceMaxMoves=1;
	public int id;
	public int onMeX;
	public int onMeZ;
	public string myType;
	public int playerBelong;
	public int maxRangeAtack;
	public int powerLevel;
	public bool isUp = false;
	public int montainsCanCross = 0;
	public string realName = "";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown(){
		if(!gameBoard.gameManager.isMyRound())
			return;
		//verificar se a peca eh minha
		Debug.Log(myType);	
		gameBoard.selectPiece(this.gameObject);
	}
	
	void SetGameboard(Board gameBoard){
		this.gameBoard=gameBoard;
	}
	
	void OnMouseOver() {
        gameBoard.showMovementOptions(gameObject);
    }
	void OnMouseExit(){
		gameBoard.hideMovementOptions();
	}
	
	public void setRotation(Transform trans){
		
		this.transform.LookAt(trans);
		transform.rotation = Quaternion.Euler(0,gameObject.transform.rotation.y,gameObject.transform.rotation.z);
	}
}
