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
        public bool isDraw { get; set; }

        public void GenerateChildren(Board board,Node self) /// this does not need to return anything
        {
            Move[] moves = board.GetLegalMoves();
            self.children = new Node[moves.Length];
            for (int i = 0; i<self.children.Length; i++)
            {
                board.MakeMove(moves[i]);
                self.children[i] = new Node 
                { 
                    move = moves[i],
                    FEN = board.GetFenString(),
                    isCheckmate= board.IsInCheckmate(),
                    isDraw = board.IsDraw()
                };
                board.UndoMove(moves[i]);
            }
        }
        public void ScoreChildren(Node self)
        {
            foreach (Node child in self.children)
            {
                if (child.isDraw) { child.CurrentEval = 0; }
                else { child.CurrentEval = self.FenEval(child.FEN, 0); }
            }
        }

        public int FenEval(string FEN, int num)
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
    public Move Think(Board board, ChessChallenge.API.Timer timer)
    {

        int turn = -1;
        if (board.IsWhiteToMove) { turn = 1; }
        int bestScore = int.MinValue * turn;

        Node tree = new Node { FEN=board.GetFenString()};
        ///Console.WriteLine(tree.FenEval(tree.FEN,0));
        tree.GenerateChildren(board,tree);

        
     
        foreach (Node child in tree.children)
        {
            if (child.isCheckmate) { return child.move; }
        }

        foreach (Node child in tree.children)
        {
            board.MakeMove(child.move);
            child.GenerateChildren(board,child);
            child.ScoreChildren(child);
            bestScore = int.MinValue * turn * -1;
            foreach (Node subChild in child.children)
            {
                
                if (bestScore * turn * -1 < subChild.CurrentEval * turn * -1)
                {
                    bestScore = subChild.CurrentEval;
                }
            }
            child.CurrentEval = bestScore;
            board.UndoMove(child.move);

            
        }



        ///tree.ScoreChildren(tree);

        Move bestMove = tree.children[0].move;


        bestScore = int.MinValue * turn;
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

    


}