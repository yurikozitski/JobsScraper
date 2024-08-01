using JobsScraper.BLL.Interfaces.RobotaUa;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobsScraper.BLL.Services.RobotaUa
{
    public class RobotaUaHtmlLoader : IRobotaUaHtmlLoader
    {
        public async Task<string?> LoadJobBoardHTMLAsync(string requestString, CancellationToken token)
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--headless");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArguments("window-size=800,10000");

            IWebDriver driver = new ChromeDriver(options);
            string? robotaUaHtml;

            try
            {
                await driver.Navigate().GoToUrlAsync(requestString);
                robotaUaHtml = driver.PageSource;
            }
            finally
            {
                driver.Close();
                driver.Quit();
            }

            return robotaUaHtml;
        }
    }
}
