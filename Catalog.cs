//using SerializeToFile;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SQLite;
using System.IO;
// System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
// System.Transactions;
using System.Xml.Serialization;

namespace lab5
{

    internal class Catalog
    {
        public List<Composition> compositions;
        private string menu;
        private uint part;
        string input2, input3;
        public Catalog()
        {
            compositions = new List<Composition>();
        }

        public string setResponse(string response)
        {
            string warning = string.Empty;
            part++;
            if (part == 1) menu = response;
            else if (part == 2) input2 = response;
            if (part == 3) input3 = response;

            switch (menu)
            {
                case "load":
                    if (part == 2)
                    {
                        part = 0;   //Загрузка каталога
                        compositions.Clear();
                        if (input2 == "json") warning = KeepData_JSON(false);
                        else if (input2 == "xml") warning = KeepData_XML(false);
                        else if (input2 == "sql") warning = KeepData_SQL(false);
                    }
                    break;
                case "save":
                    if (part == 2)
                    {
                        part = 0;   //Сохранение каталога
                        if (input2 == "json") warning = KeepData_JSON(true);
                        else if (input2 == "xml") warning = KeepData_XML(true);
                        else if (input2 == "sql") warning = KeepData_SQL(true);
                    }
                    break;
                case "list":
                    part = 0;       //Вывод каталога на печать
                    foreach (Composition item in compositions)
                    {
                        warning += item.ToString() + "\n";
                    }
                    warning += "Команда выполнена!";
                    break;
                case "search":
                    if (part == 2)
                    {
                        part = 0;   //Выполнение поиска по шаблону
                        foreach (Composition item in compositions)
                        {
                            if (item.ToString().Contains(input2))
                            {
                                warning += item.ToString() + "\n";
                            }
                        }
                        warning += "Команда выполнена!";
                    }
                    break;
                case "add":
                    if (part == 3)
                    {
                        part = 0;   //Добавление композиции
                        compositions.Add(new Composition(input2, input3));
                        warning += "Команда выполнена!";
                    }
                    break;
                case "del":
                    if (part == 2)
                    {
                        part = 0;   //Выполнение поиска по полному названию и удаление
                        foreach (Composition item in compositions)
                        {
                            if (input2 == item.ToString())
                            {
                                compositions.Remove(item);
                                warning += "Команда выполнена!";
                                break;
                            }
                        }
                        if (warning.Length == 0)
                            warning += "Композиция для удаления не найдена!";
                    }
                    break;
                    default:
                    part = 0;
                    menu = string.Empty;
                    warning = "Введена некорректная команда!!!";
                    break;
            }

            return warning;
        }

        public string KeepData_JSON(bool flagSave)
        {
            string answer = string.Empty;
            string fileName = "Catalog.json";

            try
            {
                if (flagSave)
                {
                    //Сохранение данных из файла формата JSON
                    StreamWriter sw = new StreamWriter(fileName);
                    foreach (Composition item in compositions)
                    {
                        sw.WriteLine(JsonSerializer.Serialize(item));
                    }
                    sw.Close();
                    //Console.WriteLine(File.ReadAllText(fileName));
                    answer = "Данные успешно сохранены (JSON)";
                }
                else
                {
                    //Загрузка данных из файла формата JSON
                    StreamReader sr = new StreamReader(fileName);
                    String jsonString = sr.ReadLine();
                    while (jsonString != null)
                    {
                        //Console.WriteLine(jsonString);
                        Composition item = JsonSerializer.Deserialize<Composition>(jsonString);
                        //Console.WriteLine($"Author: {item?.author}");
                        //Console.WriteLine($"Song: {item?.song}");
                        compositions.Add(item);
                        jsonString = sr.ReadLine();
                    }
                    sr.Close();
                    answer = "Данные успешно загружены (JSON)";
                }
            }
            catch (Exception e)
            {
                answer = "Произошла ошибка при выполнении команды: " + e.Message;
                //Console.WriteLine("Exception: " + e.Message);
            }

            return answer;
        }

        public string KeepData_XML(bool flagSave)
        {
            string answer = string.Empty;
            string fileName = "Catalog.xml";
            try
            {
                if (flagSave)
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(List<Composition>));
                    StreamWriter sw = new StreamWriter(fileName);
                    mySerializer.Serialize(sw, compositions);
                    sw.Close();
                    //Console.WriteLine(File.ReadAllText(fileName));
                    answer = "Данные успешно сохранены (XML)";
                }
                else
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(List<Composition>));
                    StreamReader sr = new StreamReader(fileName);
                    compositions = (List<Composition>)mySerializer.Deserialize(sr);
                    sr.Close();
                    answer = "Данные успешно загружены (XML)" + compositions.Count;
                }
            }
            catch (Exception e)
            {
                answer = "Произошла ошибка при выполнении команды: " + e.Message;
                //Console.WriteLine("Exception: " + e.Message);
            }
            return answer;
        }

        public string KeepData_SQL(bool flagSave)
        {
            string answer = string.Empty;
            try
            {
                SQLiteConnection connection = new SQLiteConnection("Data Source=Catalog.db;Version=3; FailIfMissing=False");
                connection.Open();
                //Console.WriteLine("Соединение создано");

                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "CREATE TABLE IF NOT EXISTS [Catalog]([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [author] TEXT, [song] TEXT);";
                command.ExecuteNonQuery();
                //Console.WriteLine("Таблица создана");

                if (flagSave)
                {
                    command.CommandText = "DELETE FROM Catalog";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Catalog (author, song) VALUES (:author, :song)";
                    SQLiteTransaction transaction = connection.BeginTransaction();//запускаем транзакцию
                    try
                    {
                        foreach (Composition item in compositions)
                        {
                            command.Parameters.AddWithValue("author", item.author);
                            command.Parameters.AddWithValue("song", item.song);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit(); //применяем изменения
                        answer = "Данные успешно сохранены (SQL)";
                    }
                    catch
                    {
                        transaction.Rollback(); //откатываем изменения, если произошла ошибка
                        throw;
                    }

                }
                else
                {
                    command.CommandText = "SELECT * FROM Catalog";
                    DataTable data = new DataTable();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    adapter.Fill(data);
                    //Console.WriteLine($"Прочитано {data.Rows.Count} записей из таблицы БД");
                    foreach (DataRow row in data.Rows)
                    {
                        //Console.WriteLine($"id = {row.Field<long>("id")} author = {row.Field<string>("author")} song = {row.Field<string>("song")}");
                        compositions.Add(new Composition($"{row.Field<string>("author")}", $"{row.Field<string>("song")}"));
                    }
                    answer = "Данные успешно загружены (SQL)";

                }
            }
            catch (SQLiteException e)
            {
                answer = "Произошла ошибка при выполнении команды: " + e.Message;
                // Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {e.Message}");
            }
            return answer;
        }
    }
}
