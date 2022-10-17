using System.IO;
using Enums;
using UnityEngine;

namespace Models
{
    public class Card
    {
        public int Number;
        public Shape Shape;
        public string Color;
        public Sprite Image;

        public Card(int num, Shape shape ,string col)
        {
            this.Number = num;
            this.Color = col;
            this.Shape = shape;
            this.Image = Resources.Load<Sprite>("GameCards/"+ $@"{shape.ToString().ToLower()}/{num}");
        }
    }
    
}