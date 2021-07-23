using System;
using LibOpenCrush;
namespace Crushinator
{
    class Program
    {
        static void Main(string[] args)
        {
            var pod = new POD("C:\\Users\\Noire\\Downloads\\4x4EvoR_Patch_013\\DRIVER.POD");//args[1]);
            Console.WriteLine(pod.HeaderText);
            Console.Read();
        }
    }
}
