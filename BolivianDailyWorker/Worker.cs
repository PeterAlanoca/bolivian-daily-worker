using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Npgsql;

namespace BolivianDailyWorker
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Iniciando tarea de Web Scraping...");


            var newsListUrls = await GetNewsListUrls();

            foreach (var newsUrl in newsListUrls)
            {
                var news = await GetNews(newsUrl);
                var id = await InsertNews(news);

                if (news != null && !string.IsNullOrEmpty(news.Title))
                {
                    logger.LogInformation($"Noticia Parseada | TÍTULO: {id} -> {news.Title}");
                }
            }

        }


        private async Task<List<string>> GetNewsListUrls()
        {
            logger.LogInformation("Iniciando Web Scraping usando SELENIUM...");
            string url = "https://jornada.com.bo/seccion/economia/";
            new DriverManager().SetUpDriver(new ChromeConfig());
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");

                using IWebDriver driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl(url);

                await Task.Delay(1000);

                var elementosUrl = driver.FindElements(By.CssSelector(".td-module-thumb a, .td-module-title a"));
                List<string> listaUrls = new List<string>();

                if (elementosUrl.Count > 0)
                {
                    foreach (var nodo in elementosUrl)
                    {
                        var enlace = nodo.GetAttribute("href");
                        if (!string.IsNullOrEmpty(enlace) && !listaUrls.Contains(enlace))
                        {
                            listaUrls.Add(enlace);
                        }
                    }

                    logger.LogInformation($"¡Éxito! Se recuperaron {listaUrls.Count} URLs únicas de las clases td-module-thumb y td-module-title:");
                }
                else
                {
                    logger.LogWarning("No se encontraron elementos con las clases td-module-thumb o td-module-title.");
                }

                driver.Quit();
                return listaUrls;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocurrió un error leyendo con Selenium.");
                return [];
            }
        }

        private async Task<News> GetNews(string url)
        {
            logger.LogInformation($"Extrayendo noticia de: {url}");
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            string title = "";
            string bodyHtml = "";

            try
            {
                using IWebDriver driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl(url);

                await Task.Delay(1000); 
                try
                {
                    var titleNode = driver.FindElement(By.ClassName("tdb-title-text"));
                    title = titleNode.Text.Trim();
                }
                catch (Exception) { /* No se encontró el título */ }

                try
                {
                    var pNodes = driver.FindElements(By.CssSelector(".tdb_single_content .tdb-block-inner p, .td-post-content p"));
                    
                    if (pNodes.Count > 0)
                    {
                        var sb = new System.Text.StringBuilder();
                        foreach (var node in pNodes)
                        {
                            var inner = node.GetAttribute("innerHTML").Trim();
                            if (!string.IsNullOrEmpty(inner))
                            {
                                sb.AppendLine($"<p>{inner}</p>");
                            }
                        }
                        bodyHtml = sb.ToString().Trim();
                    }
                }
                catch (Exception)
                {
                }

                driver.Quit();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error obteniendo noticia desde: {url}");
            }

            return new News
            {
                Title = title,
                Body = bodyHtml
            };
        }

        private async Task<long> InsertNews(News news)
        {
            string connectionString = "Host=localhost;Database=bolivian_daily;Username=postgres;Password=root";
            
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO public.news (title, body, created_at) 
                VALUES (@Title, @Body, @CreatedAt) 
                RETURNING id;";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("Title", news.Title ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("Body", news.Body ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("CreatedAt", DateTime.Now);

            var newId = await command.ExecuteScalarAsync();
            return Convert.ToInt64(newId);
        }

    }
}
