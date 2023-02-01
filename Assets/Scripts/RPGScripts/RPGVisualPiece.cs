using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChess;

public class RPGVisualPiece : MonoBehaviour
{
	public delegate void VisualPieceMovedAction(RPGSquares.Square movedPieceInitialSquare, Transform movedPieceTransform, Transform closestBoardSquareTransform, Piece promotionPiece = null);
	public static event VisualPieceMovedAction VisualPieceMoved;

	public Side PieceColor;
	public int PieceSpeed;
	public RPGSquares.Square CurrentSquare => RPGSquares.StringToSquare(transform.parent.name);

	private const float SquareCollisionRadius = 9f;
	private Camera boardCamera;
	private Vector3 piecePositionSS;
	private SphereCollider pieceBoundingSphere;
	private List<GameObject> potentialLandingSquares;
	private Transform thisTransform;

	private void Start()
	{
		potentialLandingSquares = new List<GameObject>();
		thisTransform = transform;
		boardCamera = Camera.main;
	}

	public void OnMouseDown()
	{
		if (enabled)
		{
			piecePositionSS = boardCamera.WorldToScreenPoint(transform.position);
		}
	}

	private void OnMouseDrag()
	{
		if (enabled)
		{
			Vector3 nextPiecePositionSS = new Vector3(Input.mousePosition.x, Input.mousePosition.y, piecePositionSS.z);
			thisTransform.position = boardCamera.ScreenToWorldPoint(nextPiecePositionSS);
		}
	}

	public void OnMouseUp()
	{
		if (enabled)
		{
			potentialLandingSquares.Clear();
			RPGBoardManager.Instance.GetSquareGOsWithinRadius(potentialLandingSquares, thisTransform.position, SquareCollisionRadius);

			if (potentialLandingSquares.Count == 0)
			{ // piece moved off board
				thisTransform.position = thisTransform.parent.position;
				return;
			}

			// determine closest square out of potential landing squares.
			Transform closestSquareTransform = potentialLandingSquares[0].transform;
			float shortestDistanceFromPieceSquared = (closestSquareTransform.transform.position - thisTransform.position).sqrMagnitude;
			for (int i = 1; i < potentialLandingSquares.Count; i++)
			{
				GameObject potentialLandingSquare = potentialLandingSquares[i];
				float distanceFromPieceSquared = (potentialLandingSquare.transform.position - thisTransform.position).sqrMagnitude;

				if (distanceFromPieceSquared < shortestDistanceFromPieceSquared)
				{
					shortestDistanceFromPieceSquared = distanceFromPieceSquared;
					closestSquareTransform = potentialLandingSquare.transform;
				}
			}

			VisualPieceMoved?.Invoke(CurrentSquare, thisTransform, closestSquareTransform);
		}
	}
}
