using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest.Classes
{
    public class RebelScene
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string NextObjectType { get; set; }
        public int NextObjectId { get; set; }

        public int BackgroundImageId { get; set; }

        public List<SceneElement> Elements { get; set; }

        public RebelScene()
        {
            this.NextObjectType = "Не выбран";
            this.Id = 0;
            this.Name = "Новая сцена";
            this.Elements = new List<SceneElement>();
        }

        public override string ToString()
        {
            return this.Name + "; " + this.Id;
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

    }

    public class RebelSceneFull
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string NextObjectType { get; set; }
        public int NextObjectId { get; set; }

        public int BackgroundImageId { get; set; }

        //public Texture2D BackGroundImage { get; set; }

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
            this.NextObjectType = tScene.NextObjectType;
            this.NextObjectId = tScene.NextObjectId;
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

                    SceneElementFull tElement = new SceneElementFull(ref fromElement);
                    tElement.StageNumber = currentStage;
                    tElement.NumberInsideStage = numberInsideStage;
                    Elements.Add(tElement);

                    if (i > 0 && fromElement.NextScreen)
                    {
                        for (int j = i; j >= 0; j--)
                        {
                            if (Elements[j].StageNumber == currentStage)
                            {
                                Elements[j].TotalElementsInStage = numberInsideStage;
                            }
                            else
                            {
                                break;
                            }
                        }
                        currentStage += 1;
                        numberInsideStage = 1;
                    }
                    else
                    {
                        numberInsideStage += 1;
                    }
                    

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

}
