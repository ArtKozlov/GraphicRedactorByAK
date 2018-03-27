using Paint;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Line
{
    public class Line : Figure, ISelectable, IEditable
    {
        public override void Draw(PaintEventArgs e)
        {
            e.Graphics.DrawLine(pen, X1, Y1, X2, Y2);
        }

        public override void Draw()
        {
            Graph.DrawLine(pen, X1, Y1, X2, Y2);
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
                        X1 = MoveX;
                        break;
                    }
                case 2:
                    {
                        X2 = MoveX;
                        break;
                    }
                case 3:
                    {
                        Y1 = MoveY;
                        break;
                    }
                case 4:
                    {
                        Y2 = MoveY;
                        break;
                    }
            }
        }
    }
}
