using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;

namespace ApiTest.Classes
{
    class ImageCache
    {
       
        public static Dictionary<int, RebelImage> ImgCache { get; set; }

        public ImageCache()
        {

        }

        public static  RebelImage GetImage(int imageId)
        {

            if (ImgCache == null)
                ImgCache = new Dictionary<int, RebelImage>();

            //already loaded image
            if(ImgCache.ContainsKey(imageId))
            {
                return ImgCache[imageId];
            }

            //cache on the user's disk
            string path;
            //path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = @".\img";
            if(System.IO.Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, imageId.ToString() + ".idt");
            if(File.Exists(path))
            {
                string imgInner;
                imgInner = File.ReadAllText(path);
                RebelImageToTransfer tImg = JsonConvert.DeserializeObject<RebelImageToTransfer>(imgInner);
                RebelImage curImage = new RebelImage(ref tImg);
                ImgCache.Add(curImage.Id, curImage);
                return curImage;
            }

            //get from server
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://localhost:44348/api/Story/");

                var postTask = client.GetAsync("Image?ImgId=" + imageId.ToString());
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var resString = result.Content.ReadAsStringAsync().Result;
                    RebelImageToTransfer tImg = JsonConvert.DeserializeObject<RebelImageToTransfer>(resString);

                    RebelImage img = new RebelImage(ref tImg);
                    path = path.Replace(@"\\", @"\");
                    File.WriteAllText(path, resString);
                    ImgCache.Add(img.Id, img);

                    return img;
                }
                else
                {
                    //Console.WriteLine(result.StatusCode);
                    return null;
                }
            }

        }

    }

    public class RebelImage
    {

        public int Id { get; set; }
        public string Partition { get; set; }
        public string Name { get; set; }
        public Image Img { get; set; }


        public RebelImage()
        {

        }

        public RebelImage(ref RebelImageToTransfer tImg)
        {
            this.Id = tImg.Id;
            this.Partition = tImg.Partition;
            this.Name = tImg.Name;
            using (var ms = new MemoryStream(tImg.img))
            {
                this.Img = Image.FromStream(ms);
            }
        }

        public void SetImage(ref byte[] imgBytes)
        {

            var ms = new System.IO.MemoryStream(imgBytes);
            this.Img = Image.FromStream(ms);
        }

        public override string ToString()
        {
            return Name + ", size: " + Img.Size + " bytes";
        }

    }

    public class RebelImageToTransfer
    {

        public int Id { get; set; }
        public string Partition { get; set; }
        public string Name { get; set; }

        public byte[] img { get; set; }

        public RebelImageToTransfer()
        { }

        public RebelImageToTransfer(ref RebelImage rImage)
        {
            this.Id = rImage.Id;
            this.Partition = rImage.Partition;
            this.Name = rImage.Name;
            using (var ms = new MemoryStream())
            {
                rImage.Img.Save(ms, rImage.Img.RawFormat);
                this.img = ms.ToArray();
            }
        }

    }

}
