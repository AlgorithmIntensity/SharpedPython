using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace SharpedPython
{
    internal class Flask
    {
        private HttpListener listener;
        private Dictionary<string, Dictionary<string, Func<HttpListenerRequest, string>>> routes;
        private int port;

        public Flask()
        {
            listener = new HttpListener();
            routes = new Dictionary<string, Dictionary<string, Func<HttpListenerRequest, string>>>();
        }

        // Декоратор для маршрутов
        public void route(string path, string method = "GET")
        {
            if (!routes.ContainsKey(method))
                routes[method] = new Dictionary<string, Func<HttpListenerRequest, string>>();

            routes[method][path] = null; // Будет установлено позже
        }

        // GET запрос
        public void get(string path, Func<HttpListenerRequest, string> handler)
        {
            if (!routes.ContainsKey("GET"))
                routes["GET"] = new Dictionary<string, Func<HttpListenerRequest, string>>();

            routes["GET"][path] = handler;
        }

        // POST запрос
        public void post(string path, Func<HttpListenerRequest, string> handler)
        {
            if (!routes.ContainsKey("POST"))
                routes["POST"] = new Dictionary<string, Func<HttpListenerRequest, string>>();

            routes["POST"][path] = handler;
        }

        // Запуск сервера
        public void run(int port = 5000)
        {
            this.port = port;
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();

            Console.WriteLine($" * Serving Flask app 'SharpedPython'");
            Console.WriteLine($" * Running on http://localhost:{port}");

            ListenAsync().GetAwaiter().GetResult();
        }

        private async Task ListenAsync()
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                ProcessRequest(context);
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            string method = request.HttpMethod;
            string path = request.Url.AbsolutePath;

            string responseString = "";

            try
            {
                if (routes.ContainsKey(method) && routes[method].ContainsKey(path))
                {
                    var handler = routes[method][path];
                    if (handler != null)
                    {
                        responseString = handler(request);
                    }
                    else
                    {
                        responseString = $"<h1>Hello from {method} {path}!</h1>";
                    }
                }
                else
                {
                    response.StatusCode = 404;
                    responseString = "<h1>404 Not Found</h1>";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                responseString = $"<h1>500 Internal Server Error</h1><p>{ex.Message}</p>";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        // Отправка JSON
        public string json(object data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data);
        }

        // Отправка HTML
        public string render(string html)
        {
            return html;
        }

        // Получение параметров запроса
        public Dictionary<string, string> get_args(HttpListenerRequest request)
        {
            var args = new Dictionary<string, string>();
            foreach (string key in request.QueryString.Keys)
            {
                if (key != null)
                    args[key] = request.QueryString[key];
            }
            return args;
        }

        // Получение данных формы
        public Dictionary<string, string> get_form(HttpListenerRequest request)
        {
            var form = new Dictionary<string, string>();
            if (request.HasEntityBody)
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string body = reader.ReadToEnd();
                    var pairs = body.Split('&');
                    foreach (var pair in pairs)
                    {
                        var keyValue = pair.Split('=');
                        if (keyValue.Length == 2)
                        {
                            form[Uri.UnescapeDataString(keyValue[0])] =
                                Uri.UnescapeDataString(keyValue[1]);
                        }
                    }
                }
            }
            return form;
        }

        // Статические файлы
        public void static_folder(string folderPath)
        {
            get("/static/{filename}", (request) =>
            {
                string filename = request.Url.Segments.Last();
                string filePath = Path.Combine(folderPath, filename);

                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                return "<h1>404 File Not Found</h1>";
            });
        }

        // Редирект
        public void redirect(HttpListenerResponse response, string url)
        {
            response.StatusCode = 302;
            response.Headers.Add("Location", url);
        }

        // Шаблонизатор (простой)
        public string render_template(string templateName, Dictionary<string, object> variables = null)
        {
            string templatePath = Path.Combine("templates", templateName);
            if (File.Exists(templatePath))
            {
                string content = File.ReadAllText(templatePath);

                if (variables != null)
                {
                    foreach (var kvp in variables)
                    {
                        content = content.Replace($"{{{{ {kvp.Key} }}}}", kvp.Value.ToString());
                    }
                }

                return content;
            }
            return $"<h1>Template {templateName} not found</h1>";
        }

        // Остановка сервера
        public void stop()
        {
            listener.Stop();
            listener.Close();
        }
    }
}