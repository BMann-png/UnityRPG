using System.Collections.Generic;

public static class RPGSquares
{
	public class Movement
	{
		public readonly Square Start;
		public readonly Square End;

		/// <summary>Creates a new Movement.</summary>
		/// <param name="piecePosition">Position of piece being moved.</param>
		/// <param name="end">Square which the piece will land on.</param>
		public Movement(Square piecePosition, Square end)
		{
			Start = piecePosition;
			End = end;
		}

		/// <summary>Copy constructor.</summary>
		internal Movement(Movement move)
		{
			Start = move.Start;
			End = move.End;
		}

		protected bool Equals(Movement other) => Start == other.Start && End == other.End;

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return GetType() == obj.GetType() && Equals((Movement)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Start.GetHashCode() * 397) ^ End.GetHashCode();
			}
		}

		public override string ToString() => $"{Start}->{End}";
	}
	/// <summary>Representation of a square on a chessboard.</summary>
	public readonly struct Square
	{
		public static readonly Square Invalid = new Square(-1, -1);
		public readonly int File;
		public readonly int Rank;

		/// <summary>Creates a new Square instance.</summary>
		/// <param name="file">Column of the square.</param>
		/// <param name="rank">Row of the square.</param>
		public Square(int file, int rank)
		{
			File = file;
			Rank = rank;
		}

		public Square(string squareString)
		{
			this = string.IsNullOrEmpty(squareString)
				? Invalid
				: RPGSquares.StringToSquare(squareString);
		}

		internal Square(Square startPosition, int fileOffset, int rankOffset)
		{
			File = startPosition.File + fileOffset;
			Rank = startPosition.Rank + rankOffset;
		}

		internal readonly bool IsValid()
		{
			return File is >= 1 and <= 8
				   && Rank is >= 1 and <= 8;
		}

		public static bool operator ==(Square lhs, Square rhs) => lhs.File == rhs.File && lhs.Rank == rhs.Rank;
		public static bool operator !=(Square lhs, Square rhs) => !(lhs == rhs);
		public static Square operator +(Square lhs, Square rhs) => new Square(lhs.File + rhs.File, lhs.Rank + rhs.Rank);

		public bool Equals(Square other) => File == other.File && Rank == other.Rank;

		public bool Equals(int file, int rank) => File == file && Rank == rank;

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;

			return obj is Square other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (File * 397) ^ Rank;
			}
		}

		public override string ToString() => RPGSquares.SquareToString(this);
	}

	public static readonly Dictionary<string, int> FileCharToIntMap = new()
	{
		{ "a", 1 },
		{ "b", 2 },
		{ "c", 3 },
		{ "d", 4 },
		{ "e", 5 },
		{ "f", 6 },
		{ "g", 7 },
		{ "h", 8 }
	};

	public static readonly Dictionary<int, string> FileIntToCharMap = new()
	{
		{ 1, "a" },
		{ 2, "b" },
		{ 3, "c" },
		{ 4, "d" },
		{ 5, "e" },
		{ 6, "f" },
		{ 7, "g" },
		{ 8, "h" }
	};

	public static readonly Square[] KnightOffsets = {
			new(-2, -1),
			new(-2, 1),
			new(2, -1),
			new(2, 1),
			new(-1, -2),
			new(-1, 2),
			new(1, -2),
			new(1, 2),
		};

	public static readonly Square[] SurroundingOffsets = {
			new(-1, 0),
			new(1, 0),
			new(0, -1),
			new(0, 1),
			new(-1, 1),
			new(-1, -1),
			new(1, -1),
			new(1, 1),
		};

	public static readonly Square[] DiagonalOffsets = {
			new(-1, 1),
			new(-1, -1),
			new(1, -1),
			new(1, 1)
		};

	public static readonly Square[] CardinalOffsets = {
			new(-1, 0),
			new(1, 0),
			new(0, -1),
			new(0, 1),
		};



	public static string SquareToString(Square square) => SquareToString(square.File, square.Rank);
	public static string SquareToString(int file, int rank)
	{
		if (FileIntToCharMap.TryGetValue(file, out string fileChar))
		{
			return $"{fileChar}{rank}";
		}

		return "Invalid";
	}

	public static Square StringToSquare(string squareText)
	{
		return new Square(
			FileCharToIntMap[squareText[0].ToString()],
			int.Parse(squareText[1].ToString())
		);
	}
}