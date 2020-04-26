using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Checker
{
    public partial class Form1 : Form
    {
        private System.Runtime.Serialization.NetDataContractSerializer serializer = new System.Runtime.Serialization.NetDataContractSerializer();
        private ClassCheckerNode rootNode;
        public Form1()
        {
            this.rootNode = new ClassCheckerNode(".*", 0, "Root");

            InitializeComponent();

            var root = this.treeView1.Nodes.Add(rootNode.ToString());
            root.Tag = rootNode;
        }

        private void addLeafToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            if (!(treeView1.SelectedNode.Tag is ClassCheckerNode)) return;
            try
            {
                while (true)
                {
                    using (ConditionInputForm f = new ConditionInputForm())
                    {
                        f.ShowDialog();
                        if (string.IsNullOrEmpty(f.textBox_cond.Text + f.textBox_name.Text + f.textBox_unit)) break;
                        ClassCheckerNode node = (ClassCheckerNode)treeView1.SelectedNode.Tag;
                        ClassCheckerLeaf leaf = new ClassCheckerLeaf(f.textBox_cond.Text, float.Parse(f.textBox_unit.Text), f.textBox_name.Text);
                        treeView1.SelectedNode.Nodes.Add(leaf.ToString()).Tag = leaf;
                        node.Children.Add(leaf);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            if (!(treeView1.SelectedNode.Tag is ClassCheckerNode)) return;
            try
            {
                using (ConditionInputForm f = new ConditionInputForm())
                {
                    f.ShowDialog();
                    ClassCheckerNode node = (ClassCheckerNode)treeView1.SelectedNode.Tag;
                    ClassCheckerNode newnode = new ClassCheckerNode(f.textBox_cond.Text, float.Parse(f.textBox_unit.Text), f.textBox_name.Text);
                    treeView1.SelectedNode.Nodes.Add(newnode.ToString()).Tag = newnode;
                    node.Children.Add(newnode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rootNode.Clear();
            using (System.IO.FileStream stream = new System.IO.FileStream("grad.xml", System.IO.FileMode.Create, System.IO.FileAccess.Write))
                serializer.Serialize(stream, rootNode);
            MessageBox.Show("OK");
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (System.IO.FileStream stream = new System.IO.FileStream("grad.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                ClassCheckerNode root = (ClassCheckerNode)serializer.Deserialize(stream);

                treeView1.Nodes[0].Nodes.Clear();

                this.rootNode = root;
                AddNodes(root, treeView1.Nodes[0]);
                treeView1.Nodes[0].Text = rootNode.ToString();
            }
            MessageBox.Show("OK");
        }

        private void AddNodes(ClassCheckerNode parentC, TreeNode parentT)
        {
            foreach (var item in parentC.Children)
            {
                switch (item)
                {
                    case ClassCheckerLeaf leaf:
                        TreeNode tnode = parentT.Nodes.Add(leaf.ToString());
                        tnode.Tag = leaf;
                        foreach(var cls in leaf.Classes)
                        {
                            tnode.Nodes.Add(cls.ToString()).Tag = cls;
                        }
                        break;

                    case ClassCheckerNode node:
                        AddNodes(node, parentT.Nodes.Add(node.ToString()));
                        break;
                }
            }
            parentT.Tag = parentC;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parent = treeView1.SelectedNode.Parent;
            if (parent == null) return;
            ((ClassCheckerNode)parent.Tag).Children.Remove((ClassChecker)treeView1.SelectedNode.Tag);
            treeView1.SelectedNode.Remove();
        }

        private void goToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(textBox1.Text))
                using (CsvParser parser = new CsvParser(reader))
                {
                    var header = parser.ReadRecord();
                    if (header.Length != 11 || header[2] != "科目番号" || header[3] != "科目名 " || header[4] != "単位数" || header[7] != "総合評価")
                        return;
                    var ac = parser.ReadRecordToEnd()
                        .Where(a => a[7] != "D");
                        var achived = ac.Select(a => new ClassDefinition { Id = a[2], Unit = float.Parse(a[4].Trim()), Name = a[3] });
                    rootNode.Clear();
                    List<ClassDefinition> missed = new List<ClassDefinition>();
                    foreach (var item in achived)
                    {
                        if(!rootNode.AddClass(item)) missed.Add(item);
                    }
                    treeView1.Nodes[0].Nodes.Clear();
                    AddNodes(rootNode, treeView1.Nodes[0]);
                    treeView1.Nodes[0].Text = rootNode.ToString();
                    //treeView1.ExpandAll();
                    MessageBox.Show(string.Join(Environment.NewLine, missed.Select(a => a.ToString())), "Dead");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            if (!(treeView1.SelectedNode.Tag is ClassChecker)) return;
            try
            {
                using (ConditionInputForm f = new ConditionInputForm())
                {
                    ClassChecker node = (ClassChecker)treeView1.SelectedNode.Tag;
                    f.textBox_cond.Text = node.TakeCondition;
                    f.textBox_unit.Text = node.RequiredAmount.ToString();
                    f.textBox_name.Text = node.Name;
                    f.ShowDialog();
                    node.TakeCondition = f.textBox_cond.Text;
                    node.RequiredAmount = float.Parse(f.textBox_unit.Text);
                    node.Name = f.textBox_name.Text;
                    treeView1.SelectedNode.Text = node.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
