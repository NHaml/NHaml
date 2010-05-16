namespace NHaml.Core.Ast
{
    public class TextChunk : ChunkNodeBase
    {
        public TextChunk(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}