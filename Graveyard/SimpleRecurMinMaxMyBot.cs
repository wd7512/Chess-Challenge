using ChessChallenge.API;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

public class MyBot : IChessBot
{
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
    public int minMax(Board board,int turn,int depth)
    {
        if (depth == 0)
        {
            return Eval(board);
        }

        if (board.IsInCheckmate())
        {
            return -10000 * turn;
        }

        if (board.IsDraw())
        {
            return 0;
        }


        Move[] moves = board.GetLegalMoves();
        int[] moveScores = new int[moves.Length];

        for (int i = 0; i < moves.Length; i++)
        {
            board.MakeMove(moves[i]);
            moveScores[i] = minMax(board,turn*-1,depth-1);
            board.UndoMove(moves[i]);
        }

        int bestScore = int.MinValue * turn;
        for (int i = 0; i < moves.Length; i++)
        {
            if (bestScore * turn < moveScores[i] * turn)
            {
                bestScore = moveScores[i];
            }
        }

        return bestScore;
    }

    public Move Think(Board board, ChessChallenge.API.Timer timer)
    {

        bool test = board.IsDraw();

        int turn = -1;
        if (board.IsWhiteToMove) { turn = 1; }

        Move[] moves = board.GetLegalMoves(); 
        int[] moveScores = new int[moves.Length];

        for (int i=0; i < moves.Length; i++)
        {
            board.MakeMove(moves[i]);
            moveScores[i] = minMax(board,turn*-1,2);
            board.UndoMove(moves[i]);
            Console.WriteLine(moveScores[i]);
        }

        Move bestMove = moves[0];
        int bestScore = int.MinValue * turn;
        for (int i = 0; i < moves.Length; i++)
        {
            if (bestScore*turn < moveScores[i]*turn)
            {
                bestScore = moveScores[i];
                bestMove = moves[i];
            }
            else if (bestScore == moveScores[i])
            {
                Random rng = new();
                if (rng.Next(2) == 0)
                {
                    bestMove = moves[i];
                }
            }
        }
        ///Console.WriteLine(bestScore);
        return bestMove;
    }
}