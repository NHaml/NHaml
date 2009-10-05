namespace NHaml.Core.Ast
{
    public class CodeChunk : ChunkNodeBase
    {
        public CodeChunk(string code)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}