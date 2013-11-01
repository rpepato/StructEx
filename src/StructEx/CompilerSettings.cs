using System.Collections.Generic;

namespace StructEx
{
    public class CompilerSettings
    {
        public bool AllowUnsafeBlocks { get; set; }

        public bool CheckForOverflow { get; set; }

        public IList<string> ConditionalSymbols { get; private set; }

        public CompilerSettings()
        {
            ConditionalSymbols = new List<string>();
        }
    }
}
