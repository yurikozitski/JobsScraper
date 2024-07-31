namespace JobsScraper.BLL.Interfaces
{
    public interface IHtmlLoader
    {
        Task<string?> LoadJobBoardHTMLAsync(string requestString, CancellationToken token);
    }
}
