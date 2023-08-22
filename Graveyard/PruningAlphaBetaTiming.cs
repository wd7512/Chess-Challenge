using ChessChallenge.API;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

public class MyBot : IChessBot
{
    int Eval(Board board,int turn)
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


    public int minMax(Board board,int turn,int depth,int alpha,int beta)
    {
        if (board.IsInCheckmate())
        {
            return -1000 * turn;
        }
        if (board.IsDraw())
        {
            return 0;
        }
        if (depth == 0)
        {
            return Eval(board,turn);
        }

        Move[] moves = board.GetLegalMoves();
        int[] moveScores = new int[moves.Length];

        for (int i = 0; i < moves.Length; i++)
        {
            board.MakeMove(moves[i]);
            moveScores[i] = minMax(board,turn*-1,depth-1,alpha,beta);
            board.UndoMove(moves[i]);

            if (turn == 1)
            {
                alpha = Math.Max(alpha, moveScores[i]);
                if (moveScores[i] >= beta) { break; }
            }
            else
            {
                beta = Math.Min(beta, moveScores[i]);
                if (moveScores[i] <= alpha) { break; }
            }
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

        int turn = -1;
        if (board.IsWhiteToMove) { turn = 1; }

        Move[] moves = board.GetLegalMoves(); 
        int[] moveScores = new int[moves.Length];

        int turnTime = Math.Max(Math.Min(10, timer.MillisecondsRemaining / 3), (65000 - Math.Abs(2 * timer.MillisecondsRemaining - 75000)) / 30);

        int startDepth = 0;
        if (turnTime > 100) { startDepth = 2; }
        else if (turnTime > 250) { startDepth = 3; }
        else if (turnTime>1500) { startDepth = 4; }

        for (int depth = startDepth; timer.MillisecondsElapsedThisTurn<turnTime; depth++)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                board.MakeMove(moves[i]);
                if (board.IsInCheckmate()) { return moves[i]; }


                moveScores[i] = minMax(board, turn * -1, depth, -10000, 10000); /// Picking odd depths generally better?
                board.UndoMove(moves[i]);
                if (timer.MillisecondsElapsedThisTurn > turnTime) { break; };
            }
            
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
        Console.WriteLine(bestScore);
        return bestMove;
    }
}