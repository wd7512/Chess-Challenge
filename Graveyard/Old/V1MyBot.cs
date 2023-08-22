using ChessChallenge.API;
using System;
using System.Data;
using System.Linq;

public class MyBot : IChessBot
{

    public Move Think(Board board, Timer timer)
    {
        ///Random rnd = new();
        Move[] moves = board.GetLegalMoves();
        ///Move bestMove = moves[rnd.Next(moves.Length)];
        int[] moveScores = new int[moves.Length];
        int turnTime = 1000;

        for (int depth = 0; timer.MillisecondsElapsedThisTurn < turnTime; depth++)
        {
            Console.WriteLine(depth);
            for (int i = 0; (i < moves.Length & timer.MillisecondsElapsedThisTurn < turnTime); i++)
            {
                board.MakeMove(moves[i]);
                if (board.IsInCheckmate()) { return moves[i]; }


                moveScores[i] = minimax(board, depth, board.IsWhiteToMove, int.MinValue, int.MaxValue);
                board.UndoMove(moves[i]);
            }
        }

        if (board.IsWhiteToMove)
        {
            return moves[moveScores.ToList().IndexOf(moveScores.Max())];
        }
        else
        {
            return moves[moveScores.ToList().IndexOf(moveScores.Min())];
        }
    }

    int Eval(Board board)
    {
        int num = 0;
        PieceList[] allPieces = board.GetAllPieceLists();

        foreach (PieceList pieceList in allPieces)
        {
            if (pieceList.TypeOfPieceInList == PieceType.Pawn)
            {
                if (pieceList.IsWhitePieceList == true) { num = num + 1 * pieceList.Count; }
                else { num = num - 1 * pieceList.Count; }
            }
            else if (pieceList.TypeOfPieceInList == PieceType.Knight | pieceList.TypeOfPieceInList == PieceType.Bishop)
            {
                if (pieceList.IsWhitePieceList == true) { num = num + 3 * pieceList.Count; }
                else { num = num - 3 * pieceList.Count; }
            }
            else if (pieceList.TypeOfPieceInList == PieceType.Rook)
            {
                if (pieceList.IsWhitePieceList == true) { num = num + 5 * pieceList.Count; }
                else { num = num - 5 * pieceList.Count; }
            }
            else if (pieceList.TypeOfPieceInList == PieceType.Queen)
            {
                if (pieceList.IsWhitePieceList == true) { num = num + 9 * pieceList.Count; }
                else { num = num - 9 * pieceList.Count; }
            }
        }
        return num;
    }

    int minimax(Board board, int depth, bool isMax, int alpha, int beta)
    {

        if (depth == 0) { return Eval(board); }
        if (isMax)
        {
            int bestVal = int.MinValue;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                int value = minimax(board, depth - 1, false, alpha, beta);
                board.UndoMove(move);
                bestVal = Math.Max(bestVal, value);
                alpha = Math.Max(alpha, bestVal);
                if (beta <= alpha) { break; }
            }
            return bestVal;
        }
        else
        {
            int bestVal = int.MaxValue;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                int value = minimax(board, depth - 1, true, alpha, beta);
                board.UndoMove(move);
                bestVal = Math.Min(bestVal, value);
                alpha = Math.Min(alpha, bestVal);
                if (beta <= alpha) { break; }
            }
            return bestVal;
        }
    }
    
}