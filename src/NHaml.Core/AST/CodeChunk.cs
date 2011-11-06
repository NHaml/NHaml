namespace NHaml.Core.Ast
{
    public class CodeChunk : ChunkNodeBase
    {
      /*  public CodeChunk(string code)
        {
            Code = code;
        }*/

        public CodeChunk(string code, bool? escape)
        {
            Code = code;
            Escape = escape;
        }

        public string Code { get; private set; }
        public bool? Escape { get; set; }
    }
}