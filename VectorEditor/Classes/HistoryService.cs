using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEditor.Classes
{
    public class HistoryService
    {
        public List<Image> States { get; set; }

        public int Current { get; set; }

        public Image Top
        {
            get { return States[Current]; }
        }

        public void AddState(Image img)
        {
            if (CanRedo())
            {
                States.RemoveRange(Current + 1, States.Count - Current - 1);
            }
            States.Add(img);
            Current++;
        }

        public HistoryService()
        {
            States = new List<Image>();
            Current = -1;
        }

        public bool CanUndo()
        {
            return Current > 0;
        }

        public Image Undo()
        {
            if (CanUndo())
            {
                Current--;
                return Top;
            }
            return null;
        }

        public bool CanRedo()
        {
            return (Current < States.Count - 1);
        }

        public Image Redo()
        {
            if (CanRedo())
            {
                Current++;
                return Top;
            }
            return null;
        }
    }
}
