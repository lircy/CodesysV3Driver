using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using PValue = CodesysV3Driver.PValue;
using CodesysV3Driver;

namespace TestCodesysV3Driver.GUI
{
    /// <summary>
    /// CODESYS V3 GUI demo: 直接引用品牌驱动程序集, 连接/建树/读写/冒烟全程走
    /// 品牌原生 API (原生数据类型 VarType 直显, 不做契约映射)。
    /// 本 demo 不引用 DriverContracts —— 契约连接/类型映射的演示只属于通用测试 demo。
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly CodeSysController _native;
        private bool _busy;

        public Form1()
        {
            InitializeComponent();
            _native = new CodeSysController();
            // 传输层条目直接用枚举值 (SelectedItem 无需字符串解析), 缺省 Tcp = 改动前的行为
            cmbTransport.Items.AddRange(new object[] { TransportLayer.Tcp, TransportLayer.Udp });
            cmbTransport.SelectedIndex = 0;
            // ILogger 注入 (公共属性): 驱动内部日志进日志区
            _native.Logger = new GuiLogger(this);
            // 掉线/重连事件 (原生 EventHandler 三件套, 五家同形)。驱动缺省 Enabled=false,
            // 品牌 GUI 打开它, 拔网现场即可直接看到判死 → 退避重连 → 恢复这条链。
            _native.Reconnect.Enabled = true;
            _native.ConnectionLost += (s, ev) => AppendLog(">>> ConnectionLost fired (掉线, 重连开始前)");
            _native.Reconnecting += (s, ev) => AppendLog(">>> Reconnecting fired (第 " + _native.ReconnectAttempts + " 次尝试)");
            _native.Reconnected += (s, ev) => AppendLog(">>> Reconnected fired (重连成功)");
            SetStatus("驱动已就绪: " + _native.GetType().FullName);
        }

        // ============================== 连接 ==============================

        private void btnConnect_Click(object sender, EventArgs e)
        {
            RunBusy(() =>
            {
                bool ok = NativeConnect("native");
                InvokeIfNeeded(() =>
                {
                    btnConnect.Enabled = !ok;
                    btnDisconnect.Enabled = ok;
                    SetStatus(ok ? "已连接 " + tbHost.Text.Trim() : "连接失败 " + tbHost.Text.Trim());
                });
                return ok;
            });
        }

        /// <summary>
        /// 品牌专属连接: 凭据、传输层与目的端口一律走原生 Connect 形参 (原生不公开凭据字段,
        /// 形参序 地址 → 端口 → 传输层 → username → password, 与本家通道层 Login(username, password) 同序)。
        /// 驱动不做"探测后自动选用", 选 Udp 失败也不会退回 Tcp。端口留空 = 该传输层默认
        /// (TCP 11740 / UDP 1740); 超出该传输层合法区间由设备层抛异常, 这里只如实记录。
        /// </summary>
        private bool NativeConnect(string tag)
        {
            ConnectInputs inputs = ReadConnectInputs();
            if (inputs.Port < 0)
            {
                AppendLog(tag + " 端口不是整数: " + inputs.PortText);
                return false;
            }
            string target = inputs.Host + ", " + inputs.Transport + ", "
                + (inputs.Port > 0 ? inputs.Port.ToString() : "默认");
            try
            {
                // 口令不进日志: target 只点名账号; 凭据序与原生 Connect 形参序同 (username → password)
                bool ok = _native.Connect(inputs.Host, inputs.Port, inputs.Transport,
                                          inputs.User, inputs.Password);
                // 预期内失败不再抛, 原因从逐实例错误账取 (码 + 文本), 不再依赖 ex.Message。
                AppendLog(tag + " Connect(" + target + ", user=" + inputs.User
                    + "), IsConnected=" + ok
                    + (ok ? "" : ", errorCode=" + _native.LastErrorCode + "：" + _native.LastErrorText));
                return ok;
            }
            catch (Exception ex)
            {
                // 仅编程错误会走到这里: 失败也要点名所选的传输层与端口
                AppendLog(tag + " Connect(" + target + ") 抛出 (编程错误): " + ex.Message);
                return false;
            }
        }

        /// <summary>连接区输入快照 (Port = -1 表示端口原文不是整数)。</summary>
        private sealed class ConnectInputs
        {
            public string Host;
            public string User;
            public string Password;
            public TransportLayer Transport;
            public string PortText;
            public int Port;
        }

        /// <summary>
        /// 读连接区输入。连接跑在 RunBusy 的后台线程上: ComboBox.SelectedItem 自带跨线程
        /// 断言 (不像 TextBox.Text 那样取缓存值), 后台线程必须先封送回 UI 线程再取。
        /// </summary>
        private ConnectInputs ReadConnectInputs()
        {
            if (InvokeRequired)
                return (ConnectInputs)Invoke(new Func<ConnectInputs>(ReadConnectInputs));
            var inputs = new ConnectInputs
            {
                Host = tbHost.Text.Trim(),
                User = tbUser.Text.Trim(),
                Password = tbPwd.Text.Trim(),
                Transport = (TransportLayer)cmbTransport.SelectedItem,
                PortText = tbPort.Text.Trim(),
            };
            int port = 0;
            if (inputs.PortText.Length > 0 && !int.TryParse(inputs.PortText, out port)) port = -1;
            inputs.Port = port;
            return inputs;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            RunBusy(() =>
            {
                _native.Disconnect();
                InvokeIfNeeded(() =>
                {
                    btnConnect.Enabled = true;
                    btnDisconnect.Enabled = false;
                    SetStatus("已断开");
                });
                AppendLog("已断开");
                return true;
            });
        }

        // ============================== 符号树 ==============================

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "CODESYS 全局变量 XML (*.xml)|*.xml|所有文件 (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                tbProject.Text = openFileDialog.FileName;
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            string file = tbProject.Text.Trim();
            if (!File.Exists(file)) { AppendLog("工程文件不存在: " + file); return; }
            RunBusy(() =>
            {
                bool ok = _native.LoadSymbolsFromFile(file);
                AppendLog("LoadSymbolsFromFile(" + Path.GetFileName(file) + ") = " + ok);
                if (ok) FillTree();
                return ok;
            });
        }

        private void btnOnlineTree_Click(object sender, EventArgs e)
        {
            RunBusy(() =>
            {
                bool ok = _native.LoadSymbolsFromPLC();
                AppendLog("LoadSymbolsFromPLC() = " + ok + (ok ? "" : " (该目标可能不支持在线取树, 已加载的离线树不受影响)"));
                if (ok) FillTree();
                return ok;
            });
        }

        private void FillTree()
        {
            InvokeIfNeeded(() =>
            {
                treeSymbols.BeginUpdate();
                treeSymbols.Nodes.Clear();
                AddTreeChildren(treeSymbols.Nodes, null);
                treeSymbols.EndUpdate();
                SetStatus("符号树已加载, 顶层节点 " + treeSymbols.Nodes.Count + " 个");
            });
        }

        /// <summary>把 parentPath 的子节点挂到节点集合下; 容器节点预置占位子节点以显示展开箭头。原生 Browse, 类型直显 VarType。</summary>
        private void AddTreeChildren(TreeNodeCollection nodes, string parentPath)
        {
            List<SymbolNode> children;
            try
            {
                children = _native.Browse(parentPath ?? string.Empty);
            }
            catch (Exception ex)
            {
                AppendLog("Browse(" + (parentPath ?? "<root>") + ") 失败: " + ex.Message);
                return;
            }
            foreach (SymbolNode item in children)
            {
                TreeNode node = new TreeNode(NodeText(item));
                node.Tag = item;
                if (item.IsContainer) node.Nodes.Add(new TreeNode());   // 懒展开占位
                nodes.Add(node);
            }
        }

        private static string NodeText(SymbolNode item)
        {
            return item.Name;
        }

        private void treeSymbols_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            // 仅当子节点是占位符时才展开 (已展开过则跳过)
            if (node.Nodes.Count != 1 || node.Nodes[0].Tag != null) return;
            SymbolNode item = node.Tag as SymbolNode;
            if (item == null) return;
            treeSymbols.BeginUpdate();
            node.Nodes.Clear();
            AddTreeChildren(node.Nodes, item.SymbolPath);
            treeSymbols.EndUpdate();
        }

        private void treeSymbols_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SymbolNode item = e.Node == null ? null : e.Node.Tag as SymbolNode;
            if (item == null)
            {
                tbSymbol.Text = string.Empty;
                tbDatatype.Text = string.Empty;
                return;
            }
            tbSymbol.Text = item.SymbolPath;
            tbDatatype.Text = item.Datatype.ToString();
        }

        /// <summary>
        /// 读写前的权限提示: 树节点名不带权限, 只在操作类型与变量声明的访问权限明显不匹配时弹窗。
        /// 只提示不拦 —— 提示后照常发请求, 最终裁判是设备返回值; None 在本驱动里兼作"权限未知" 无信息可判, 不提示。
        /// </summary>
        private void WarnAccessMismatch(string symbol, bool writing)
        {
            TreeNode selected = treeSymbols.SelectedNode;
            SymbolNode item = selected == null ? null : selected.Tag as SymbolNode;
            if (item == null || item.SymbolPath != symbol || item.Access == AccessRight.None) return;
            string message = null;
            if (writing && item.Access == AccessRight.Read) message = "变量只读不可写";
            else if (!writing && item.Access == AccessRight.Write) message = "变量只写不可读";
            if (message == null) return;
            AppendLog("权限提示: " + symbol + " —— " + message + " (仍按操作请求发出)");
            MessageBox.Show(this, message + "。" + Environment.NewLine + Environment.NewLine + "变量: " + symbol,
                "权限提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ============================== 读写 ==============================

        private void btnRead_Click(object sender, EventArgs e)
        {
            string symbol = tbSymbol.Text.Trim();
            if (symbol.Length == 0) { AppendLog("请先选中符号树变量或输入符号路径"); return; }
            WarnAccessMismatch(symbol, false);
            RunBusy(() =>
            {
                if (!EnsureConnected()) return false;
                List<PValue> values;
                bool ok = _native.Read(new List<string> { symbol }, out values);
                string text = ok && values != null && values.Count > 0 ? values[0].ToStringWithType() : "<读取失败>";
                AppendLog("读 " + symbol + " = " + text + " (ok=" + ok
                    + (ok ? "" : ", errorCode=" + _native.LastErrorCode + "：" + _native.LastErrorText) + ")");
                return ok;
            });
        }

        private void btnWriteText_Click(object sender, EventArgs e)
        {
            string symbol = tbSymbol.Text.Trim();
            if (symbol.Length == 0) { AppendLog("请先选中符号树变量或输入符号路径"); return; }
            WarnAccessMismatch(symbol, true);
            string val = tbValue.Text.Trim();
            RunBusy(() =>
            {
                if (!EnsureConnected()) return false;
                bool ok = _native.WriteByText(symbol, val);
                AppendLog("文本写 " + symbol + " = \"" + val + "\" -> " + ok
                    + (ok ? "" : " (errorCode=" + _native.LastErrorCode + "：" + _native.LastErrorText + ")"));
                return ok;
            });
        }

        /// <summary>未连接则按界面参数 (含传输层与端口) 自动连接 (读写前调用); 返回是否可用。</summary>
        private bool EnsureConnected()
        {
            if (_native.IsConnected) return true;
            return NativeConnect("自动连接");
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            string symbol = tbSymbol.Text.Trim();
            if (symbol.Length == 0) { AppendLog("请先选中符号树变量或输入符号路径"); return; }
            WarnAccessMismatch(symbol, true);
            VarType type;
            if (!Enum.TryParse(tbDatatype.Text.Trim(), out type))
            {
                AppendLog("无法解析数据类型: " + tbDatatype.Text);
                return;
            }
            string text = tbValue.Text;
            RunBusy(() =>
            {
                if (!EnsureConnected()) return false;
                try
                {
                    object raw = ParseTypedText(type, text);
                    bool ok = _native.Write(symbol, PValue.Create(type, raw));
                    AppendLog("写 " + symbol + " <- " + text + " : " + ok
                        + (ok ? "" : " (errorCode=" + _native.LastErrorCode + "：" + _native.LastErrorText + ")"));
                    return ok;
                }
                catch (Exception ex)
                {
                    AppendLog("写异常: " + ex.Message);
                    return false;
                }
            });
        }

        /// <summary>按 CODESYS 原生类型解析输入文本 (STRING/WSTRING 原样返回)。</summary>
        private static object ParseTypedText(VarType type, string text)
        {
            switch (type)
            {
                case VarType.BOOL:
                    if (text == "1" || text.Equals("true", System.StringComparison.OrdinalIgnoreCase)) return true;
                    if (text == "0" || text.Equals("false", System.StringComparison.OrdinalIgnoreCase)) return false;
                    return bool.Parse(text);
                case VarType.SINT: return sbyte.Parse(text);
                case VarType.INT: return short.Parse(text);
                case VarType.DINT: return int.Parse(text);
                case VarType.LINT: return long.Parse(text);
                case VarType.USINT:
                case VarType.BYTE: return byte.Parse(text);
                case VarType.UINT:
                case VarType.WORD: return ushort.Parse(text);
                case VarType.UDINT:
                case VarType.DWORD:
                case VarType.TIME:
                case VarType.DATE:
                case VarType.DATE_AND_TIME:
                case VarType.TIME_OF_DAY: return uint.Parse(text);
                case VarType.ULINT:
                case VarType.LWORD:
                case VarType.LDATE_AND_TIME:
                case VarType.LTIME_OF_DAY: return ulong.Parse(text);
                case VarType.LTIME: return long.Parse(text);
                case VarType.REAL: return float.Parse(text);
                case VarType.LREAL: return double.Parse(text);
                case VarType.STRING:
                case VarType.WSTRING: return text;
                default: return text;
            }
        }

        // ============================== 抽样冒烟 ==============================

        private void btnSmoke_Click(object sender, EventArgs e)
        {
            RunBusy(() =>
            {
                AppendLog("---- 抽样冒烟开始 ----");
                SmokeCore();
                AppendLog("---- 抽样冒烟结束 ----");
                return true;
            });
        }

        private void SmokeCore()
        {
            // 有界广度采样 (展开预算 2000 节点): 巨型在线树全量物化会向 PLC 发
            // 海量 browse 请求打挂会话, 冒烟只用受界样本。
            List<SymbolNode> leaves = CollectByBrowse(2000, 256);
            if (leaves.Count == 0) { AppendLog("没有可用叶子, 冒烟跳过"); return; }

            var samples = leaves
                .Where(x => !x.IsContainer && x.Access != AccessRight.None &&
                            x.Datatype != VarType.STRING && x.Datatype != VarType.WSTRING &&
                            x.Datatype != VarType.UNDEFINED)
                .Take(32).ToList();
            var names = samples.Select(x => x.SymbolPath).ToList();
            List<PValue> v1, v2;
            bool ok1 = _native.Read(names, out v1);
            if ((!ok1 || v1 == null || v1.Count != names.Count) && names.Count > 0)
            {
                // 批量失败: 个别坏点拖垮整批, 逐标签剔除后重试
                var good = new List<string>();
                foreach (string n in names)
                {
                    List<PValue> single;
                    if (_native.Read(new List<string> { n }, out single) && single != null && single.Count == 1)
                        good.Add(n);
                }
                AppendLog("批量读失败, 剔除 " + (names.Count - good.Count) + " 个坏点, 重试 " + good.Count + " 个");
                names = good;
                v1 = null;
                ok1 = names.Count > 0 && _native.Read(names, out v1);
            }
            bool ok2 = _native.Read(names, out v2);
            bool consistent = ok1 && ok2 && v1.Count == v2.Count && v1.Zip(v2, (a, b) => a.ToStringWithType() == b.ToStringWithType()).All(x => x);
            AppendLog("批量读 " + names.Count + " x2: ok=" + ok1 + "/" + ok2 + " consistent=" + consistent);

            var bools = leaves.Where(x => x.Datatype == VarType.BOOL && x.Access == AccessRight.ReadWrite).ToList();
            var rnd = new Random(20261113);
            for (int i = 0; i < bools.Count; i++) { int j = rnd.Next(i, bools.Count); var tmp = bools[i]; bools[i] = bools[j]; bools[j] = tmp; }
            int pass = 0, tried = 0;
            foreach (SymbolNode b in bools)
            {
                if (pass >= 3 || tried >= 6) break;
                tried++;
                List<PValue> cur, chk;
                if (!_native.Read(new List<string> { b.SymbolPath }, out cur) || cur == null || cur.Count == 0 || !(cur[0] is ValueBool)) continue;
                bool orig = ((ValueBool)cur[0]).GetValue();
                bool w1 = _native.Write(b.SymbolPath, new ValueBool(!orig));
                _native.Read(new List<string> { b.SymbolPath }, out chk);
                bool back = chk != null && chk.Count > 0 && chk[0] is ValueBool && ((ValueBool)chk[0]).GetValue();
                bool w2 = _native.Write(b.SymbolPath, new ValueBool(orig));
                _native.Read(new List<string> { b.SymbolPath }, out chk);
                bool restored = chk != null && chk.Count > 0 && chk[0] is ValueBool && ((ValueBool)chk[0]).GetValue();
                if (w1 && back == !orig && w2 && restored == orig) pass++;
                AppendLog("  BOOL 往返 " + b.SymbolPath + ": " + orig + " -> " + back + " -> " + restored
                          + " (w1=" + w1 + ", w2=" + w2 + ")");
            }
            AppendLog("BOOL 写往返: " + pass + "/" + tried);
        }

        // 无工程文件时的有界广度采样: 展开预算用尽即停, 巨型在线树绝不全量物化。
        private List<SymbolNode> CollectByBrowse(int expandBudget, int maxLeaves)
        {
            var found = new List<SymbolNode>();
            var queue = new Queue<SymbolNode>();
            foreach (SymbolNode r in _native.Browse(string.Empty)) queue.Enqueue(r);
            int expanded = 0;
            while (queue.Count > 0 && expanded < expandBudget && found.Count < maxLeaves)
            {
                SymbolNode n = queue.Dequeue();
                if (!n.IsContainer)
                {
                    if (n.Access != AccessRight.None && n.Datatype != VarType.UNDEFINED &&
                        n.Datatype != VarType.STRING && n.Datatype != VarType.WSTRING)
                        found.Add(n);
                    continue;
                }
                expanded++;
                foreach (SymbolNode c in _native.Browse(n.SymbolPath)) queue.Enqueue(c);
            }
            return found;
        }

        // ============================== 公共辅助 ==============================

        /// <summary>后台执行驱动调用, 防止阻塞 UI; 期间防重入。</summary>
        private void RunBusy(Func<bool> action)
        {
            if (_busy) { AppendLog("正在执行其他操作, 请稍候"); return; }
            _busy = true;
            Task.Factory.StartNew(() =>
            {
                try { action(); }
                catch (Exception ex) { AppendLog("ERROR: " + ex); }
                finally { _busy = false; }
            }, TaskCreationOptions.LongRunning);
        }

        private void SetStatus(string text)
        {
            InvokeIfNeeded(() => { lbStatus.Text = text; });
        }

        public void AppendLog(string line)
        {
            InvokeIfNeeded(() =>
            {
                tbLog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + line + Environment.NewLine);
            });
        }

        private void InvokeIfNeeded(Action action)
        {
            if (IsDisposed) return;
            // BeginInvoke 而非 Invoke: 驱动的重连事件从后台线程打来, 同步 Marshal 会撞上
            // UI 线程正停在 Disconnect 的有界 Join(3000) 里, 界面白等最多 3 秒。
            try { if (InvokeRequired) BeginInvoke(action); else action(); }
            catch { /* 窗口关闭过程中的回调 */ }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { _native.Disconnect(); } catch { /* 尽力断开 */ }
        }
    }

    /// <summary>演示 ILogger 注入: 驱动内部日志输出到 GUI 日志区。</summary>
    internal class GuiLogger : ILogger
    {
        private readonly Form1 _form;

        public GuiLogger(Form1 form)
        {
            _form = form;
        }

        public IDisposable BeginScope<TState>(TState state) { return null; }
        public bool IsEnabled(LogLevel logLevel) { return true; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null) return;
            string msg = formatter(state, exception);
            if (string.IsNullOrEmpty(msg)) return;
            _form.AppendLog("[驱动] [" + logLevel + "] " + msg);
        }
    }
}
