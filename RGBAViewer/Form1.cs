using BitMiracle.LibTiff.Classic;
using LibOpenCrush;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RGBAViewer
{
    public partial class Form1 : Form
    {
        POD pod;
        object file;
        public Form1()
        {
            InitializeComponent();
            suggestComboBox2.DisplayMember = "Key";
            suggestComboBox3.DisplayMember = "Key";
            suggestComboBox1.Enabled = suggestComboBox2.Enabled = suggestComboBox3.Enabled = false;


        }

        private void DataSourceChanged(object sender, EventArgs e)
        {
            var cb = sender as SuggestComboBox;
            if (cb == null) return;


            cb.Enabled = cb.DataSource != null;
        }

        public Bitmap RawToBitmap(byte[] rawdata,int width, int height)
        {

            for (int i = 0; i < rawdata.Length - 1; i += 4)
            {
                byte R = rawdata[i];
                byte G = rawdata[i + 1];
                byte B = rawdata[i + 2];
                byte A = rawdata[i + 3];

                rawdata[i] = B;
                rawdata[i + 1] = G;
                rawdata[i + 2] = R;
                rawdata[i + 3] = A;
            }



            Bitmap output = new Bitmap(width, height);
            Rectangle rect = new Rectangle(0, 0, output.Width, output.Height);
            BitmapData bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(rawdata, 0, ptr, rawdata.Length);
            output.UnlockBits(bmpData);


            return output;

        }

        public Bitmap ColorsToBitmap(LibOpenCrush.Color[] rawdata, int width, int height)
        {

            Bitmap output = new Bitmap(width, height);
            Rectangle rect = new Rectangle(0, 0, output.Width, output.Height);
            BitmapData bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            for (int i = 0; i < rawdata.Length - 1; i++)
            {
                System.Runtime.InteropServices.Marshal.Copy(rawdata[i].ToBGRA_Byte(), 0, ptr, 4);
                ptr += 4;
            }

            output.UnlockBits(bmpData);

            return output;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IEnumerable<string> podfiles = new List<string>();
            if (Directory.Exists(textBox1.Text))
            {
                podfiles = Directory
                    .EnumerateFiles(textBox1.Text, "*POD", SearchOption.TopDirectoryOnly)
                    .Select(x => x.Substring(textBox1.Text.Length).TrimStart('\\'));
            }
            if (podfiles.Any())
                suggestComboBox1.DataSource = podfiles.ToList();
            else
            {
                suggestComboBox1.DataSource = new string[] { "No POD files found" };
                suggestComboBox2.DataSource = null;
            }

        }

        private async Task LoadPod(string file)
        {
            var x = await Task.Run(() =>
            {
                try
                {
                    this.pod = new POD(file);
                    this.file = null;
                    return pod.FileTree;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            });

            suggestComboBox2.DataSource = x.Select(x=>x.Key).ToList();
            suggestComboBox2.Enabled = true;
            
        }



        private async void suggestComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text.TrimEnd('\\')+ "\\" + suggestComboBox1.Text))
                await LoadPod(textBox1.Text.TrimEnd('\\') + "\\" + suggestComboBox1.Text);
        }

        private void suggestComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { 
                LoadImage(suggestComboBox2.Text);
                if (Path.GetExtension(suggestComboBox2.Text.ToLower()) != ".smf")
                    suggestComboBox3.DataSource = null;
            }
            catch (FileNotFoundException ex)
            {
                
            }
        }
        private void LoadImage(string filename)
        {
            if (!pod.FileTree.ContainsKey(filename)) throw new FileNotFoundException("Filename not in POD",filename);

            var ext = Path.GetExtension(filename.ToLower());
            switch (ext)
            {
                case ".tif":
                    var tif = this.pod.GetTIF(filename);
                    string errors = "";
                    if (tif != null && tif.RGBAImageOK(out errors))
                    {
                        var height = tif.GetField(TiffTag.IMAGELENGTH);
                        var width = tif.GetField(TiffTag.IMAGEWIDTH);
                        var array = Metadata.GetTIF_RGBA(tif);
                        var bytes = new byte[array.Length * 4];
                        Tiff.IntsToByteArray(array, 0, array.Length, bytes, 0);
                        var bmp = RawToBitmap(bytes, width[0].ToInt(), height[0].ToInt());
                        pictureBox1.Image = bmp;
                    }
                    break;
                case ".raw":
                case ".act":
                case ".opa":
                    var img = this.pod.GetRAW(filename);
                    var rawbmp = ColorsToBitmap(img.PixelArray, img.textureSize, img.textureSize);
                    pictureBox1.Image = rawbmp;
                    break;
                case ".smf":
                    var smf = this.pod.GetSMF(filename);
                    suggestComboBox3.DataSource = smf.Parts.Values.Select(x => x.Name).ToList();
                    //suggestComboBox3.DisplayMember = "Name";
                    break;
                default:
                    pictureBox1.Image = PODViewer.Properties.Resources.unsupported_format;
                    break;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var dg = new SaveFileDialog();
            dg.FileName = Path.GetFileName(suggestComboBox2.Text);
            if (dg.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dg.FileName))
                {
                    if (MessageBox.Show(this, $"Overwrite file {dg.FileName}?", "Overwrite", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        return;
                }

                this.pod.SaveFile(suggestComboBox2.Text,dg.FileName);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var defaultpath = "C:\\Users\\Noire\\Downloads\\4x4EvoR_Patch_013\\";
            if (Directory.Exists(defaultpath))
                textBox1.Text = defaultpath;
        }

        private void suggestComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var partname = suggestComboBox3.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(partname))
                return;
            var smf = this.pod.GetSMF(suggestComboBox2.Text);
            if (smf == null)
                return;
            LoadImage("ART\\"+smf.Parts[partname].TextureFileName.ToUpper());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dlg = folderBrowserDialog1.ShowDialog();
            if(dlg == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
