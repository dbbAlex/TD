using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using TD_WPF.Game.Spielobjekte;

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
            nameOfEditorOption = ((Button)sender).Name;

            this.gameFrame.removeRecatngles();
            this.gameFrame.possibleHint.Clear();
            this.gameFrame.showHints();
            this.gameFrame.Map.Focus();
        }

        public void HandleGameOptionEvent(object sender, EventArgs a)
        {
            nameOfGameOption = ((Button)sender).Name;

            this.gameFrame.removeRecatngles();
            this.gameFrame.possibleHint.Clear();
            this.gameFrame.showHints();
        }
    }
}
