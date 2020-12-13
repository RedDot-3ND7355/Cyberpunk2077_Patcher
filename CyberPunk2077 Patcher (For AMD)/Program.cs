using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberPunk2077_Patcher__For_AMD_
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            EmbeddedAssembly.Load("CyberPunk2077_Patcher__For_AMD_.Resources.DotNetZip.zip", "DotNetZip.dll");
            EmbeddedAssembly.Load("CyberPunk2077_Patcher__For_AMD_.Resources.MaterialSkin.zip", "MaterialSkin.dll");
            EmbeddedAssembly.Load("CyberPunk2077_Patcher__For_AMD_.Resources.NAudio.zip", "NAudio.dll");
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.CurrentDomain_AssemblyResolve);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Preload());
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
