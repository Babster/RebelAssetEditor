using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace Story
{

    public class RebelScene
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int BackgroundImageId { get; set; }

        public List<SceneElement> Elements { get; set; }

        public RebelScene()
        {
            this.Id = 0;
            this.Name = "Новая сцена";
            this.Elements = new List<SceneElement>();
        }



        public override string ToString()
        {
            return this.Name + "; " + this.Id;
        }

    }

    public class RebelSceneWithSql : RebelScene
    {

        public RebelSceneWithSql() : base()
        {

        }

        public RebelSceneWithSql(ref SqlDataReader r)
        {
            LoadDataByReader(ref r);
        }

        public RebelSceneWithSql(int sceneId)
        {
            string q = @"SELECT 
                    id, 
                    name,
                    active, 
                    ISNULL(backgound_image_id, '') AS backgound_image_id
                FROM 
                    story_scenes
                WHERE
                    id = " + sceneId.ToString();

            SqlDataReader r = DataConnection.GetReader(q);
            r.Read();
            LoadDataByReader(ref r);
            r.Close();

        }

        private void LoadDataByReader(ref SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            this.Name = Convert.ToString(r["name"]);

            if (Convert.ToInt32(r["active"]) == 1)
            {
                this.Active = true;
            }
            this.BackgroundImageId = Convert.ToInt32(r["backgound_image_id"]);

            this.LoadElements();
        }

        public void SaveData()
        {

            string q;

            if (this.Id == 0)
            {
                q = @"INSERT INTO story_scenes(name) VALUES('')
                    SELECT @@Identity AS field0";

                this.Id = Convert.ToInt32(DataConnection.GetResult(q, null, 0));

            }


            q = @"UPDATE story_scenes SET
                name = @str1, 
                active = 1,
                backgound_image_id = " + this.BackgroundImageId.ToString() + @"
                WHERE
                    id = " + this.Id.ToString();


            List<string> s = new List<string>();
            s.Add(this.Name);

            DataConnection.Execute(q, s);

            SaveElements();

        }

        private void LoadElements()
        {
            this.Elements = new List<SceneElement>();

            string q = @"
                    SELECT
                        id,
                        story_scene_id,
                        element_type,
                        image_id,
                        text_russian,
                        text_english,
                        next_screen_on_end 
                    FROM
                        story_scenes_elements
                    WHERE 
                        story_scene_id = " + this.Id.ToString();

            SqlDataReader r;
            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    SceneElement curElement = new SceneElement(ref r);
                    this.Elements.Add(curElement);
                }
            }

        }

        private void SaveElements()
        {

            string q;

            if (this.Elements.Count == 0)
            {
                q = @"DELETE FROM story_scenes_elements WHERE story_scene_id = " + this.Id.ToString();
                DataConnection.Execute(q);
                return;
            }

            string ids = "";
            foreach (SceneElement curElement in this.Elements)
            {
                curElement.SceneId = this.Id;
                curElement.SaveData();

                if (ids != "")
                    ids += ",";
                ids += curElement.Id;

            }

            q = @"DELETE FROM story_scenes_elements WHERE story_scene_id = " + this.Id.ToString() + @"
                        AND id NOT IN(" + ids + ")";
            DataConnection.Execute(q);
        }

        public SceneElement AddElement()
        {

            SceneElement curElement = new SceneElement();
            curElement.Type = "Не выбран";
            curElement.TextRussian = "Текст...";
            curElement.TextEnglish = "Enter text...";
            curElement.SceneId = this.Id;
            this.Elements.Add(curElement);
            return curElement;
        }

        public static string StoryScenesQuery()
        {
            string q;
            q = $@"
                SELECT 
                    id, 
                    name,
                    active, 
                    ISNULL(backgound_image_id, '') AS backgound_image_id
                FROM 
                    story_scenes";
            return q;
        }

        public static List<RebelSceneWithSql> GetSceneList()
        {

            List<RebelSceneWithSql> tList = new List<RebelSceneWithSql>();

            string q = StoryScenesQuery();
            SqlDataReader r = DataConnection.GetReader(q);
            while(r.Read())
            {
                tList.Add(new RebelSceneWithSql(ref r));
            }
            r.Close();
            return tList;
        }

    }

    public class SceneElement
    {

        public int Id { get; set; }
        public int SceneId { get; set; }
        public string Type { get; set; }
        public int ImageId { get; set; }
        public string TextRussian { get; set; }
        public string TextEnglish { get; set; }

        public bool NextScreen { get; set; }

        public SceneElement()
        {

        }

        public SceneElement(ref SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            this.SceneId = Convert.ToInt32(r["story_scene_id"]);
            this.Type = Convert.ToString(r["element_type"]);
            this.ImageId = Convert.ToInt32(r["image_id"]);
            this.TextRussian = Convert.ToString(r["text_russian"]);
            this.TextEnglish = Convert.ToString(r["text_english"]);
            if (Convert.ToInt32(r["next_screen_on_end"]) == 1)
            {
                this.NextScreen = true;
            }
        }

        public override string ToString()
        {
            if (TextRussian != "")
            {
                return this.Type + ". " + TextRussian.Substring(0, TextRussian.Length >= 20 ? 20 : TextRussian.Length);
            }
            else
            {
                return this.Type;
            }

        }

        public void SaveData()
        {
            string q;
            if (this.Id == 0)
            {
                q = @"INSERT INTO story_scenes_elements(element_type) VALUES('')
                                SELECT @@IDENTITY As field0";
                this.Id = Convert.ToInt32(DataConnection.GetResult(q, null, 0));
            }
            q = @"UPDATE story_scenes_elements SET
                                story_scene_id = " + this.SceneId + @",
                                element_type = @str1,
                                image_id = " + this.ImageId.ToString() + @",
                                text_russian = @str2,
                                text_english = @str3,
                                next_screen_on_end = " + (this.NextScreen ? "1" : "0") + @" 
                            WHERE id = " + this.Id.ToString();
            List<string> names = new List<string>();
            names.Add(this.Type);
            names.Add(this.TextRussian);
            names.Add(this.TextEnglish);
            DataConnection.Execute(q, names);
        }

    }

    public class RebelSceneFull
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string NextObjectType { get; set; }
        public int NextObjectId { get; set; }

        public int BackgroundImageId { get; set; }

        public object BackGroundImage { get; set; }

        public List<SceneElementFull> Elements { get; set; }

        private int CurrentNumber { get; set; }


        public RebelSceneFull()
        {
            CurrentNumber = 0;
            Elements = new List<SceneElementFull>();
        }

        public RebelSceneFull(ref RebelScene tScene)
        {

            CurrentNumber = 0;

            this.Id = tScene.Id;
            this.Name = tScene.Name;
            this.Active = tScene.Active;
            this.BackgroundImageId = tScene.BackgroundImageId;
            if (this.BackgroundImageId > 0)
            {
                //BackGroundImage = ImageNamespace.ImageCache.GetImage(tScene.BackgroundImageId).Img;
            }

            int currentStage = 1;
            int numberInsideStage = 1;

            Elements = new List<SceneElementFull>();
            if (tScene.Elements.Count > 0)
            {
                for (int i = 0; i < tScene.Elements.Count; i++)
                {
                    SceneElement fromElement = tScene.Elements[i];

                    if (i > 0 && fromElement.NextScreen)
                    {
                        for (int j = i; Elements[j].StageNumber == currentStage && j >= 0; j--)
                        {
                            Elements[j].TotalElementsInStage = numberInsideStage;
                        }
                        currentStage += 1;
                        numberInsideStage = 1;
                    }


                    SceneElementFull tElement = new SceneElementFull(ref fromElement);
                    tElement.StageNumber = currentStage;
                    tElement.NumberInsideStage = numberInsideStage;
                    Elements.Add(tElement);

                    numberInsideStage += 1;

                }
            }

        }



        public SceneElementFull GetNextElement()
        {

            if (CurrentNumber == Elements.Count)
            {
                return null;
            }

            CurrentNumber += 1;
            return Elements[CurrentNumber];
        }

    }

    public class SceneElementFull
    {

        public int Id { get; set; }
        public int SceneId { get; set; }
        public string Type { get; set; }
        public int ImageId { get; set; }

        //public Texture2D img { get; set; }

        public string TextRussian { get; set; }
        public string TextEnglish { get; set; }

        public bool NextScreen { get; set; }

        public int StageNumber { get; set; }

        public int NumberInsideStage { get; set; }

        public int TotalElementsInStage { get; set; }

        public SceneElementFull()
        {

        }

        public SceneElementFull(ref SceneElement tElement)
        {
            this.Id = tElement.Id;
            this.SceneId = tElement.SceneId;
            this.Type = tElement.Type;
            this.ImageId = tElement.ImageId;
            if (this.ImageId > 0)
            {
                //this.img = ImageNamespace.ImageCache.GetImage(this.ImageId).Img;
            }

            this.TextRussian = tElement.TextRussian;
            this.TextEnglish = tElement.TextEnglish;
        }

    }

    public static class Images
    {

        public static ImageCache imgCache;
        public static void CreateImageCache()
        {
            imgCache = new ImageCache();
        }


    }

    public class ImageCache
    {
        public Dictionary<int, RebelImage> imgs;

        public ImageCache()
        {

            imgs = new Dictionary<int, RebelImage>();

            string q;
            q = "SELECT id, partition, name, file_inner FROM images";
            SqlDataReader r;
            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                while (r.Read())
                {
                    RebelImageWithSql curImage = new RebelImageWithSql(ref r);
                    imgs.Add(curImage.Id, curImage);
                }
            }
            r.Close();
        }

        public RebelImage GetImageById(int Id)
        {
            if (imgs.ContainsKey(Id))
            {
                return imgs[Id];
            }
            else
            {
                return null;
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

    public class RebelImageWithSql : RebelImage
    {

        public RebelImageWithSql()
        {

        }

        public void Savedata()
        {

            string q;

            if (Id == 0)
            {
                q = @"INSERT INTO images(name) VALUES('')
                        SELECT @@IDENTITY AS field0";
                Id = Convert.ToInt32(DataConnection.GetResult(q, null, 0));
            }

            q = @"UPDATE images SET 
                    partition = @str1,
                    name = @str2
                    <file_inner>
                    WHERE id = " + Id.ToString();

            List<string> names = new List<string>();
            names.Add(this.Partition);
            names.Add(this.Name);

            List<byte[]> imgs = null;
            if (this.Img != null)
            {
                q = q.Replace("<file_inner>", ", file_inner = @data1 ");
                imgs = new List<byte[]>();
                MemoryStream mStream = new MemoryStream();
                this.Img.Save(mStream, this.Img.RawFormat);
                imgs.Add(mStream.ToArray());
            }
            else
            {
                q = q.Replace("<file_inner>", "");
            }

            DataConnection.Execute(q, names, imgs);

        }

        public RebelImageWithSql(ref SqlDataReader r)
        {
            this.Id = Convert.ToInt32(r["id"]);
            this.Partition = Convert.ToString(r["partition"]);
            this.Name = Convert.ToString(r["name"]);
            if (r["file_inner"] != DBNull.Value)
            {
                byte[] byteImg = (byte[])r["file_inner"];
                this.SetImage(ref byteImg);
            }
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

    public static class StoryLogic
    {

        public static StringAndInt NextObject(int AdmiralId, bool doNotLog = false)
        {

            StringAndInt tObject = new StringAndInt();

            string q;
            q = @"SELECT
					admirals_progress.object_type,
					admirals_progress.object_id
				FROM
					admirals_progress 
				INNER JOIN
					(SELECT
						admiral,
						MAX(date_completed) AS max_date_completed
					FROM
						admirals_progress
					GROUP BY
						admiral) AS max_progress ON 
													max_progress.admiral = admirals_progress.admiral
													AND max_progress.max_date_completed = admirals_progress.date_completed 
				WHERE
					admirals_progress.admiral = " + AdmiralId.ToString();

            SqlDataReader r;
            r = DataConnection.GetReader(q);
            if (r.HasRows)
            {
                r.Read();
                tObject.IntValue = Convert.ToInt32(r["object_id"]);
                tObject.StrValue = Convert.ToString(r["object_type"]);
            }
            else
            {
                tObject = CommonFunctions.GetCommonValue("start_object");
            }
            r.Close();

            if (doNotLog == false)
            {
                DataConnection.Log(AdmiralId, tObject.StrValue, tObject.IntValue, "request next", "");
            }

            return tObject;

        }

        public static void RegisterStepFinished(int AdmiralId)
        {
            StringAndInt thisStep = NextObject(AdmiralId, true);

            int Id = 0;

            string q = @"SELECT id FROM admirals_progress WHERE 
			admiral = " + AdmiralId.ToString() + @"
			AND object_type = @str1
			AND object_id = " + thisStep.IntValue.ToString();

            List<string> vs = new List<string>();
            vs.Add(thisStep.StrValue);

            SqlDataReader r = DataConnection.GetReader(q, vs);
            if (r.HasRows)
            {
                r.Read();
                Id = Convert.ToInt32(r["id"]);
            }
            r.Close();

            if (Id == 0)
            {
                q = @"INSERT INTO admirals_progress(admiral, object_type, object_id) 
						VALUES(" + AdmiralId.ToString() + @", @str1, " + thisStep.IntValue.ToString() + @")
						SELECT @@IDENTITY AS field0";
                Id = Convert.ToInt32(DataConnection.GetResult(q, vs));
            }

            q = @"UPDATE admirals_progress SET date_completed = GETDATE() WHERE id = " + Id.ToString();
            DataConnection.Execute(q);

            DataConnection.Log(AdmiralId, thisStep.StrValue, thisStep.IntValue, "completed", "");

        }
    
    }

}

public static class PlayerStoryFlowHub
{
    /// <summary>
    /// Эту процедуру следует вызывать когда нужно получить текущий элемент истории, на котором
    /// находится игрок
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public static PlayerProgressElement CurrentProgressElementForPlayer(int playerId)
    {
        if (playerProgressElementDictionary == null)
        {
            playerProgressElementDictionary = new Dictionary<int, PlayerProgressElement>();
        }

        if (playerProgressElementDictionary.ContainsKey(playerId))
        {
            return playerProgressElementDictionary[playerId];
        }

        PlayerProgressElement curElement;
        string q = $@"
            SELECT
                id,
                player,
                object_type,
                object_id,
                date_completed
            FROM
                players_progress
            INNER JOIN
                (
                    SELECT
                        MAX(date_completed) AS max_date_completed
                    FROM
                        players_progress
                    WHERE
                        player = {playerId}
                ) AS max_players_progress ON 
                            max_players_progress.max_date_completed = players_progress.date_completed
            WHERE
                player = {playerId}";
        SqlDataReader r = DataConnection.GetReader(q);
        if (r.HasRows)
        {
            r.Read();
            curElement = new PlayerProgressElement(r);
            
        }
        else
        {
            curElement = new PlayerProgressElement();
            StoryObjectFlowElement tElement = NextStoryObject(curElement);
            curElement = new PlayerProgressElement(playerId, tElement);
        }
        r.Close();
        
        playerProgressElementDictionary.Add(playerId, curElement);
        return curElement;

    }

    /// <summary>
    /// Эту процедуру вызываем когда нужно зарегистрировать прогресс игрока (что он прошёл текущую сцену или что-то ещё)
    /// </summary>
    /// <param name="playerId"></param>
    public static PlayerProgressElement RegisterPlayerProgress(int playerId)
    {
        PlayerProgressElement currentElement = CurrentProgressElementForPlayer(playerId);
        currentElement.SaveData();
        PlayerProgressElement tElement = new PlayerProgressElement(playerId, NextStoryObject(currentElement));
        playerProgressElementDictionary[playerId] = new PlayerProgressElement(playerId, NextStoryObject(currentElement));
        if(!CheckAutomaticallyExecutionedEvent(playerId))
        {
            return playerProgressElementDictionary[playerId];
        }
        else
        {
            return RegisterPlayerProgress(playerId);
        }
    }

    private static bool CheckAutomaticallyExecutionedEvent(int playerId)
    {
        if(playerProgressElementDictionary[playerId].ObjectType == "event")
        {
            GameEvent.EventById(playerProgressElementDictionary[playerId].ObjectId).ExecuteEvent(playerId);
            return true;
        }

        return false;

    }

    /// <summary>
    /// Соответствует одной записи в таблице story_object_flow
    /// </summary>
    public class StoryObjectFlowElement
    {
        public int Id { get; set; }
        public string PreviousObjectType { get; set; }
        public int PreviousObjectId { get; set; }
        public string ObjectType { get; set; }
        public int ObjectId { get; set; }

        public string ObjectKey 
        { 
            get
            {
                return $"{ObjectType}:{ObjectId}";
            }
        }

        public StoryObjectFlowElement(SqlDataReader r)
        {
            Id = (int)r["id"];
            PreviousObjectType = (string)r["previous_object_type"];
            PreviousObjectId = (int)r["previous_object_id"];
            ObjectType = (string)r["object_type"];
            ObjectId = (int)r["object_id"];
        }
    }

    private static Dictionary<string, StoryObjectFlowElement> NextStoryObjectDictionary;
    private static void CreateStoryObjectDictionary()
    {
        if(NextStoryObjectDictionary != null)
        {
            return;
        }
        NextStoryObjectDictionary = new Dictionary<string, StoryObjectFlowElement>();
        List<StoryObjectFlowElement> tList = new List<StoryObjectFlowElement>();
        string q;
        q = $@"
            SELECT
                id,
                previous_object_type,
                previous_object_id,
                object_type,
                object_id
            FROM
                story_object_flow";
        SqlDataReader r = DataConnection.GetReader(q);
        while (r.Read())
        {
            tList.Add(new StoryObjectFlowElement(r));
        }
        r.Close();

        foreach(var element in tList)
        {
            if(element.PreviousObjectType == "none")
            {
                string curKey = ":0";
                NextStoryObjectDictionary.Add(curKey, element);
            }
            else
            {
                //string curKey = element.ObjectKey;
                foreach (var previousElement in tList)
                {
                    if (previousElement.ObjectId == element.PreviousObjectId && previousElement.ObjectType == element.PreviousObjectType)
                    {
                        NextStoryObjectDictionary.Add(previousElement.ObjectKey, element);
                        break;
                    }
                }
            }

        }

    }

    private static StoryObjectFlowElement NextStoryObject(PlayerProgressElement curObject)
    {
        CreateStoryObjectDictionary();
        if(curObject == null)
        {
            curObject = new PlayerProgressElement();
        }

        if (NextStoryObjectDictionary.ContainsKey(curObject.ObjectKey))
        {
            return NextStoryObjectDictionary[curObject.ObjectKey];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Соответствует одной записи в таблице players_progress
    /// </summary>
    public class PlayerProgressElement
    {
        public int Id { get; set; }
        public int Player { get; set; }
        public string ObjectType { get; set; }
        public int ObjectId { get; set; }
        public DateTime DateCompleted { get; set; }

        public string ObjectKey
        {
            get
            {
                return $"{ObjectType}:{ObjectId}";
            }
        }

        public PlayerProgressElement() 
        {
            ObjectType = "";
        }

        public PlayerProgressElement(SqlDataReader r)
        {
            Id = (int)r["id"];
            Player = (int)r["player"];
            ObjectType = (string)r["object_type"];
            ObjectId = (int)r["object_id"];
            DateCompleted = (DateTime)r["date_completed"];
        }

        /// <summary>
        /// Тут сразу создаётся объект и записывается в базу данных. Дополнительно ничего сохранять
        /// не нужно
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="storyElement"></param>
        public PlayerProgressElement(int playerId, StoryObjectFlowElement storyElement)
        {
            Player = playerId;
            ObjectType = storyElement.ObjectType;
            ObjectId = storyElement.ObjectId;
        }

        public void SaveData()
        {

            if(Id > 0)
            { 
                return; 
            }

            DateCompleted = DateTime.Now;

            string q = $@"
                INSERT INTO players_progress 
                (
                    player,
                    object_type,
                    object_id,
                    date_completed
                ) VALUES (
                    {Player},
                    @str1,
                    {ObjectId},
                    GETDATE()
                )
                SELECT @@IDENTITY AS Result";
            List<string> names = new List<string> { ObjectType };
            Id = DataConnection.GetResultInt(q, names);
        }

        public StringAndInt ToStringAndInt()
        {
            StringAndInt tElement = new StringAndInt();
            tElement.StrValue = ObjectType;
            tElement.IntValue = ObjectId;
            return tElement;
        }

        public override string ToString()
        {
            return $"{ObjectType} : {ObjectId}";
        }

    }

    private static Dictionary<int, PlayerProgressElement> playerProgressElementDictionary;

}

