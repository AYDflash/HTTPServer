using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading.Tasks;

namespace HTTP_Server_throughHttplistener
{
    class HTTPServer
    {

        HttpListener server;
        string name;
        bool flag = true;

        public  void StartServer(string prefix)
        {
            server = new HttpListener();
            //Текущая ОС не поддерживается
            if (!HttpListener.IsSupported) return;
            //добавление префикса say
            //обязательно в конце должен быть слэш /
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentException("prefix");
            //Добавление префикса
            server.Prefixes.Add(prefix);
            //Запуск сервера
            server.Start();
            Console.WriteLine("Сервер запущен!");
            //прослушиваем входящие соединения
            while (server.IsListening)
            {
                //Ожидаем входящие запросы
                HttpListenerContext context = server.GetContext();
                //получаем входящий запрос
                HttpListenerRequest request = context.Request;
                //Обработка POST запроса
                //запрос получен методом POST
                if (request.HttpMethod == "POST")
                {
                    //Показать, что пришло от клиента
                    ShowRequestData(request);
                    //завершаем работу сервера
                    if (!flag) return;
                }
                //Формировка ответа сервера
                //динамически создаем страницу
                string htmlpage = @"<!DOCTYPE html>
                <html><head></head><body>
                <form method=""post"" action=""say"">
                <p><b>Введите последовательность целых чисел</b><br>
                <input type=""text"" name=""myname"" size=""40""></p>
                <p><input type=""submit"" value=""Отправить""></p>
                <p><b>Результат: {0}</b><br>
                </form></body></html>";
                string responseString = string.Format(htmlpage, name);
                
                //Отправка данных клиенту
                HttpListenerResponse response = context.Response;
                response.ContentType = "text/html; charset=UTF-8";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;

                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public void ShowRequestData(HttpListenerRequest _request)
        {
            //Есть ли данные от клиента?!
            if (!_request.HasEntityBody) return;

            //Смотрим, что пришло
            using (Stream body = _request.InputStream)
            {
                using (StreamReader reader = new StreamReader(body))
                {

                    string text = reader.ReadToEnd();
                    //Удаляем подстроку "myname="
                    text = text.Remove(0, 7);
                    //Преобразуем сообщение в UTF8
                    text = System.Web.HttpUtility.UrlDecode(text, Encoding.UTF8);
                    Console.WriteLine(text);
                    //Выполнение задачи
                    string[] arr = text.Split(' ');
                    int[] array = new int[arr.Length];
                    int summ = 0;

                    for (int i = 0; i < arr.Length; i++)
                    {
                        
                        array[i] = Convert.ToInt32(arr[i]);
                        summ += array[i];
                    }
                    text = Convert.ToString(summ);
                    Console.WriteLine("Среднее значение: {0}", text);
                    //Переменная ответа клиенту
                    name = text;
                    flag = true;
                    //Останавливаем сервер
                    if (text == "stop")
                    {
                        server.Stop();
                        Console.WriteLine("Сервер остановлен!");
                        flag = false;
                    }
                }
            }
        }
    }
}
