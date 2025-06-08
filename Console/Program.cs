using CodeSysCommDriver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCodesys1
{
    class Program
    {
        static void Main(string[] args)
        {
            string hostName = Environment.MachineName;
            CodeSysController controller = new CodeSysController();
            controller.UserName = "";//没有设定用户密码就写空字符串
            controller.Password = "";//没有设定用户密码就写空字符串
            bool res = controller.Connect("127.0.0.1");
            if (!res)
            {
                Console.WriteLine("无法连接到控制器");
                Console.ReadKey();
                return;
            }
            ControllerInfo info =  controller.ControllerInfo;
            //加载变量树根节点(必须执行，否则所有变量无法读取)
            IntPtr rootHandle = controller.LoadSymbolsFromPLC();
            //IntPtr rootHandle = controller.LoadSymbolsFromFile("D:\\personal data\\PLC\\Codesys program\\Test1.Device.Application.xml");
            //加载所有叶子节点
            List<ItemAddress> items = controller.LoadAllSymbolicInfo();
            List<ItemAddress> items_ = new List<ItemAddress>();
            items_.AddRange(items.GetRange(6, 252));
            string formatstring = "{0,-120}{1,-30}";
            Console.WriteLine(string.Format(formatstring, "Symbol", "Datatype"));
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
            List<string> variableNames = new List<string>();
            foreach (ItemAddress symbolic in items_)
            {
                symbolic.GetSymbol(out string symbol);
                string s = string.Format(formatstring, symbol, symbolic.Datatype);
                Console.WriteLine(s);
                variableNames.Add(symbol);
            }
            Console.WriteLine("-------------------------------------------------- End---------------------------------------------------------------");
            Console.WriteLine("按任意键开始读写变量");
            Console.ReadKey();
            //res = controller.ReadWString("Application.GVL.v12345678990", out string value0);
            //res = controller.ReadString("Application.GVL.v1234", out string value1);
            //res = controller.ReadDouble("Application.GVL.v123456", out double value2);
            //res = controller.ReadFloat("Application.GVL.v12345", out float value3);
            //res = controller.ReadShort("Application.GVL.v12", out short value4);
            //res = controller.ReadByte("Application.GVL.v1", out byte value5);
            ////
            //res = controller.WriteBool("Application.GVL.v12345678999999999999999999990.RESET", true);
            //res = controller.WriteByte("Application.GVL.v1", 48);
            //res = controller.WriteShort("Application.GVL.v12", 1590);
            //res = controller.WriteUshort("Application.GVL.v123", 123);
            //res = controller.WriteString("Application.GVL.v1234", "OKay1");
            //res = controller.WriteDouble("Application.GVL.v123456", 505.67);
            //res = controller.WriteWString("Application.GVL.v12345678990", "韩国e与1日本");
            //res = controller.WriteUshort("Application.GVL.v123456789990", 19000);
            //res = controller.WriteLong("Application.GVL.v12345678999990", 1900000);
            //res = controller.WriteUlong("Application.GVL.v123456789999990", 100001001);
            double wrtValue = 0.01;
            while (res)
            {
                wrtValue += 0.022;
                res = controller.WriteDouble("Application.GVL.var11", wrtValue);
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                res = controller.Read(variableNames.GetRange(0, 252), out List<PValue> values);
                stopwatch.Stop();
                long ms = stopwatch.ElapsedMilliseconds;
                if (res)
                {
                    string header = $"读取{values.Count}个变量耗时{ms}毫秒";
                    for (int i = 0; i < values.Count;i++)
                    {
                        PValue value = values[i];
                        string variableName = variableNames[i];
                        Console.WriteLine($"[{header}]{variableName} = {value.ToString()}");
                    }
                }
            }
            Console.ReadKey();
            controller.Disconnect();
        }
    }
}
