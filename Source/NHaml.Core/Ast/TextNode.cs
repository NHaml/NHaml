using System;
using System.Collections.Generic;

namespace NHaml.Core.Ast
{
    public class TextNode : AstNode
    {
        public TextNode()
        {
            Chunks = new List<ChunkNodeBase>();
        }

        public TextNode(ChunkNodeBase chunk)
        {
            if(chunk == null)
                throw new ArgumentNullException("chunk");

            Chunks = new List<ChunkNodeBase> {chunk};
        }

        public List<ChunkNodeBase> Chunks { get; private set; }
    }
}