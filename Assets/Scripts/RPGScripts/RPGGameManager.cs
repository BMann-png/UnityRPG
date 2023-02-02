using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityChess;
using UnityChess.Engine;
using UnityEngine;
using TMPro;

public class RPGGameManager : MonoBehaviourSingleton<RPGGameManager>
{
	public static event Action NewGameStartedEvent;
	public static event Action GameEndedEvent;
	public static event Action MoveExecutedEvent;

	[SerializeField] public GameObject wui;
	[SerializeField] public TMP_Text wh;
	[SerializeField] public GameObject bui;
	[SerializeField] public TMP_Text bh;

	private GameObject actionObject;
	public GameObject targetObject;

	bool pieceMovementActive = true;

	public RPGPiece[,] boardMatrix = new RPGPiece[8, 8]
	{
		{ null, null, null, new RPGPiece("Queen", Side.Black), null, null, null, null },
		{ null, null, null, null,							   null, null, null, null },
		{ null, null, null, null,							   null, null, null, null },
		{ null, null, null, null,							   null, null, null, null },
		{ null, null, null, null,							   null, null, null, null },
		{ null, null, null, null,							   null, null, null, null },
		{ null, null, null, null,							   null, null, null, null },
		{ null, null, null, null, new RPGPiece("King", Side.White),  null, null, null }
	};

	public Side SideToMove = Side.White;

	public List<(RPGSquares.Square, RPGPiece)> CurrentPieces
	{
		get
		{
			currentPiecesBacking.Clear();
			for (int file = 1; file <= 8; file++)
			{
				for (int rank = 1; rank <= 8; rank++)
				{
					RPGPiece piece = boardMatrix[file - 1, rank - 1];
					if (piece != null) currentPiecesBacking.Add((new RPGSquares.Square(file, rank), piece));
				}
			}

			return currentPiecesBacking;
		}
	}


	private readonly List<(RPGSquares.Square, RPGPiece)> currentPiecesBacking = new List<(RPGSquares.Square, RPGPiece)>();

	public void Start()
	{
		RPGVisualPiece.VisualPieceMoved += OnPieceMoved;

		StartNewGame();
	}

    public void LateUpdate()
    {
		if (!pieceMovementActive && Input.GetMouseButtonDown(1))
        {
			RPGBoardManager.Instance.SetActiveAllPieces(true);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				targetObject = hit.collider.gameObject;
			}

			Debug.Log(targetObject.GetComponent<RPGVisualPiece>().CurrentSquare);
        }
	}


    public async void StartNewGame(bool isWhiteAI = false, bool isBlackAI = false)
	{
		NewGameStartedEvent?.Invoke();
	}

	private bool TryExecuteMove(RPGSquares.Movement move)
	{

		RPGPiece pieceToMove = boardMatrix[move.Start.Rank - 1, move.Start.File - 1];
		boardMatrix[move.Start.Rank - 1, move.Start.File - 1] = null;
		boardMatrix[move.End.Rank - 1, move.End.File - 1] = pieceToMove;

		//game ended
		if (false)
		{
			RPGBoardManager.Instance.SetActiveAllPieces(false);
			GameEndedEvent?.Invoke();
		}
		else
		{
			
		}

		MoveExecutedEvent?.Invoke();

		return true;
	}

	private async void OnPieceMoved(RPGSquares.Square movedPieceInitialSquare, Transform movedPieceTransform, Transform closestBoardSquareTransform, Piece promotionPiece = null)
	{
		RPGSquares.Square endSquare = new RPGSquares.Square(closestBoardSquareTransform.name);

		RPGSquares.Movement move = new RPGSquares.Movement(movedPieceInitialSquare, endSquare);

		if (!RPGBoardManager.Instance.ValidatePieceMovement(move))
		{
			movedPieceTransform.position = movedPieceTransform.parent.position;

			return;
		}

		if (TryExecuteMove(move))
		{
			if (true) { RPGBoardManager.Instance.TryDestroyVisualPiece(move.End); }

			movedPieceTransform.parent = closestBoardSquareTransform;
			movedPieceTransform.position = closestBoardSquareTransform.position;

            if (SideToMove == Side.White)
            {
                SideToMove = Side.Black;
				wui.SetActive(true);
            }
			else
            {
				SideToMove = Side.White;
				bui.SetActive(true);
            }
			actionObject = RPGBoardManager.Instance.GetPieceGOAtPosition(move.End);
			Debug.Log(actionObject.GetComponent<RPGVisualPiece>().CurrentSquare);

			pieceMovementActive = false;
			RPGBoardManager.Instance.SetActiveAllPieces(false);
		}
    }

	public void OnActionClick(int i)
    {
		if (actionObject.TryGetComponent<RPGActionDefinition>(out RPGActionDefinition ra))
        {
			if(ra.InteractWithPiece(i, targetObject)) 
				RPGBoardManager.Instance.TryDestroyVisualPiece(targetObject.GetComponent<RPGVisualPiece>().CurrentSquare);
			targetObject = null;
			actionObject = null;


			wui.SetActive(false);
			bui.SetActive(false);

			pieceMovementActive = true;
			RPGBoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(SideToMove);
		}
    }

	public void OnNewGameClick()
    {
		StartNewGame();
    }
}
