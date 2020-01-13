using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //using (var context = new ServerDbContext())
            //{
            //    var userFirst = new User { Name = "Dias", Age = "32" };
            //    var userSecond = new User { Name = "Dinazavr", Age = "180" };
            //    context.Users.Add(userFirst);
            //    context.Users.Add(userSecond);
            //    context.SaveChanges();
            //}
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            SmsReader();
            DDConverter obje = new DDConverter();

            Console.WriteLine(obje.FromObject(new List<User>() { new User(), new User() }));
            Console.ReadLine();

        }



        private async static void SmsReader()
        {
            await SmsReaderTask();
        }

        private async static Task SmsReaderTask()
        {
            var listened = new TcpListener(IPAddress.Parse("10.1.4.90"), 3231);

            listened.Start();
            Console.WriteLine("Сервер заупущен");

            //TcpClient client = listened.AcceptTcpClient();
            //NetworkStream stream = client.GetStream();
            while (true)
            {
                using (TcpClient client = await listened.AcceptTcpClientAsync())
                {

                    using (NetworkStream stream = client.GetStream())
                    {
                        var resultText = String.Empty;

                        do
                        {
                            var buffer = new byte[1024];
                            await stream.ReadAsync(buffer, 0, buffer.Length);

                            resultText += System.Text.Encoding.UTF8.GetString(buffer);

                        }
                        while (stream.DataAvailable);
                        DDConverter converter = new DDConverter();
                        string answer;
                        using (var context = new ServerDbContext())
                        {
                            answer = converter.FromObject(context.Users.ToList());
                        }
                        // 2 ответа
                        var answerBytesQuantity = System.Text.Encoding.UTF8.GetBytes($"{answer.Length}"); // кол-во байтов, клиент создаст буфер
                        stream.Write(answerBytesQuantity, 0, answerBytesQuantity.Length);
                        stream.Write(System.Text.Encoding.UTF8.GetBytes(answer), 0, System.Text.Encoding.UTF8.GetBytes(answer).Length);

                        Console.WriteLine($"Message: {answer}");
                    }

                }
                //  var answer = System.Text.Encoding.UTF8.GetBytes("Запрос получен");
                //   stream.Write(answer, 0, answer.Length);

            }

            listened.Stop();
            Console.WriteLine("Сервер Ввключегн");

        }
    }
}
