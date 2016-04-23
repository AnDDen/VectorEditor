using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace VectorEditor.Classes.Figures
{
    public class FigureFactory
    {
        public static IFigure GetFigure(Type figureType)
        {
            if (figureType.GetInterfaces().Contains(typeof(IFigure)))
            {
                return Activator.CreateInstance(figureType) as IFigure;
            }
            return null;
        }
    }
}
