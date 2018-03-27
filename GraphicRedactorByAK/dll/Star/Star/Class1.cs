using Paint;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Star
{
    public class Star : Figure, ISelectable, IEditable
    {
        public Point[] Vertex { get; set; }

        public Star()
        {
            Vertex = new Point[10];
        }

        public override void Draw(PaintEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                Vertex[i * 2] = new Point((X1 + Width / 2 + (int)(Width * Math.Cos(-Math.PI / 2 + Math.PI * 2 * i / 5) / 2)), (Y1 + Height / 2 + (int)(Height * Math.Sin(-Math.PI / 2 + Math.PI * 2 * i / 5) / 2)));
                Vertex[i * 2 + 1] = new Point((X1 + Width / 2 + (int)(Width * Math.Cos(-Math.PI / 4 + Math.PI * 2 * i / 5) / 4)), (Y1 + Height / 2 + (int)(Height * Math.Sin(-Math.PI / 4 + Math.PI * 2 * i / 5) / 4)));
            }
            e.Graphics.DrawPolygon(pen, Vertex);
        }

        public override void Draw()
        {
            for (int i = 0; i < 5; i++)
            {
                Vertex[i * 2] = new Point((X1 + Width / 2 + (int)(Width * Math.Cos(-Math.PI / 2 + Math.PI * 2 * i / 5) / 2)), (Y1 + Height / 2 + (int)(Height * Math.Sin(-Math.PI / 2 + Math.PI * 2 * i / 5) / 2)));
                Vertex[i * 2 + 1] = new Point((X1 + Width / 2 + (int)(Width * Math.Cos(-Math.PI / 4 + Math.PI * 2 * i / 5) / 4)), (Y1 + Height / 2 + (int)(Height * Math.Sin(-Math.PI / 4 + Math.PI * 2 * i / 5) / 4)));
            }

            Graph.DrawPolygon(pen, Vertex);
        }

        void ISelectable.Select(Graphics gr)
        {
            Pen SelPen = new Pen(Color.Blue, 1);
            SelPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            gr.DrawRectangle(SelPen, Math.Min(X1, X2) - pen.Width / 2 - 1, Math.Min(Y1, Y2) - pen.Width / 2 - 1, Math.Abs(Width) + pen.Width + 2, Math.Abs(Height) + pen.Width + 2);
            SelPen.Dispose();
        }

        void IEditable.ChangeColor(Color penColor)
        {
            pen.Color = penColor;
        }

        void IEditable.ChangeTol(float penWidth)
        {
            pen.Width = penWidth;
        }

        void IEditable.Move(int MoveX, int MoveY)
        {
            X2 = X2 + MoveX - TouchedX;
            Y2 = Y2 + MoveY - TouchedY;
            X1 = X1 + MoveX - TouchedX;
            Y1 = Y1 + MoveY - TouchedY;
            TouchedX = MoveX;
            TouchedY = MoveY;
        }

        void IEditable.Resize(int MoveX, int MoveY)
        {
            switch (Resizing)
            {
                case 1:
                    {
                        if (X1 < (X2 - 1) || MoveX < X1)
                        {
                            X1 = MoveX;
                            if (X2 <= X1)
                                X1 = X2 - 1;
                        }
                        break;
                    }
                case 2:
                    {
                        if ((X1 + 1) < X2 || MoveX > X2)
                        {
                            X2 = MoveX;
                            if (X2 <= X1)
                                X2 = X1 + 1;
                        }
                        break;
                    }
                case 3:
                    {
                        if (Y1 < (Y2 - 1) || MoveY < Y1)
                        {
                            Y1 = MoveY;
                            if (Y2 <= Y1)
                                Y1 = Y2 - 1;
                        }
                        break;
                    }
                case 4:
                    {
                        if ((Y1 + 1) < Y2 || MoveY > Y2)
                        {
                            Y2 = MoveY;
                            if (Y2 <= Y1)
                                Y2 = Y1 + 1;
                        }
                        break;
                    }
            }
        }
    }
}
