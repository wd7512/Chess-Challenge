using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;

public class MyBot : IChessBot
{
    /// <summary>
    /// int[] pieceValues = { 100, 300, 300, 500, 900 };
    /// </summary>
    Random rng = new();

    class Node
    {
        public Node[] children { get; set; }
        public int MinMaxEval { get; set; }
        public int CurrentEval { get; set; }
        public string FEN { get; set; }
        public Move move { get; set; }
        public bool isCheckmate { get; set; }

        public void GenerateChildren(Board board,Node self) /// this does not need to return anything
        {
            Move[] moves = board.GetLegalMoves();
            self.children = new Node[moves.Length];
            for (int i = 0; i<self.children.Length; i++)
            {
                board.MakeMove(moves[i]);
                self.children[i] = new Node { move = moves[i],FEN = board.GetFenString(),isCheckmate=board.IsInCheckmate()};
                board.UndoMove(moves[i]);
                
            }
           
        }
    }
    public Move Think(Board board, ChessChallenge.API.Timer timer)
    {

        ///Console.WriteLine(board.GetFenString()+"|||"+FenEval(board.GetFenString(),0) + "|||" +Eval(board,0));
        Node tree = new Node { FEN=board.GetFenString()};
        tree.GenerateChildren(board,tree);

        
        
        ///MoveScore[] moveScores = new MoveScore[moves.Length];
        foreach (Node child in tree.children)
        {
            if (child.isCheckmate) { return child.move; }
            child.CurrentEval = FenEval(child.FEN,0);
        }

        Move bestMove = tree.children[0].move;
        int turn = -1;
        if (board.IsWhiteToMove) { turn = 1; }

        int bestScore = int.MinValue * turn;
        for (int i = 0; i < tree.children.Length; i++)
        {
            if (bestScore*turn < tree.children[i].CurrentEval*turn)
            {
                bestScore = tree.children[i].CurrentEval;
                bestMove = tree.children[i].move;
            }
            else if (bestScore == tree.children[i].CurrentEval)
            {
                if (rng.Next(2) == 0)
                {
                    bestMove = tree.children[i].move;
                }
            }
        }
        return bestMove;
        
    }

    
    public int FenEval(string FEN,int num)
    {
        num = 0;
        foreach (char ch in FEN.Split()[0])
        {
            if (ch == 'p') { num--; }
            else if (ch == 'P') { num++; }
            else if (ch == 'n') { num -= 3; }
            else if (ch == 'N') { num += 3; }
            else if (ch == 'b') { num -= 3; }
            else if (ch == 'B') { num += 3; }
            else if (ch == 'r') { num -= 5; }
            else if (ch == 'R') { num += 5; }
            else if (ch == 'q') { num -= 9; }
            else if (ch == 'Q') { num += 9; }
        }
        return num;
    }

}