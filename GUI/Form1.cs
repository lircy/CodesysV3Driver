using CodeSysCommDriver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodesysGUIBrowser
{
    public partial class Form1 : Form
    {
        private CodeSysController controller = new CodeSysController();
        private ItemAddress _selectedItem = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void setStatus(string status)
        {
            lbStatus.Text = $"：{status}";
            lbStatus.Refresh();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string ip = tbIpAddress.Text;
            controller.UserName = tbUser.Text;
            controller.Password = tbPassword.Text;
            setStatus("connecting...");
            bool res = controller.Connect(ip);
            if (res)
            {
                setStatus("connected");
                ControllerInfo info = controller.ControllerInfo;
                lbInfo.Text = $"PLC type：{info.DeviceType}|{info.Vendor}|{info.Firmware}";
            }
            else
                setStatus("error");
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            setStatus("disconnecting...");
            controller.Disconnect();
            _selectedItem = null;
            treeView1.Nodes.Clear();
            lbInfo.Text = $"PLC type：";
            tbValue.Text = string.Empty;
            tbSymbolicDatatype.Text = string.Empty;
            tbSymbol.Text = string.Empty;
            setStatus("disconnected");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null) return;
            ItemAddress item = (ItemAddress)e.Node.Tag;
            if (item != null && item.GetSymbol(out string symbol) == 0)
            {
                _selectedItem = item;
                tbSymbol.Text = symbol;
                tbSymbolicDatatype.Text = item.Datatype.ToString();
            }
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count < 0 || e.Node.Nodes[0].Text != "Loading...") return;
            setStatus("loading...");
            e.Node.Nodes.Clear();
            ItemAddress item = (ItemAddress)e.Node.Tag;
            TreeNode tn;
            List<ItemAddress> itemInfoList = controller.Browse(item.Handle);
            foreach (ItemAddress itemAddress in itemInfoList)
            {
                tn = e.Node.Nodes.Add($"{itemAddress.Name}");
                if (itemAddress.GetChildCount(out int childcount) == 0)
                {
                    if (childcount > 0)
                        tn.Nodes.Add("Loading...");
                }
                tn.Tag = itemAddress;
                setStatus("connected");
            }
            setStatus("connected");
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (!controller.IsConnected)
            {
                MessageBox.Show("PLC未连接");
                return;
            }
            string symbol = tbSymbol.Text;
            if (!string.IsNullOrEmpty(symbol))
            {
                if (controller.Read(new List<string>() { symbol }, out List<PValue> values) && values.Count > 0)
                    tbValue.Text = values[0].GetValueAsString();
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (!controller.IsConnected)
            {
                MessageBox.Show("PLC not connected");
                return;
            }
            if (_selectedItem == null)
            {
                MessageBox.Show("No variable selected. Please select a variable from the tree.");
                return;
            }

            string symbol = tbSymbol.Text;
            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("No symbol available for the selected variable.");
                return;
            }

            string rawValue = tbValue.Text;
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                MessageBox.Show("Please enter a value in the Value field.");
                return;
            }

            // Parse the user's input into the appropriate PValue based on the variable's type
            PValue pValue = PValue.FromString(rawValue, _selectedItem.Datatype);
            if (pValue == null)
            {
                MessageBox.Show($"Cannot parse '{rawValue}' as type {_selectedItem.Datatype}.\n" +
                    "Please enter a valid value for this type.");
                return;
            }

            setStatus("writing...");
            try
            {
                bool success = controller.Write(symbol, pValue);
                if (success)
                {
                    setStatus("write successful");
                    // Auto-refresh: re-read to confirm the new value
                    btnRead_Click(sender, e);
                    setStatus("write successful, value confirmed");
                }
                else
                {
                    setStatus("write failed");
                    MessageBox.Show("Write operation failed. The PLC may have rejected the value.");
                }
            }
            catch (Exception ex)
            {
                setStatus("write error");
                MessageBox.Show($"Write error: {ex.Message}");
            }
        }

        private void btn_LoadXml_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择 XML 文件";
                openFileDialog.Filter = "XML 文件|*.xml";
                openFileDialog.FilterIndex = 1;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    treeView1.Nodes.Clear();
                    setStatus("loading...");
                    IntPtr rootHandle = controller.LoadSymbolsFromFile(openFileDialog.FileName);
                    TreeNode tn;
                    List<ItemAddress> itemInfoList = controller.Browse(rootHandle);
                    foreach (ItemAddress itemAddress in itemInfoList)
                    {
                        tn = treeView1.Nodes.Add(itemAddress.Name);
                        if (itemAddress.GetChildCount(out int childcount) == 0)
                        {
                            if (childcount > 0)
                                tn.Nodes.Add("Loading...");
                        }
                        tn.Tag = itemAddress;
                    }
                    setStatus("load finished");
                }
            }
        }

        private void btn_OnlineGetVars_Click(object sender, EventArgs e)
        {
            if (!controller.IsConnected)
            {
                MessageBox.Show("PLC未连接");
                return;
            }
            treeView1.Nodes.Clear();
            setStatus("loading...");
            IntPtr rootHandle = controller.LoadSymbolsFromPLC();
            TreeNode tn;
            List<ItemAddress> itemInfoList = controller.Browse(rootHandle);
            foreach (ItemAddress itemAddress in itemInfoList)
            {
                tn = treeView1.Nodes.Add(itemAddress.Name);
                if (itemAddress.GetChildCount(out int childcount) == 0)
                {
                    if (childcount > 0)
                        tn.Nodes.Add("Loading...");
                }
                tn.Tag = itemAddress;
            }
            setStatus("load finished");
        }
    }
}
