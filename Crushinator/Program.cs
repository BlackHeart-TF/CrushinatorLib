using System;
using System.IO;
using BitMiracle.LibTiff.Classic;
using LibOpenCrush;
namespace Crushinator
{
    class Program
    {
        static void Main(string[] args)
        {
            var pod = new POD("C:\\Users\\Noire\\Downloads\\4x4EvoR_Patch_013\\TRUCK.POD");//args[1]);
            var smf = pod.GetSMF("MODELS\\BLAZERZR2.SMF");
            var smf2 = pod.GetSMF("MODELS\\BLZLTTIRE16L.SMF");
            var trk = pod.FileTree["TRUCK\\CBT2.TRK"];
            var tif = pod.GetTIF("ART\\BLAZERZR2.TIF");
            string errors="";
            if (tif.RGBAImageOK(out errors))
            {
                var height = tif.GetField(TiffTag.IMAGELENGTH);
                var width = tif.GetField(TiffTag.IMAGEWIDTH);
                var array = Metadata.GetTIF_RGBA(tif);
                var bytes = new byte[array.Length * 4];
                Tiff.IntsToByteArray(array, 0, array.Length, bytes, 0);
            }
            var body = smf.Parts["Body"];
            var TRK = new TRK(trk);
            var trk2 = new TRK2(trk);

            Console.WriteLine(smf.Metadata.PartCount);
            Console.Read();
        }
    }
}
