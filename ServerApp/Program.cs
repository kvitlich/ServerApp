using Newtonsoft.Json.Linq;
using System;
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
            //FillDataBase();
            Console.WriteLine("DB is filled");

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            SmsReader();
            //DDConverter obje = new DDConverter();

            //Console.WriteLine(obje.FromObject(new List<User>() { new User(), new User() }));
            Console.ReadLine();
        }

        private async static void SmsReader()
        {
            await SmsReaderTask();
        }

        private async static Task SmsReaderTask()
        {
            //var listened = new TcpListener(IPAddress.Parse("10.1.4.90"), 3231);
            var listened = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);

            listened.Start();
            Console.WriteLine("Сервер запущен");

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

                        Console.WriteLine(resultText);

                        JObject json = new JObject();
                        json = JObject.Parse(resultText);
                        string result = json["Action"].ToString();

                        if (result == "Give")
                        {
                            DDConverter converter = new DDConverter();
                            //string answer = @"[table:Users][column][name:Full Name][data:Dias\Dinara\Chmawnik\][/column][column][name:Age][data:16\20\350\][/column][column][name:Gender][data:male\female\nothing\][/column][/table]";

                            string answer = String.Empty;
                            using (var context = new ServerDbContext())
                            {
                                answer = converter.FromObject(context.Users.OrderBy(user => user.Id).Where(user => user.DeletedDate == null).ToList());
                            }

                            // 2 ответа
                            var answerBytesQuantity = System.Text.Encoding.UTF8.GetBytes($"{answer.Length}"); // кол-во байтов, клиент создаст буфер
                            stream.Write(answerBytesQuantity, 0, answerBytesQuantity.Length);

                            var answerBytes = System.Text.Encoding.UTF8.GetBytes(answer);
                            stream.Write(answerBytes, 0, answerBytes.Length);
                        }
                        else if (result == "Add")
                        {
                            result = json["Data"].ToString();
                            var newUserData = result.Split('\\');

                            using (var context = new ServerDbContext())
                            {
                                var user = new User { Name = newUserData[0], Age = newUserData[1] };
                                context.Add(user);
                                await context.SaveChangesAsync();
                            }

                            //Console.WriteLine($"Name = {newUserData[0]}, Age = {newUserData[1]}");

                            var answer = "Новый User добавлен.";
                            var answerBytes = System.Text.Encoding.UTF8.GetBytes(answer);

                            stream.Write(answerBytes, 0, answerBytes.Length);
                        }
                        else if (result == "Update")
                        {
                            result = json["Data"].ToString();
                            var updateUserData = result.Split('\\');
                            int number = Int32.Parse(updateUserData[0]);

                            using (var context = new ServerDbContext())
                            {
                                var chosenUser = context.Users.OrderBy(user => user.Id).Where(user => user.DeletedDate == null).Skip(number-1).Take(1).FirstOrDefault();
                                chosenUser.Name = updateUserData[1];
                                chosenUser.Age = updateUserData[2];

                                await context.SaveChangesAsync();
                            }

                            //Console.WriteLine($"Name = {updateUserData[1]}, Age = {updateUserData[2]}");

                            var answer = "User изменён.";
                            var answerBytes = System.Text.Encoding.UTF8.GetBytes(answer);
                            stream.Write(answerBytes, 0, answerBytes.Length);
                        }
                        else if (result == "Remove")
                        {
                            result = json["Data"].ToString();
                            var removeUserData = result.Split('\\');
                            int number = Int32.Parse(removeUserData[0]);

                            using (var context = new ServerDbContext())
                            {
                                var chosenUser = context.Users.OrderBy(user => user.Id).Where(user => user.DeletedDate == null).Skip(number-1).Take(1).FirstOrDefault();
                                chosenUser.DeletedDate = DateTime.Now;

                                await context.SaveChangesAsync();
                            }

                            //Console.WriteLine($"Number = {number}");

                            var answer = "User удалён.";
                            var answerBytes = System.Text.Encoding.UTF8.GetBytes(answer);
                            stream.Write(answerBytes, 0, answerBytes.Length);
                        }
                    }
                }
            }
        }

        private static void FillDataBase()
        {
            using (var context = new ServerDbContext())
            {
                var userFirst = new User { Name = "Dias", Age = "32" };
                var userSecond = new User { Name = "Dinazavr", Age = "180" };
                context.Users.Add(userFirst);
                context.Users.Add(userSecond);
                context.SaveChanges();
            }
        }
    }
}