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

	public RPGVisualPiece[,] boardMatrix
    {
		get
        {
			boardMatrix = new RPGVisualPiece[8, 8];

			for (int file = 1; file <= 8; file++)
			{
				for (int rank = 1; rank <= 8; rank++)
				{
					boardMatrix[rank - 1, file - 1] = null;
				}
			}
			return boardMatrix;
		}
		set
        {

        }
    }

	public Board CurrentBoard
	{
		get
		{
			game.BoardTimeline.TryGetCurrent(out Board currentBoard);
			return currentBoard;
		}
	}


	public Side SideToMove
	{
		get
		{
			game.ConditionsTimeline.TryGetCurrent(out GameConditions currentConditions);
			return currentConditions.SideToMove;
		}
	}

	public List<(RPGSquares.Square, RPGVisualPiece)> CurrentPieces
	{
		get
		{
			currentPiecesBacking.Clear();
			for (int file = 1; file <= 8; file++)
			{
				for (int rank = 1; rank <= 8; rank++)
				{
					RPGVisualPiece piece = boardMatrix[file, rank];
					if (piece != null) currentPiecesBacking.Add((new RPGSquares.Square(file, rank), piece));
				}
			}

			return currentPiecesBacking;
		}
	}


	private readonly List<(RPGSquares.Square, RPGVisualPiece)> currentPiecesBacking = new List<(RPGSquares.Square, RPGVisualPiece)>();

	private Game game;
	private Dictionary<GameSerializationType, IGameSerializer> serializersByType;
	private GameSerializationType selectedSerializationType = GameSerializationType.FEN;

	public void Start()
	{
		RPGVisualPiece.VisualPieceMoved += OnPieceMoved;

		StartNewGame();
	}

#if AI_TEST
	public async void StartNewGame(bool isWhiteAI = true, bool isBlackAI = true) {
#else
	public async void StartNewGame(bool isWhiteAI = false, bool isBlackAI = false)
	{
#endif
		game = new Game();

		NewGameStartedEvent?.Invoke();
	}

	public void LoadGame(string serializedGame)
	{
		game = serializersByType[selectedSerializationType].Deserialize(serializedGame);
		NewGameStartedEvent?.Invoke();
	}

	private bool TryExecuteMove(RPGSquares.Movement move)
	{

		RPGVisualPiece pieceToMove = boardMatrix[move.Start.Rank - 1, move.Start.File - 1];
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

		//if (!game.TryGetLegalMove(movedPieceInitialSquare, endSquare, out RPGSquares.Movement move))
		{
			movedPieceTransform.position = movedPieceTransform.parent.position;

			return;
		}

		if (TryExecuteMove(move))
		{
			if (true) { RPGBoardManager.Instance.TryDestroyVisualPiece(move.End); }

			movedPieceTransform.parent = closestBoardSquareTransform;
			movedPieceTransform.position = closestBoardSquareTransform.position;
		}

		bool gameIsOver = game.HalfMoveTimeline.TryGetCurrent(out HalfMove lastHalfMove)
						  && lastHalfMove.CausedStalemate || lastHalfMove.CausedCheckmate;
		
	}
}
