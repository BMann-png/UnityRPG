using System;
using System.Collections.Generic;
using UnityEngine;
using UnityChess;

public class RPGBoardManager : MonoBehaviourSingleton<RPGBoardManager>
{
	private readonly GameObject[] allSquaresGO = new GameObject[64];
	private Dictionary<RPGSquares.Square, GameObject> positionMap;
	private const float BoardPlaneSideLength = 14f; // measured from corner square center to corner square center, on same side.
	private const float BoardPlaneSideHalfLength = BoardPlaneSideLength * 0.5f;
	private const float BoardHeight = 1.6f;
	private readonly System.Random rng = new System.Random();

	private void Awake()
	{
		RPGGameManager.NewGameStartedEvent += OnNewGameStarted;

		positionMap = new Dictionary<RPGSquares.Square, GameObject>(64);
		Transform boardTransform = transform;
		Vector3 boardPosition = boardTransform.position;

		for (int file = 1; file <= 8; file++)
		{
			for (int rank = 1; rank <= 8; rank++)
			{
				GameObject squareGO = new GameObject(RPGSquares.SquareToString(file, rank))
				{
					transform = {
						position = new Vector3(boardPosition.x + FileOrRankToSidePosition(file), boardPosition.y + BoardHeight, boardPosition.z + FileOrRankToSidePosition(rank)),
						parent = boardTransform
					},
					tag = "Square"
				};

				positionMap.Add(new RPGSquares.Square(file, rank), squareGO);
				allSquaresGO[(file - 1) * 8 + (rank - 1)] = squareGO;
			}
		}
	}

	private void OnNewGameStarted()
	{
		ClearBoard();

		foreach ((RPGSquares.Square square, RPGPiece piece) in RPGGameManager.Instance.CurrentPieces)
		{
			CreateAndPlacePieceGO(piece, square);
		}

		EnsureOnlyPiecesOfSideAreEnabled(RPGGameManager.Instance.SideToMove);
	}

	

	public void CreateAndPlacePieceGO(RPGPiece piece, RPGSquares.Square position)
	{
		string modelName = $"{piece.Owner} {piece.name}";
		GameObject pieceGO = Instantiate(
			Resources.Load("PieceSets/RPGPieces/" + modelName) as GameObject,
			positionMap[position].transform
		);

		/*if (!(piece is Knight) && !(piece is King)) {
			pieceGO.transform.Rotate(0f, (float) rng.NextDouble() * 360f, 0f);
		}*/
	}

	public void GetSquareGOsWithinRadius(List<GameObject> squareGOs, Vector3 positionWS, float radius)
	{
		float radiusSqr = radius * radius;
		foreach (GameObject squareGO in allSquaresGO)
		{
			if ((squareGO.transform.position - positionWS).sqrMagnitude < radiusSqr)
				squareGOs.Add(squareGO);
		}
	}

	public void SetActiveAllPieces(bool active)
	{
		RPGVisualPiece[] visualPiece = GetComponentsInChildren<RPGVisualPiece>(true);
		foreach (RPGVisualPiece pieceBehaviour in visualPiece) pieceBehaviour.enabled = active;
	}

	public void EnsureOnlyPiecesOfSideAreEnabled(Side side)
	{
		RPGVisualPiece[] visualPiece = GetComponentsInChildren<RPGVisualPiece>(true);
		foreach (RPGVisualPiece pieceBehaviour in visualPiece)
		{
			RPGPiece piece = RPGGameManager.Instance.boardMatrix[pieceBehaviour.CurrentSquare.Rank - 1, pieceBehaviour.CurrentSquare.File - 1];

			pieceBehaviour.enabled = pieceBehaviour.PieceColor == side;
									 
		}
	}

	public void TryDestroyVisualPiece(RPGSquares.Square position)
	{
		RPGVisualPiece visualPiece = positionMap[position].GetComponentInChildren<RPGVisualPiece>();
		if (visualPiece != null) DestroyImmediate(visualPiece.gameObject);
	}

	public GameObject GetPieceGOAtPosition(RPGSquares.Square position)
	{
		GameObject square = GetSquareGOByPosition(position);
		return square.transform.childCount == 0 ? null : square.transform.GetChild(0).gameObject;
	}

	private static float FileOrRankToSidePosition(int index)
	{
		float t = (index - 1) / 7f;
		return Mathf.Lerp(-BoardPlaneSideHalfLength, BoardPlaneSideHalfLength, t);
	}

	private void ClearBoard()
	{
		RPGVisualPiece[] visualPiece = GetComponentsInChildren<RPGVisualPiece>(true);

		foreach (RPGVisualPiece pieceBehaviour in visualPiece)
		{
			DestroyImmediate(pieceBehaviour.gameObject);
		}
	}

	public GameObject GetSquareGOByPosition(RPGSquares.Square position) => Array.Find(allSquaresGO, go => go.name == RPGSquares.SquareToString(position));

	public bool ValidatePieceMovement(RPGSquares.Movement move)
	{
		RPGVisualPiece visualPiece = positionMap[move.Start].GetComponentInChildren<RPGVisualPiece>();
		if (visualPiece != null)
        {
			int xOff = Math.Abs(move.Start.File - move.End.File);
			int yOff = Math.Abs(move.Start.Rank - move.End.Rank);

			return visualPiece.PieceSpeed >= xOff + yOff;
        }

		return false;
	}
}
