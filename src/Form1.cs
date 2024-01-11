using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicCompilationTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public string SomePublicField = "Hello!";
        int classNumber = 0;
        private void button1_Click(object sender, EventArgs e)
        {

            var csc = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] {
        "mscorlib.dll",
        "System.Windows.Forms.dll",
        "System.dll",
        "System.Drawing.dll",
        "System.Core.dll",
        "Microsoft.VisualBasic.dll",
        "Microsoft.CSharp.dll"});
            var results = csc.CompileAssemblyFromSource(parameters,
            @"
    using System.Windows.Forms;
    using System.Drawing;
    using Microsoft.VisualBasic;
    public class Sample 
    {
        public void DoSomething(dynamic form)
        {
            Interaction.InputBox(""Testing"", ""Test"");
            //MessageBox.Show(form.SomePublicField);
            var b = new Button();
            b.Text = form.Text;
            b.Click += (s,e)=>{MessageBox.Show(form.SomePublicField);};

            b.Location = new Point((form.ClientSize.Width - b.Width) / 2, (form.ClientSize.Height - b.Height) / 2);

            form.Controls.Add(b);
        }
    }".Replace("public class Sample", "public class class" + classNumber));
            //Check if compilation is successful, run the code
            if (!results.Errors.HasErrors)
            {
                var t = results.CompiledAssembly.GetType("class" + classNumber);
                //GetType("Sample");
                dynamic o = Activator.CreateInstance(t);
                Debug.WriteLine("Compiled with no errors");
                o.DoSomething(this);
            }
            else
            {
                var errors = string.Join(Environment.NewLine,
                    results.Errors.Cast<CompilerError>().Select(x => x.ErrorText));
                MessageBox.Show(errors);
            }
            classNumber++;
        }
    }
}
