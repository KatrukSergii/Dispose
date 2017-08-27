using System;

namespace DisposeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FileWritter writter = new FileWritter("test.txt"))
            {
                string textToWrite = "Test text";
                writter.Write(textToWrite);
            }
        }
    }
}
