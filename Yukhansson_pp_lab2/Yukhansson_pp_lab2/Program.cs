using System;
using System.IO;
using System.Threading;

namespace Yukhansson_pp_lab2
{    
    class Program
    {
        static Semaphore SemNoR = new Semaphore(1, 1); //семафор доступа к количеству читателей
        static Semaphore a = new Semaphore(1, 1);//семафор доступа к чтению/записи из файла
        static int numberOfReaders = 0;
        static void Main(string[] args)
        {
            Thread[] threads = new Thread[15];
            for(int i = 0; i < 10; i++)
            {
                if (i%2==0)
                {
                    threads[i] = new Thread(Read);
                }
                else
                {
                    threads[i] = new Thread(Write);
                }
            }
            for(int i = 10; i < 15; i++)
            {
                threads[i] = new Thread(Read);
            }
            foreach(var t in threads)
            {
                t.Start();
            }
        }
        static void Write()
        {
            a.WaitOne();
            using (FileStream fileStream = new FileStream("test.txt", FileMode.OpenOrCreate))
            {
                byte[] bytes = System.Text.Encoding.Default.GetBytes("test");
                fileStream.Seek(0, SeekOrigin.End);
                fileStream.Write(bytes, 0, bytes.Length);
            }
            Console.WriteLine("произвёл запись информации в файл");
            Thread.Sleep(700);
            a.Release();
        }
        static void Read()
        {
            SemNoR.WaitOne();
            numberOfReaders++;
            if (numberOfReaders == 1)
            {
                a.WaitOne();
            }
            SemNoR.Release();
            using (FileStream fileStream = File.OpenRead("test.txt"))
            {
                byte[] bytes = new byte[fileStream.Length];

                fileStream.Read(bytes, 0, bytes.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(bytes);
                Console.WriteLine($"Текст из файла: {textFromFile}");
            }
            Thread.Sleep(2000);
            SemNoR.WaitOne();
            numberOfReaders--;
            if (numberOfReaders == 0)
            {
                a.Release();
            }
            SemNoR.Release();
        }
    }
}
