namespace JobsScraper.BLL.Exceptions
{
    public class ParsingException : Exception
    {
        public string JobBoard { get; }

        public string XPath { get; }

        public ParsingException(string message, string jobBoard, string xPath)
            : base(message)
        {
            this.JobBoard = jobBoard;
            this.XPath = xPath;
        }
    }
}
