using System.Drawing;

namespace GraphicRedactorByAK
{
    public interface IEditable
    {
        void Move(int x1, int y1);
        void Resize(int x1, int y1);
        void ChangeColor(Color penColor);
        void ChangeTol(float penWidth);
    }
}
