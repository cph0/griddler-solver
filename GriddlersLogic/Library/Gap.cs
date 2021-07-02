﻿using MultiKeyLookup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class Gap : Range
    {
        private readonly MultiKeyLookup<Block> _Blocks;

        public bool HasFirstPoint => _Blocks.ContainsKey("Start", Start);
        public bool HasLastPoint => _Blocks.ContainsKey("End", End);
        public int NumberOfBlocks => _Blocks.Count;
        public bool HasPoints => NumberOfBlocks > 0;
        public bool IsFull => _Blocks.GetValueOrDefault("Start", Start)?.End == End;

        public Gap(int start, int end, IEnumerable<Block>? blocks = null) : base(start, end)
        {
            _Blocks = new MultiKeyLookup<Block>(blocks?.ToArray() ?? Array.Empty<Block>(), 
                        ("Start", k => k.Start), ("End", k => k.End));
        }


        public static bool operator ==(Gap g, Item a)
        {
            return g.Size == a.Value && g.GetBlockAtStart(g.Start)?.Colour == (a.Green ? "green" : "black");
        }

        public static bool operator !=(Gap g, Item a)
        {
            return g.Size != a.Value || g.GetBlockAtStart(g.Start)?.Colour != (a.Green ? "green" : "black");
        }

        public IEnumerable<Block> GetBlocks()
        {
            for (int Index = Start; Index <= End; Index++)
            {
                if (_Blocks.TryGetValue("Start", Index, out Block? Block))
                    yield return Block;
            }
        }

        public Block? GetBlockAtStart(int start)
        {
            if (_Blocks.TryGetValue("Start", start, out Block? Block))
                return Block;

            return null;
        }

        public Block? GetBlockAtEnd(int end)
        {
            if (_Blocks.TryGetValue("End", end, out Block? Block))
                return Block;

            return null;
        }

        public Block? GetLastBlock(int start)
        {
            for (int Index = start; Index >= Start; Index--)
            {
                Block? Block = GetBlockAtEnd(Index);
                if (Block != null)
                    return Block;
            }

            return null;
        }

        public Block? GetNextBlock(int start)
        {
            for (int Index = start; Index <= End; Index++)
            {
                Block? Block = GetBlockAtStart(Index);
                if (Block != null)
                    return Block;
            }

            return null;
        }

        public Gap SetStart(int start)
        {
            Start = start;
            return this;
        }

        public Gap SetEnd(int end)
        {
            End = end;
            return this;
        }

        public Gap SplitGap(int index)
        {
            List<Block> Blocks = new List<Block>(_Blocks.Count);
            for (int Index = index + 1; Index <= End; Index++)
            {
                if (_Blocks.TryGetValue("Start", Index, out Block? Block))
                {
                    Blocks.Add(Block);
                    _Blocks.Remove("Start", Index);
                }
            }

            Gap RightGap = new Gap(index + 1, End, Blocks);
            SetEnd(index - 1);
            return RightGap;
        }

        private void AddBlock(int index, string colour, int? item)
        {
            Block? LeftBlock = _Blocks.GetValueOrDefault("End", index - 1);
            Block? RightBlock = _Blocks.GetValueOrDefault("Start", index + 1);
            int Start = index;
            int End = index;

            if (LeftBlock != null && LeftBlock.Colour == colour)
            {
                _Blocks.Remove("End", index - 1);
                Start = LeftBlock.Start;
            }

            if (RightBlock != null && RightBlock.Colour == colour)
            {
                _Blocks.Remove("Start", index + 1);
                End = RightBlock.End;
            }

            _Blocks.Add(new Block(Start, End, colour == "green"));
        }

        public void AddPoint(int index, string colour, int? item)
        {
            AddBlock(index, colour, item);


        }
    }
}
