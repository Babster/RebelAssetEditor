//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.IO;

//namespace TransferClasses
//{

//    public class RebelScene
//    {

//        public int Id { get; set; }

//        public string Name { get; set; }

//        public bool Active { get; set; }

//        public string NextObjectType { get; set; }
//        public int NextObjectId { get; set; }

//        public int BackgroundImageId { get; set; }

//        public List<SceneElement> Elements { get; set; }

//        public RebelScene()
//        {
//            this.NextObjectType = "Не выбран";
//            this.Id = 0;
//            this.Name = "Новая сцена";
//            this.Elements = new List<SceneElement>();
//        }

//        public RebelScene(ref SqlDataReader r)
//        {
//            this.Id = Convert.ToInt32(r["id"]);
//            this.Name = Convert.ToString(r["name"]);

//            if (Convert.ToInt32(r["active"]) == 1)
//            {
//                this.Active = true;
//            }
//            this.NextObjectType = Convert.ToString(r["next_object_type"]);
//            this.NextObjectId = Convert.ToInt32(r["next_object_id"]);
//            this.BackgroundImageId = Convert.ToInt32(r["backgound_image_id"]);

//            this.LoadElements();

//        }

//        public void SaveData()
//        {

//            string q;

//            if (this.Id == 0)
//            {
//                q = @"INSERT INTO story_scenes(name) VALUES('')
//                    SELECT @@Identity AS field0";

//                this.Id = Convert.ToInt32(DataConnection.GetResult(q, null, 0));

//            }

//            int nextObjId = this.NextObjectId;


//            q = @"UPDATE story_scenes SET
//                name = @str1, 
//                active = 1,
//                next_object_type = @str2, 
//                next_object_id = " + nextObjId.ToString() + @",
//                backgound_image_id = " + this.BackgroundImageId.ToString() + @"
//                WHERE
//                    id = " + this.Id.ToString();


//            List<string> s = new List<string>();
//            s.Add(this.Name);
//            s.Add(this.NextObjectType);

//            DataConnection.Execute(q, s);

//            SaveElements();

//        }

//        private void LoadElements()
//        {
//            this.Elements = new List<SceneElement>();

//            string q = @"
//                    SELECT
//                        id,
//                        story_scene_id,
//                        element_type,
//                        image_id,
//                        text_russian,
//                        text_english,
//                        next_screen_on_end 
//                    FROM
//                        story_scenes_elements
//                    WHERE 
//                        story_scene_id = " + this.Id.ToString();

//            SqlDataReader r;
//            r = DataConnection.GetReader(q);
//            if (r.HasRows)
//            {
//                while (r.Read())
//                {
//                    SceneElement curElement = new SceneElement(ref r);
//                    this.Elements.Add(curElement);
//                }
//            }

//        }

//        private void SaveElements()
//        {

//            string q;

//            if (this.Elements.Count == 0)
//            {
//                q = @"DELETE FROM story_scenes_elements WHERE story_scene_id = " + this.Id.ToString();
//                DataConnection.Execute(q);
//                return;
//            }

//            string ids = "";
//            foreach (SceneElement curElement in this.Elements)
//            {
//                curElement.SceneId = this.Id;
//                curElement.SaveData();

//                if (ids != "")
//                    ids += ",";
//                ids += curElement.Id;

//            }

//            q = @"DELETE FROM story_scenes_elements WHERE story_scene_id = " + this.Id.ToString() + @"
//                        AND id NOT IN(" + ids + ")";
//            DataConnection.Execute(q);
//        }

//        public SceneElement AddElement()
//        {

//            SceneElement curElement = new SceneElement();
//            curElement.Type = "Не выбран";
//            curElement.TextRussian = "Текст...";
//            curElement.TextEnglish = "Enter text...";
//            curElement.SceneId = this.Id;
//            this.Elements.Add(curElement);
//            return curElement;
//        }

//        public class SceneElement
//        {

//            public int Id { get; set; }
//            public int SceneId { get; set; }
//            public string Type { get; set; }
//            public int ImageId { get; set; }
//            public string TextRussian { get; set; }
//            public string TextEnglish { get; set; }

//            public bool NextScreen { get; set; }

//            public SceneElement()
//            {

//            }

//            public SceneElement(ref SqlDataReader r)
//            {
//                this.Id = Convert.ToInt32(r["id"]);
//                this.SceneId = Convert.ToInt32(r["story_scene_id"]);
//                this.Type = Convert.ToString(r["element_type"]);
//                this.ImageId = Convert.ToInt32(r["image_id"]);
//                this.TextRussian = Convert.ToString(r["text_russian"]);
//                this.TextEnglish = Convert.ToString(r["text_english"]);
//                if (Convert.ToInt32(r["next_screen_on_end"]) == 1)
//                {
//                    this.NextScreen = true;
//                }
//            }

//            public override string ToString()
//            {
//                if (TextRussian != "")
//                {
//                    return this.Type + ". " + TextRussian.Substring(0, TextRussian.Length >= 20 ? 20 : TextRussian.Length);
//                }
//                else
//                {
//                    return this.Type;
//                }

//            }

//            public void SaveData()
//            {
//                string q;
//                if (this.Id == 0)
//                {
//                    q = @"INSERT INTO story_scenes_elements(element_type) VALUES('')
//                                SELECT @@IDENTITY As field0";
//                    this.Id = Convert.ToInt32(DataConnection.GetResult(q, null, 0));
//                }
//                q = @"UPDATE story_scenes_elements SET
//                                story_scene_id = " + this.SceneId + @",
//                                element_type = @str1,
//                                image_id = " + this.ImageId.ToString() + @",
//                                text_russian = @str2,
//                                text_english = @str3,
//                                next_screen_on_end = " + (this.NextScreen ? "1" : "0") + @" 
//                            WHERE id = " + this.Id.ToString();
//                List<string> names = new List<string>();
//                names.Add(this.Type);
//                names.Add(this.TextRussian);
//                names.Add(this.TextEnglish);
//                DataConnection.Execute(q, names);
//            }

//        }

//    }

//    public  class ImageCache
//    {
//        public Dictionary<int, RebelImage> imgs;

//        public ImageCache()
//        {

//            imgs = new Dictionary<int, RebelImage>();

//            string q;
//            q = "SELECT id, partition, name, file_inner FROM images";
//            SqlDataReader r;
//            r = DataConnection.GetReader(q);
//            if (r.HasRows)
//            {
//                while (r.Read())
//                {
//                    RebelImage curImage = new RebelImage(ref r);
//                    imgs.Add(curImage.Id, curImage);
//                }
//            }
//            r.Close();
//        }

//    }

//    public class RebelImage
//    {

//        public int Id { get; set; }
//        public string Partition { get; set; }
//        public string Name { get; set; }
//        public Image Img { get; set; }


//        public RebelImage()
//        {

//        }

//        public RebelImage(ref SqlDataReader r)
//        {
//            this.Id = Convert.ToInt32(r["id"]);
//            this.Partition = Convert.ToString(r["partition"]);
//            this.Name = Convert.ToString(r["name"]);
//            if (r["file_inner"] != DBNull.Value)
//            {
//                byte[] byteImg = (byte[])r["file_inner"];
//                this.SetImage(ref byteImg);
//            }
//        }

//        public void SetImage(ref byte[] imgBytes)
//        {

//            var ms = new System.IO.MemoryStream(imgBytes);
//            this.Img = Image.FromStream(ms);
//        }

//        public void Savedata()
//        {

//            string q;

//            if (Id == 0)
//            {
//                q = @"INSERT INTO images(name) VALUES('')
//                        SELECT @@IDENTITY AS field0";
//                Id = Convert.ToInt32(DataConnection.GetResult(q, null, 0));
//            }

//            q = @"UPDATE images SET 
//                    partition = @str1,
//                    name = @str2
//                    <file_inner>
//                    WHERE id = " + Id.ToString();

//            List<string> names = new List<string>();
//            names.Add(this.Partition);
//            names.Add(this.Name);

//            List<byte[]> imgs = null;
//            if (this.Img != null)
//            {
//                q = q.Replace("<file_inner>", ", file_inner = @data1 ");
//                imgs = new List<byte[]>();
//                MemoryStream mStream = new MemoryStream();
//                this.Img.Save(mStream, this.Img.RawFormat);
//                imgs.Add(mStream.ToArray());
//            }
//            else
//            {
//                q = q.Replace("<file_inner>", "");
//            }

//            DataConnection.Execute(q, names, imgs);

//        }


//    }

//}