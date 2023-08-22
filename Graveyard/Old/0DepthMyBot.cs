using ChessChallenge.API;
using System;
using System.Data;
using System.Linq;

public class MyBot : IChessBot
{

    public Move Think(Board board, Timer timer)
    {
        Random rnd = new();
        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[rnd.Next(moves.Length)];
        int[] moveScores = new int[moves.Length];

        for (int depth = 0; timer.MillisecondsElapsedThisTurn < 500; depth++)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                board.MakeMove(moves[i]);
                moveScores[i] = Eval(board);
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
        
    }
}