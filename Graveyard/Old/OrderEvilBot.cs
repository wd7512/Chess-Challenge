using ChessChallenge.API;
using System;
using System.Linq;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        public Move Think(Board board, Timer timer)
        {
            ///Random rnd = new();
            Move[] moves = board.GetLegalMoves();
            ///Move bestMove = moves[rnd.Next(moves.Length)];
            int[] moveScores = new int[moves.Length];

            int[] orderScores = new int[moves.Length];
            int turnTime = Math.Max(100,(65000-Math.Abs(2*timer.MillisecondsRemaining-75000))/30);

            int index = 0;
            Move[] newMoves = new Move[moves.Length];

            for (int i = 0; i < moves.Length; i++)
            {
                board.MakeMove(moves[i]);
                orderScores[i] = Eval(board);
                if (board.IsWhiteToMove) { orderScores[i] = -orderScores[i]; }
                if (board.IsInCheck())
                {
                    orderScores[i] = orderScores[i] + 1000;
                }
                
                board.UndoMove(moves[i]);
                if (moves[i].IsCapture) { orderScores[i] = orderScores[i] + 500; }
            }

            for (int i = 0; i < moves.Length; i++)
            {
                int maxIndex = orderScores.ToList().IndexOf(orderScores.Max());
                newMoves[i] = moves[maxIndex];
                orderScores[maxIndex] = int.MinValue;
            }


            int depth = 0;

            for (depth = 1; timer.MillisecondsElapsedThisTurn < turnTime; depth++)
            {
                ///Console.WriteLine(depth);
                for (int i = 0; (i < moves.Length & timer.MillisecondsElapsedThisTurn < turnTime); i++)
                {
                    board.MakeMove(newMoves[i]);
                    if (board.IsInCheckmate()) { return newMoves[i]; }


                    moveScores[i] = minimax(board, depth, board.IsWhiteToMove, int.MinValue, int.MaxValue);
                    board.UndoMove(newMoves[i]);
                }
            }

            if (board.IsWhiteToMove)
            {
                Console.WriteLine("EvilB: " + moveScores.Max()*100 + " " + depth);
                return newMoves[moveScores.ToList().IndexOf(moveScores.Max())];

            }
            else
            {
                Console.WriteLine("EvilB: " + moveScores.Min()*100 + " "+depth);
                return newMoves[moveScores.ToList().IndexOf(moveScores.Min())];
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
            if (board.IsInCheckmate())
            {
                if (isMax) { return int.MinValue; }
                else { return int.MaxValue; }
            }

            if (board.IsDraw()) { return 0; }

            if (depth == 0) { return Eval(board); }

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
    }
}