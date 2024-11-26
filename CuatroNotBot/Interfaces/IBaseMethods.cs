using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace CuatroNotBot
{
    internal interface IBaseMethods
    {
        public System.Windows.Point[,] GetCoordinatesTheBlockCells(Grid parent)
        {
            int size = parent.Children.Count / 16;
            System.Windows.Point[,] coordinatesCells = new System.Windows.Point[size, size];
            for (int i = 0; i < parent.Children.Count; i++)
            {
                Grid child = (Grid)parent.Children[i];
                if (child is not null)
                {
                    int x = Grid.GetColumn(child);
                    int y = Grid.GetRow(child);
                    coordinatesCells[x, y] = child.PointToScreen(new System.Windows.Point(child.ActualHeight / 2, child.ActualWidth / 2));
                }
                else
                {
                    throw new Exception();
                }
            }
            return coordinatesCells;
        }



    }
}
