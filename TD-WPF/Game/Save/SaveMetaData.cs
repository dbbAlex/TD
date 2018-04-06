using System;
using System.Drawing;

namespace TD_WPF.Game.Save
{
    public class SaveMetaData
    {
        public Bitmap Thumbnail { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Guid Guid { get; set; }
    }
}