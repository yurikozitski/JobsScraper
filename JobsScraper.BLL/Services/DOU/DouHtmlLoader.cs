using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobsScraper.BLL.Services.DOU
{
    public class DouHtmlLoader : IDouHtmlLoader
    {
        private readonly IDouRequestStringBuilder douRequestStringBuilder;

        public DouHtmlLoader(IDouRequestStringBuilder douRequestStringBuilder)
        {
            this.douRequestStringBuilder = douRequestStringBuilder;
        }

        public Task<string?> LoadJobBoardHTMLAsync(JobSearchModel jobSearchModel, CancellationToken token)
        {
            return Task.Run(() =>
            {
                string requestString = this.douRequestStringBuilder.GetRequestString(jobSearchModel);

                var options = new ChromeOptions();
                options.AddArguments(new List<string>() { "headless", "disable-gpu" });
                IWebDriver driver = new ChromeDriver(options);

                driver.Navigate().GoToUrl(requestString);

                while (true)
                {
                    IWebElement loadMoreButton;

                    try
                    {
                        loadMoreButton = driver.FindElement(By.CssSelector("div[class='more-btn']")).FindElement(By.TagName("a"));
                    }
                    catch (NoSuchElementException)
                    {
                        break;
                    }

                    if (!loadMoreButton.Displayed)
                    {
                        break;
                    }

                    loadMoreButton.Click();
                }

                string? douHtml = driver.PageSource ?? null;
                driver.Quit();

                return douHtml;
            });
        }
    }
}
