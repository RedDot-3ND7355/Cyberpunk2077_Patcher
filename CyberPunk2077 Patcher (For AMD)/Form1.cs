using CyberPunk2077_Patcher__For_AMD_.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using MaterialSkin.Controls;
using MaterialSkin;
using NAudio.Wave;
using System.Diagnostics;
using System.Security.Principal;

namespace CyberPunk2077_Patcher__For_AMD_
{
    public partial class Form1 : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public Form1()
        {
            InitializeComponent();

            // Check admin rights
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                RunAsAdmin();

            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;

            // Set this to false to disable backcolor enforcing on non-materialSkin components
            // This HAS to be set before the AddFormToManage()
            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            // MaterialSkinManager properties
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.BLACK);

            // Needs a bit of a hacky mood
            StartMusic();
        }

        private void RunAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c timeout 1 && \"" + Application.ExecutablePath + "\"")
            {
                Verb = "runas",
                RedirectStandardError = false,
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
            }
            KillApp();
        }

        private void KillApp()
        {
            Process.GetCurrentProcess().Kill();
        }

        private WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
        private void StartMusic()
        {
            Mp3FileReader player = new Mp3FileReader(new MemoryStream(ExtractRessource(Resources.Song))); // from ressources
            WaveStream wavStream = WaveFormatConversionStream.CreatePcmStream(player);
            var baStream = new BlockAlignReductionStream(wavStream);
            waveOut.Init(baStream);
            waveOut.Volume = (float)0.2;
            waveOut.Play();
        }

        private byte[] ExtractRessource(byte[] zippedBuffer)
        {
            MemoryStream stream = new MemoryStream(zippedBuffer);
            using (Ionic.Zip.ZipFile z = Ionic.Zip.ZipFile.Read(stream))
            {
                MemoryStream TempArrray = new MemoryStream();
                z[0].Extract(TempArrray);
                return TempArrray.ToArray();
            }
        }

        private static readonly byte[] PatchFind =    { 0x75, 0x30, 0x33, 0xC9, 0xB8, 0x01, 0x00, 0x00, 0x00, 0x0F, 0xA2, 0x8B, 0xC8, 0xC1, 0xF9, 0x08 };
        private static readonly byte[] PatchReplace = { 0xEB, 0x30, 0x33, 0xC9, 0xB8, 0x01, 0x00, 0x00, 0x00, 0x0F, 0xA2, 0x8B, 0xC8, 0xC1, 0xF9, 0x08 };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DetectPatch(byte[] sequence, int position)
        {
            if (position + PatchFind.Length > sequence.Length) return false;
            for (int p = 0; p < PatchFind.Length; p++)
            {
                if (PatchFind[p] != sequence[position + p]) return false;
            }
            return true;
        }

        bool worked = false;
        private void PatchFile(string originalFile)
        {
            // Reset bool
            worked = false;

            // Ensure target directory exists.
            var targetDirectory = Path.GetDirectoryName(originalFile);
            if (targetDirectory == null) return;
            Directory.CreateDirectory(targetDirectory);

            // Backup target file
            if (materialCheckbox1.Checked)
            {
                if (File.Exists(originalFile + ".bak"))
                    File.Delete(originalFile + ".bak");
                File.Copy(originalFile, originalFile + ".bak");
            }

            // Read file bytes.
            byte[] fileContent = File.ReadAllBytes(originalFile);

            // Detect and patch file.
            for (int p = 0; p < fileContent.Length; p++)
            {
                if (!DetectPatch(fileContent, p)) continue;

                // Set as worked!
                worked = true;

                for (int w = 0; w < PatchFind.Length; w++)
                {
                    fileContent[p + w] = PatchReplace[w];
                }
            }

            // Save it to another location.
            File.WriteAllBytes(originalFile, fileContent);
        }

        private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (!materialSwitch1.Checked)
                waveOut.Stop();
            else
                waveOut.Play();
        }

        string FilePath = "";
        private void materialButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Search for Cyberpunk2077.exe";
            fileDialog.Filter = "exe files(*.exe)| *.exe";
            fileDialog.FileName = "Cyberpunk2077.exe";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(fileDialog.FileName) && fileDialog.FileName.EndsWith("Cyberpunk2077.exe"))
                {
                    FilePath = fileDialog.FileName;
                    materialTabControl1.SelectedIndex = (materialTabControl1.SelectedIndex + 1);
                }
            }

        }

        private void materialSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            if (!materialSwitch2.Checked)
                waveOut.Stop();
            else
                waveOut.Play();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            PatchFile(FilePath);
            if (worked)
                materialTabControl1.SelectedIndex = (materialTabControl1.SelectedIndex + 1);
            else
                materialCard1.Visible = true;
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.me/nebulahosts");
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            materialCard1.Visible = false;
        }
    }
}
