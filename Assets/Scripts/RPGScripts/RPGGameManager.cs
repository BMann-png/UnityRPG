using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityChess;
using UnityChess.Engine;
using UnityEngine;

public class RPGGameManager : MonoBehaviourSingleton<RPGGameManager>
{
	public static event Action NewGameStartedEvent;
	public static event Action GameEndedEvent;
	public static event Action MoveExecutedEvent;

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
			RPGBoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(SideToMove);
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
            }
			else
            {
				SideToMove = Side.White;
            }
        }

    }

	public void OnNewGameClick()
    {
		StartNewGame();
    }
}
