using ChessChallenge.API;
using System.Linq;
using System;
using System.Net.NetworkInformation;

public class MyBot : IChessBot
{
    int[] pieceValues = { 100, 300, 300, 500, 900 }; 

    class MoveScore
    {
        public Move move { get; set; }
        public int score { get; set; }
    }
    public Move Think(Board board, Timer timer)
    {
        ///Console.WriteLine("MyBot0: " + Evaluate(board));
        ///Random rnd = new();
        Move[] moves = board.GetLegalMoves();
        ///Move bestMove = moves[rnd.Next(moves.Length)];
        int[] moveScores = new int[moves.Length];
        int[] newScores = new int[moves.Length];
        int turnTime = Math.Max(Math.Min(100,timer.MillisecondsRemaining/3), (65000 - Math.Abs(2 * timer.MillisecondsRemaining - 75000)) / 30);

        int depth = 0;
        for (depth = 0; timer.MillisecondsElapsedThisTurn < turnTime; depth++)
        {
            ///moveScores = newScores;
            ///Console.WriteLine(depth);
            for (int i = 0; (i < moves.Length & timer.MillisecondsElapsedThisTurn < turnTime); i++)
            {
                board.MakeMove(moves[i]);
                if (board.IsInCheckmate()) { return moves[i]; }


                moveScores[i] = minimax(board, depth, board.IsWhiteToMove, int.MinValue, int.MaxValue);
                board.UndoMove(moves[i]);
            }

            ///int[] indexes = SortAndIndex(newScores);

            
        }
        ///Console.WriteLine("MyBot: " + Evaluate(board));
        if (board.IsWhiteToMove)
        {
            Console.WriteLine("MyBot: " + moveScores.Max() + " " + depth);
            return moves[moveScores.ToList().IndexOf(moveScores.Max())];
            
        }
        else
        {
            Console.WriteLine("MyBot: " + moveScores.Min() + " " + depth);
            return moves[moveScores.ToList().IndexOf(moveScores.Min())];
        }
    }

    int Evaluate(Board board)
    {
        int num = 0;

        if (board.IsInCheck())
        {
            if (board.IsWhiteToMove)
            {
                num = num - 1;
            }
            else
            {
                num = num + 1;
            }
        }
        

        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            foreach (Piece piece in pieceList)
            {
                if (piece.IsPawn)
                {
                    if (piece.IsWhite) 
                    { 
                        num = num + pieceValues[0]; 
                        if (piece.Square.Rank == 1) { num = num - 1; }
                    }
                    else 
                    { 
                        num = num - pieceValues[0];

                        if (piece.Square.Rank == 6) { num = num + 1; }
                    }
                }
                else if (piece.IsKnight)
                {
                    if (piece.IsWhite)
                    {
                        num = num + pieceValues[1];
                        if (piece.Square.File == 0 | piece.Square.File == 7) { num = num - 2; }
                        if (piece.Square.File == 1 | piece.Square.File == 6) { num = num - 1; }
                    }
                    else
                    {
                        num = num - pieceValues[1];
                        if (piece.Square.File == 0 | piece.Square.File == 7) { num = num + 2; }
                        if (piece.Square.File == 1 | piece.Square.File == 6) { num = num + 1; }
                    }
                }
                else if (piece.IsBishop)
                {
                    if (piece.IsWhite)
                    {
                        num = num + pieceValues[2];
                        if (pieceList.Count >= 2) { num = num + 1; }
                    }
                    else
                    {
                        num = num - pieceValues[2];
                        if (pieceList.Count >= 2) { num = num - 1; }
                    }
                }
                else if (piece.IsRook)
                {
                    if (piece.IsWhite)
                    {
                        num = num + pieceValues[3];
                        if (pieceList.Count >= 2) { num = num + 1; }
                    }
                    else
                    {
                        num = num - pieceValues[3];
                        if (pieceList.Count >= 2) { num = num - 1; }
                    }
                }
                else if (piece.IsQueen)
                {
                    if (piece.IsWhite) { num = num + pieceValues[4]; }
                    else { num = num - pieceValues[4]; }
                }

            }
        }
        return num;
    }
    int minimax(Board board, int depth, bool isMax, int alpha, int beta)
    {
        if (board.IsInCheckmate())
        {
            if (isMax) { return int.MinValue; }
            else { return int.MaxValue; }
        }

        if (board.IsDraw()) { return 0; }

        if (depth == 0) { return Evaluate(board); }

        if (isMax)
        {
            int maxVal = int.MinValue;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                int value = minimax(board, depth - 1, false, alpha, beta);
                board.UndoMove(move);
                maxVal = Math.Max(maxVal, value);
                alpha = Math.Max(alpha, maxVal);
                if (beta <= alpha) { return alpha; }
            }
            return maxVal;
        }
        else
        {
            int minVal = int.MaxValue;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                int value = minimax(board, depth - 1, true, alpha, beta);
                board.UndoMove(move);
                minVal = Math.Min(minVal, value);
                beta = Math.Min(beta, value);
                if (beta <= alpha) { return beta; }
            }
            return minVal;
        }
    }

    int[] SortAndIndex<T>(T[] rg)
    {
        int i, c = rg.Length;
        var keys = new int[c];
        if (c > 1)
        {
            for (i = 0; i < c; i++)
                keys[i] = i;

            System.Array.Sort(rg, keys /*, ... */);
        }
        return keys;
    }
}