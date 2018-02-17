using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TD_WPF.Game.Handler
{
    
    public class OptionEventHandler
    {
        public string nameOfEditorOption { get; set; }
        public string nameOfGameOption { get; set; }
        GameFrame gameFrame;

        public OptionEventHandler(GameFrame gameFrame)
        {
            this.gameFrame = gameFrame;
        }

        public void HandleEditorOptionEvent(object sender, EventArgs a)
        {
            nameOfEditorOption = ((RadioButton)sender).Name;
            Console.WriteLine("nameOfEditorOption : " + nameOfEditorOption);
        }

        public void HandleGameOptionEvent(object sender, EventArgs a)
        {
            nameOfGameOption = ((RadioButton)sender).Name;
            Console.WriteLine("nameOfGameOption : " + nameOfGameOption);
        }
    }
}
