using JobsScraper.BLL.Interfaces.DOU;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobsScraper.BLL.Services.DOU
{
    public class DouHtmlLoader : IDouHtmlLoader
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<DouHtmlLoader> logger;

        public DouHtmlLoader(IConfiguration configuration, ILogger<DouHtmlLoader> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public Task<string?> LoadJobBoardHTMLAsync(string requestString, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var options = new ChromeOptions();
                options.AddArgument("--no-sandbox");
                options.AddArgument("--headless");
                options.AddArgument("--disable-dev-shm-usage");

                IWebDriver driver = new ChromeDriver(options);
                string? douHtml = default;

                try
                {
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

                    douHtml = driver.PageSource ?? null;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error while loading data from DOU");
                }
                finally
                {
                    driver.Close();
                    driver.Quit();
                }

                return douHtml;
            });
        }
    }
}
