using System.Drawing;
using System.Windows.Forms;

namespace GraphicRedactorByAK
{
    public abstract class Figure
    {
        private Graphics graph;

        public bool Editable { get; set; }
        public int Resizing { get; set; }
        public bool Moving { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public Pen pen { get; set; }
        public int TouchedX { get; set; }
        public int TouchedY { get; set; }

        public Figure()
        {
            pen = new Pen(Color.Black);
            Editable = false;
            Moving = false;
            Resizing = 0;
        }

        public virtual int Width { get { return (X2 - X1); } }
        public virtual int Height { get { return (Y2 - Y1); } }

        public Graphics Graph { get { return graph; } set { graph = value; } }

        public abstract void Draw();
        public abstract void Draw(PaintEventArgs e);
    }
}
