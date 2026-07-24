using CodeSysCommDriver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCodesys1
{
    class Program
    {
        static void Main(string[] args)
        {
            string hostName = Environment.MachineName;
            CodeSysController controller1 = new CodeSysController();
            CodeSysController controller2 = new CodeSysController();
            controller1.UserName = "admin";
            controller1.Password = "admin";
            controller2.UserName = "";
            controller2.Password = "";
            bool res1 = controller1.Connect("192.168.250.187");
            //bool res2 = controller2.Connect("192.168.250.187");
            if (!res1)
            {
                Console.WriteLine("实例1无法连接到控制器");
                Console.ReadKey();
                return;
            }
            //if (!res2)
            //{
            //    Console.WriteLine("实例2无法连接到控制器");
            //    Console.ReadKey();
            //    return;
            //}
            ControllerInfo info1 =  controller1.ControllerInfo;
            //ControllerInfo info2 = controller2.ControllerInfo;
            //加载变量树根节点(必须执行，否则所有变量无法读取)
            //IntPtr rootHandle1 = controller1.LoadSymbolsFromPLC();
            // IntPtr rootHandle2 = controller2.LoadSymbolsFromPLC();
            IntPtr rootHandle1 = controller1.LoadSymbolsFromFile("D:\\Desktop\\ClaudeWorkspace\\CodesysV3Driver\\恒泰PLC代码20240401.Device.Application.xml");
            //rootHandle2 = controller2.LoadSymbolsFromFile("D:\\personal data\\PLC\\Codesys program\\fxy20240224.Device.Application.xml");
            //加载所有叶子节点
            List<ItemAddress> items1 = controller1.LoadAllSymbolicInfo();
            //List<ItemAddress> items2 = controller2.LoadAllSymbolicInfo();
            List<ItemAddress> items_1 = new List<ItemAddress>();
            items_1.AddRange(items1.GetRange(0, 10));
            List<ItemAddress> items_2 = new List<ItemAddress>();
            //items_2.AddRange(items2.GetRange(0, 12));
            string formatstring = "{0,-120}{1,-30}";
            Console.WriteLine(string.Format(formatstring, "Symbol", "Datatype"));
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
            List<string> variableNames1 = new List<string>();
            List<string> variableNames2 = new List<string>();
            foreach (ItemAddress symbolic in items_1)
            {
                symbolic.GetSymbol(out string symbol);
                string s = string.Format(formatstring, symbol, symbolic.Datatype);
                Console.WriteLine($"实例1：{s}");
                variableNames1.Add(symbol);
            }
            foreach (ItemAddress symbolic in items_2)
            {
                symbolic.GetSymbol(out string symbol);
                string s = string.Format(formatstring, symbol, symbolic.Datatype);
                Console.WriteLine($"实例2：{s}");
                variableNames2.Add(symbol);
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
            // bool res = controller1.Read(variableNames1.GetRange(0, 12), out List<PValue> values1);
            // "Application.GVL.TMP_BOOL", "Application.GVL.TMP_WORD", "Application.GVL.TMP_INT_ARR[0]"
            //for(int i= 0; i < 3; i++)
            //{
            //    List<string> sr = new List<string>
            //                {
            //                    //"Application.GVL.TMP_BOOL",
            //                    //"Application.GVL.TMP_WORD",
            //                    //"Application.GVL.TMP_INT",
            //                    //"Application.GVL.TMP_DINT"
            //                    "Application.GVL.lrVar14"
            //                };
            //    bool res = false;
            //    res = controller1.Read(sr, out List<PValue> values1);
            //    for (int p = 0; p < values1.Count; p++)
            //    {
            //        PValue v = values1[p];
            //        Console.WriteLine(v.ToString());
            //    }
            //    Thread.Sleep(1000);
            //    res = controller1.WriteDouble("Application.GVL.lrVar14", 100.0 + i);
            //    //Console.WriteLine(res.ToString());
            //    //Thread.Sleep(1000);
            //    //res = controller1.WriteInt("Application.GVL.TMP_DINT", 101);
            //    //Console.WriteLine(res.ToString());
            //    //Thread.Sleep(1000);
            //    //res = controller1.Write("Application.GVL.TMP_DINT", new ValueDInt(102));
            //    //Console.WriteLine(res.ToString());
            //    //Thread.Sleep(1000);
            //    //res = controller1.WriteShort("Application.GVL.TMP_WORD", (short)i);
            //    //Console.WriteLine(res.ToString());
            //    //Thread.Sleep(1000);
            //    //res = controller1.WriteBool("Application.GVL.TMP_BOOL", i % 2 == 0);
            //    //Console.WriteLine(res.ToString());
            //    //Thread.Sleep(1000);
            //    //if (controller1.ReadShort("Application.GVL.TMP_WORD", out short value))
            //    //    Console.WriteLine(value.ToString());
            //    //Thread.Sleep(1000);
            //    //if (controller1.ReadShort("Application.GVL.TMP_INT", out short value1))
            //    //    Console.WriteLine(value1.ToString());
            //    Thread.Sleep(1000);
            //    if (controller1.ReadDouble("Application.GVL.lrVar14", out double value2))
            //        Console.WriteLine(value2.ToString());
            //    Thread.Sleep(1000);
            //}

            //res1 = controller1.WriteUshort("Application.PLC_PRG.nCounter", 123);
            //res1 = controller1.WriteInt("Application.PLC_PRG.nCounter1", 234);
            //res = controller.WriteString("Application.GVL.v1234", "OKay1");
            //res = controller.WriteDouble("Application.GVL.v123456", 505.67);
            //res = controller.WriteWString("Application.GVL.v12345678990", "韩国e与1日本");
            //res = controller.WriteUshort("Application.GVL.v123456789990", 19000);
            //res = controller.WriteLong("Application.GVL.v12345678999990", 1900000);
            //res = controller.WriteUlong("Application.GVL.v123456789999990", 100001001);
            //double wrtValue = 0.01;

            while (res1)
            {
                //wrtValue += 0.022;
                //res1 = controller1.WriteDouble("Application.GVL.var11", wrtValue);
                System.Diagnostics.Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();
                stopwatch1.Start();
                res1 = controller1.Read(variableNames1, out List<PValue> values1);
                stopwatch1.Stop();
                long ms1 = stopwatch1.ElapsedMilliseconds;
                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}]读取耗时{ms1:F3}ms");
                if (res1)
                {
                    string header = $"实例1：读取{values1.Count}个变量耗时{ms1}毫秒";
                    for (int i = 0; i < values1.Count; i++)
                    {
                        PValue value = values1[i];
                        string variableName = variableNames1[i];
                        //Console.WriteLine($"[{header}]{variableName} = {value.ToString()}");
                    }
                }
                //System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
                //stopwatch2.Start();
                //res2 = controller2.Read(variableNames2.GetRange(0, 12), out List<PValue> values2);
                //stopwatch2.Stop();
                //long ms2 = stopwatch2.ElapsedMilliseconds;
                //if (res2)
                //{
                //    string header = $"实例2：读取{values2.Count}个变量耗时{ms2}毫秒";
                //    for (int i = 0; i < values2.Count; i++)
                //    {
                //        PValue value = values2[i];
                //        string variableName = variableNames2[i];
                //        Console.WriteLine($"[{header}]{variableName} = {value.ToString()}");
                //    }
                //}
                //System.Threading.Thread.Sleep(10);
            }

            Console.ReadKey();
            controller1.Disconnect();
            //controller2.Disconnect();
        }
    }
}
