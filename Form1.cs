using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace HTMLSyntaxAnalyser
{
    public partial class HTMLSyntaxAnalyser : Form
    {
        string html = "";
        string tags = "";
        bool fileSelected = false;

        public HTMLSyntaxAnalyser()
        {
            InitializeComponent();
            processToolStripMenuItem.Visible = false;
            listBox1.Visible = false;
        }
        private void ValidateHTMLTags(string html, List<string>outPut)
        {
            Stack<string> myStack = new Stack<string>();
            bool isValidHTML = true;
            int startInd = 0;
            int endInd = 0;
            while (html.Length != 0)
            {
                startInd = html.IndexOf('<');
                endInd = html.IndexOf('>');
                string sub = html.Substring(startInd, (endInd - startInd + 1));
                if (sub.Contains("/>"))
                {
                    if (sub.Contains(" "))
                    {
                        outPut.Add(tags + "Encountered a self closing tag : " + sub.Substring(0, sub.IndexOf(' ')) + "/>");
                    }
                    else
                    {
                        outPut.Add(tags + "Encountered a self closing tag : " + sub);
                    }
                }
                else if (sub == "<br>" || sub == "</br>" || sub == "<hr>" || sub.Contains("<meta") ||
                    sub.Contains("<!") || sub.Contains("<img") || sub.Contains("<link") ||
                    sub.Contains("<frame") || sub.Contains("<base") || sub.Contains("<col") ||
                    sub.Contains("<input") || sub.Contains("<area") || sub.Contains("<param") ||
                    sub.Contains("<isindex"))
                {
                    // Empty tags
                    if (sub.Contains(" "))
                    {
                        outPut.Add(tags + "Encountered an empty tag : " + sub.Substring(0, sub.IndexOf(' ')) + "/>");
                    }
                    else
                    {
                        outPut.Add(tags + "Encountered an empty tag : " + sub);
                    }
                }
                else if (sub.Contains("</"))
                {
                    string closeTag = sub.Remove(1, 1).ToLower();
                    string openTag = myStack.Pop().ToString();
                    if (openTag == closeTag)
                    {
                        if (tags.Length > 1)
                        {
                            tags = tags.Substring(0, tags.Length - 1);
                        }
                        else
                        {
                            tags = "";
                        }
                        outPut.Add(tags + "Encountered a closing tag : " + sub);
                    }
                    else
                    {
                        isValidHTML = false;
                        myStack.Push(openTag.ToLower());
                        outPut.Add("Wrong closing" + closeTag + ", Opening : "+openTag);
                    }
                }
                else if (sub.Contains("<!--"))
                {
                    if (sub.Contains("-->"))
                    {
                        //It is comment so do nothing.
                        outPut.Add(tags + "Encountered a Comment");
                    }
                    else
                    {
                        outPut.Add("wrong Comment");
                        isValidHTML = false;
                    }
                }
                
                else
                {
                    if (sub.Contains(" "))
                    {
                        string opening = sub.Substring(0, sub.IndexOf(' ')) + ">";
                        outPut.Add(tags+"Encountered an opening tag : " + opening);
                        myStack.Push(opening.ToLower());
                        tags += "\t";
                    }
                    else
                    {
                        outPut.Add(tags+"Encountered an opening tag : " + sub);
                        myStack.Push(sub.ToLower());
                        tags += "\t";
                    }
                }
                html = html.Substring(endInd + 1);
            }
            if (myStack.Count != 0)
            {
                outPut.Add("Stack not empty"+myStack.Count);
                isValidHTML = false;
            }
            if (isValidHTML)
            {
                htmlValidity.Text = "All HTML Tags are balanced";
            }
            else
            {
                htmlValidity.Text = "All HTML Tags are not balanced";

            }

        }
        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
           
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = "";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.AddExtension = true;
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "HTML files (*.html)|*.html";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                MessageBox.Show(filePath);
                html = File.ReadAllText(filePath);
                fileSelected = true;
                processToolStripMenuItem.Visible = true;
            }
            htmlValidity.Text = "Press Process to continue Validity.";
        }

        private void processToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void checkTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileSelected)
            {
                tags = "";
                List<string> outPut = new List<string>();
                ValidateHTMLTags(html, outPut);
                this.listBox1.DataSource = outPut;
                listBox1.Visible = true;

            }
            else
            {
                MessageBox.Show("Select a file first");
            }
        }
    }
}
