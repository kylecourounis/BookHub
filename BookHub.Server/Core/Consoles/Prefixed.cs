namespace BookHub.Server.Core.Consoles
{
    using System;
    using System.IO;
    using System.Text;

    internal class Prefixed : TextWriter
    {
        private readonly TextWriter Stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Prefixed"/> class.
        /// </summary>
        internal Prefixed()
        {
            this.Stream             = Console.Out;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// When overridden in a derived class, returns the character encoding in which the output is written.
        /// </summary>
        public override Encoding Encoding => new ASCIIEncoding();

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        public override void WriteLine(string text)
        {
            this.Stream.WriteLine($"[+] {text}");
        }

        /// <summary>
        /// Writes a line terminator to the text string or stream.
        /// </summary>
        public override void WriteLine()
        {
            this.Stream.WriteLine();
        }

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        public override void Write(string text)
        {
            this.Stream.Write(text);
        }
    }
}
