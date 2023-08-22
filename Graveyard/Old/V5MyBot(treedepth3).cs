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
    

    class Node
    {
        public Node[] children { get; set; }
        public int CurrentEval { get; set; }
        public string FEN { get; set; }
        public Move move { get; set; }
        public bool isCheckmate { get; set; }
        public bool isDraw { get; set; }
        public bool isLeaf { get; set; }
        public int Turn { get; set; }

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
                    isDraw = board.IsDraw(),
                    Turn = self.Turn*-1
                };
                board.UndoMove(moves[i]);
                self.children[i].isLeaf = self.children[i].isCheckmate | self.children[i].isDraw;
            }
        }

        public void Eval(Node self)
        {
            if (self.isDraw) { self.CurrentEval = 0; }
            else if (self.isCheckmate) { self.CurrentEval = int.MinValue*self.Turn; }
            else
            {
                self.CurrentEval = 0;
                foreach (char ch in self.FEN.Split()[0])
                {
                    if (ch == 'p') { self.CurrentEval--; }
                    else if (ch == 'P') { self.CurrentEval++; }
                    else if (ch == 'n') { self.CurrentEval -= 3; }
                    else if (ch == 'N') { self.CurrentEval += 3; }
                    else if (ch == 'b') { self.CurrentEval -= 3; }
                    else if (ch == 'B') { self.CurrentEval += 3; }
                    else if (ch == 'r') { self.CurrentEval -= 5; }
                    else if (ch == 'R') { self.CurrentEval += 5; }
                    else if (ch == 'q') { self.CurrentEval -= 9; }
                    else if (ch == 'Q') { self.CurrentEval += 9; }
                }
            }
        }

        public int recursiveSearch(Board board, ChessChallenge.API.Timer timer,int maxTime,Node self)
        {
            if (timer.MillisecondsElapsedThisTurn > maxTime)
            {
                self.Eval(self);
                return self.CurrentEval;
            }

            self.CurrentEval = int.MinValue * self.Turn;
            self.GenerateChildren(board, self);
            foreach (Node child  in self.children)
            {
                if (child.isLeaf) { child.Eval(child); }
                else
                {
                    board.MakeMove(child.move);
                    child.CurrentEval = child.recursiveSearch(board, timer, maxTime, child);
                    board.UndoMove(child.move);
                }
                if (self.CurrentEval * self.Turn < child.CurrentEval * self.Turn)
                {
                    self.CurrentEval = child.CurrentEval;
                }
            }

            return self.CurrentEval;
        }
    }
    public Move Think(Board board, ChessChallenge.API.Timer timer)
    {
        
        int turn = -1;
        if (board.IsWhiteToMove) { turn = 1; }
        

        Node tree = new Node { FEN=board.GetFenString(),Turn=turn};
        tree.GenerateChildren(board,tree);

        foreach (Node child in tree.children) 
        { 
            if (child.isCheckmate) { return child.move; } /// play checkmate if possible
            else /// generate a score for each child
            {
                if (child.isLeaf) /// if it is leaf just score its position
                {child.Eval(child);}
                else /// Generate Depth 2
                {
                    board.MakeMove(child.move);
                    child.GenerateChildren(board,child);
                    child.CurrentEval = int.MinValue*child.Turn; /// a number to "beat" to find the optimal move
                    foreach (Node subChild in child.children) /// for each of the 2nd layer nodes
                    {
                        if (subChild.isLeaf)
                        {subChild.Eval(subChild);}
                        else /// this is where further searching would go
                        {
                            board.MakeMove(subChild.move);
                            subChild.GenerateChildren(board,subChild);
                            subChild.CurrentEval = int.MinValue*subChild.Turn;
                            foreach (Node subSubChild in subChild.children)
                            {
                                if (subSubChild.isLeaf)
                                {
                                    subSubChild.Eval(subSubChild);
                                }
                                else
                                {
                                    subSubChild.Eval(subSubChild);
                                }
                                if (subChild.CurrentEval*subChild.Turn < subSubChild.CurrentEval*subChild.Turn)
                                {
                                    subChild.CurrentEval = subSubChild.CurrentEval;
                                }
                            }

                            board.UndoMove(subChild.move);
                        }

                        if (child.CurrentEval * child.Turn < subChild.CurrentEval * child.Turn)
                        {
                            child.CurrentEval = subChild.CurrentEval;
                        }
                    }
                    
                    board.UndoMove(child.move);
                }
            }
        }



        Move bestMove = tree.children[0].move;
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
                Random rng = new();
                if (rng.Next(2) == 0)
                {
                    bestMove = tree.children[i].move;
                }
            }
        }
        ///Console.WriteLine(bestScore);
        return bestMove;
    }
}