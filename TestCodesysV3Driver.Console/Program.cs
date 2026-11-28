using System;
using System.Collections.Generic;
using CodesysV3Driver;

namespace TestCodesysV3Driver.Console
{
    /// <summary>
    /// CODESYS V3 驱动控制台演示：连接 → 加载符号表 → 浏览抽样 → 读抽样 → 断开。
    /// 只读演示，不写 PLC；写用法见 Main 末尾注释。
    ///
    /// 用法:
    ///   TestCodesysV3Driver.Console.exe &lt;host&gt; [port=11740] [user] [password] [symbolFile]
    /// 说明:
    ///   传输层默认 TCP；UDP 用法见 Connect 注释。给 symbolFile（CODESYS 符号导出 XML）
    ///   则离线建树；否则在线从 PLC 加载。登录形态由设备自动适配（新固件两步加密 / 旧固件单步）。
    /// </summary>
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("用法: TestCodesysV3Driver.Console.exe <host> [port=11740] [user] [password] [symbolFile]");
                System.Console.WriteLine("示例: TestCodesysV3Driver.Console.exe 192.168.0.1 11740 admin password");
                return 0;
            }
            string host = args[0];
            int port = args.Length > 1 ? int.Parse(args[1]) : 11740;
            string user = args.Length > 2 ? args[2] : "";
            string password = args.Length > 3 ? args[3] : "";
            string symbolFile = args.Length > 4 ? args[4] : null;

            var driver = new CodeSysController();
            try
            {
                // 1. 连接 PLC（默认 TCP；如需 UDP: Connect(host, 1740, TransportLayer.Udp, user, password)）
                if (!driver.Connect(host, port, TransportLayer.Tcp, user, password))
                {
                    System.Console.WriteLine("连接失败: " + host + ":" + port);
                    return 1;
                }
                System.Console.WriteLine("已连接 " + host + ":" + port + " (TCP)");

                // 2. 符号表：离线文件优先，否则在线加载
                bool loaded = symbolFile != null
                    ? driver.LoadSymbolsFromFile(symbolFile)
                    : driver.LoadSymbolsFromPLC();
                if (!loaded)
                {
                    System.Console.WriteLine("符号表加载失败（详见驱动日志）。");
                    return 1;
                }
                System.Console.WriteLine(symbolFile != null ? "离线符号表已加载: " + symbolFile : "在线符号表已加载");

                // 3. 浏览顶层符号组
                List<SymbolNode> groups = driver.Browse(null);
                System.Console.WriteLine("顶层符号组: " + groups.Count + " 个");
                for (int i = 0; i < groups.Count && i < 5; i++)
                    System.Console.WriteLine("  " + groups[i].Name + "  (" + groups[i].Datatype + ")");

                // 4. 读抽样：取前 8 个可读原子叶节点
                List<SymbolNode> leaves = driver.LoadAllSymbolicInfo();
                var names = new List<string>();
                foreach (SymbolNode leaf in leaves)
                {
                    if (names.Count >= 8) break;
                    if (leaf.IsContainer) continue;
                    if (leaf.Access == AccessRight.Write || leaf.Access == AccessRight.None) continue;
                    if (leaf.Datatype == VarType.UNDEFINED || leaf.Datatype == VarType.STRUCT ||
                        leaf.Datatype == VarType.ARRAY || leaf.Datatype == VarType.ROOT) continue;
                    names.Add(leaf.SymbolPath);
                }
                if (names.Count == 0)
                {
                    System.Console.WriteLine("无可读叶节点可供抽样。");
                    return 0;
                }
                List<PValue> values;
                if (driver.Read(names, out values))
                {
                    System.Console.WriteLine("读抽样 " + names.Count + " 个:");
                    for (int i = 0; i < names.Count; i++)
                        System.Console.WriteLine("  " + names[i] + " = " + values[i]);
                }
                else
                {
                    System.Console.WriteLine("读失败: " + driver.LastErrorText);
                }

                // 5. 写用法示例（默认不执行，避免改动用户 PLC）:
                //   driver.Write("Application.GVL_Example.Start", PValue.Create(VarType.BOOL, true));
                //   driver.Write("Application.GVL_Example.Speed", PValue.Create(VarType.REAL, 12.5));
                return 0;
            }
            finally
            {
                driver.Disconnect();   // 幂等；Dispose() 亦等价（释放链路、保留符号树）
            }
        }
    }
}