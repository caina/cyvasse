using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {
	
	public Board gameBoard;
	public enum types {piece = 1, tile = 2};
	public int pieceMaxMoves=1;
	public int id;
	public int onMeX;
	public int onMeZ;
	public int myType;
	public int playerBelong;
	public int maxRangeAtack;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown(){
		//verificar se a peca eh minha
		
		gameBoard.selectPiece(this.gameObject);
	}
	
	void SetGameboard(Board gameBoard){
		this.gameBoard=gameBoard;
	}
}
