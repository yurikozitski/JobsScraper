using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobsScraper.BLL.Services.DOU
{
    public class DouHtmlLoader : IDouHtmlLoader
    {
        private readonly IDouRequestStringBuilder douRequestStringBuilder;
        private readonly IConfiguration configuration;

        public DouHtmlLoader(IDouRequestStringBuilder douRequestStringBuilder, IConfiguration configuration)
        {
            this.douRequestStringBuilder = douRequestStringBuilder;
            this.configuration = configuration;
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
                    if (token.IsCancellationRequested)
                    {
                        driver.Quit();
                        token.ThrowIfCancellationRequested();
                    }

                    IWebElement? loadMoreButton;

                    try
                    {
                        loadMoreButton = driver.FindElement(By.CssSelector(this.configuration["DOU:XPaths:LoadMoreButton"])).FindElement(By.TagName("a"));
                    }
                    catch (NoSuchElementException)
                    {
                        break;
                    }

                    if (loadMoreButton == null || !loadMoreButton.Displayed)
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
