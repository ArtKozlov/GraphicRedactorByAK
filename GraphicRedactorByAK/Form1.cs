using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace GraphicRedactorByAK
{
    public partial class Form1 : Form
    {
        public List<Figure> Objects;
        public List<Type> Types;
        private bool mouseDown = false;
        private Graphics gr;
        private Pen pen;
        private Figure SelectedFigure;

        public Form1()
        {
            InitializeComponent();
            Types = new List<Type>();
            string[] FilesInDLLDir = Directory.GetFiles("./DLL", "*.dll", SearchOption.AllDirectories);
            foreach (string file in FilesInDLLDir)
            {
                Assembly dllAssembly = Assembly.LoadFrom(file);
                foreach (var TypeInfo in dllAssembly.DefinedTypes)
                {
                    if (TypeInfo.BaseType == typeof(Figure))
                    {
                        comboBox1.Items.Add(TypeInfo.Name);
                        Types.Add(TypeInfo);
                    }
                }
            }

            Objects = new List<Figure>();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = Graphics.FromImage(pictureBox1.Image);
            pen = new Pen(colorDialog1.Color, (comboBox2.Text[0] - 48));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Objects.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
            catch
            {

            }
            gr.Clear(Color.White);
            foreach (var fig in Objects)
            {
                fig.Draw();
            }
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    try
                    {
                        string nameOfType = comboBox1.SelectedItem.ToString();
                        foreach (Type TypeOfFigure in Types)
                        {
                            if (TypeOfFigure.Name == nameOfType)
                            {
                                object fig = Activator.CreateInstance(TypeOfFigure);
                                Figure figure = (Figure)fig;
                                figure.X1 = e.X;
                                figure.X2 = e.X;
                                figure.Y1 = e.Y;
                                figure.Y2 = e.Y;
                                figure.pen.Width = pen.Width;
                                figure.pen.Color = pen.Color;
                                figure.Graph = Graphics.FromImage(pictureBox1.Image);
                                Objects.Add(figure);
                                mouseDown = true;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        comboBox1.SelectedIndex = 0;
                    }
                }
            }
            if (radioButton2.Checked)
            {
                RectangleF eRect = new RectangleF(e.X, e.Y, 1, 1);

                if (SelectedFigure != null && (SelectedFigure is IEditable))
                {
                    RectangleF r = new RectangleF(Math.Min(SelectedFigure.X1, SelectedFigure.X2), Math.Min(SelectedFigure.Y1, SelectedFigure.Y2), Math.Abs(SelectedFigure.Width), Math.Abs(SelectedFigure.Height));
                    if (r.IntersectsWith(eRect))
                    {
                        if ((e.X <= SelectedFigure.X1 + SelectedFigure.pen.Width / 2 + 2))
                        {
                            SelectedFigure.Resizing = 1;
                            mouseDown = true;
                        }
                        else
                        {
                            if ((e.X >= SelectedFigure.X2 - SelectedFigure.pen.Width / 2 - 2))
                            {
                                SelectedFigure.Resizing = 2;
                                mouseDown = true;
                            }
                            else
                            {
                                if ((e.Y <= SelectedFigure.Y1 + SelectedFigure.pen.Width / 2 + 2))
                                {
                                    SelectedFigure.Resizing = 3;
                                    mouseDown = true;
                                }
                                else
                                {
                                    if ((e.Y >= SelectedFigure.Y2 - SelectedFigure.pen.Width / 2 - 2))
                                    {
                                        SelectedFigure.Resizing = 4;
                                        mouseDown = true;
                                    }
                                    else
                                    {
                                        SelectedFigure.Resizing = 0;
                                        SelectedFigure.Moving = true;
                                        SelectedFigure.TouchedX = e.X;
                                        SelectedFigure.TouchedY = e.Y;
                                        mouseDown = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        bool AnyFigIsSelected = false;
                        foreach (var fig in Objects)
                        {
                            if (fig is ISelectable)
                            {
                                RectangleF FigRect = new RectangleF(Math.Min(fig.X1, fig.X2), Math.Min(fig.Y1, fig.Y2), Math.Abs(fig.Width), Math.Abs(fig.Height));
                                if (FigRect.IntersectsWith(eRect))
                                {
                                    listBox1.SelectedIndex = Objects.IndexOf(fig);
                                    AnyFigIsSelected = true;
                                    SelectedFigure = fig;
                                    break;
                                }
                            }
                        }
                        if (!AnyFigIsSelected)
                        {
                            listBox1.SelectedIndex = -1;
                            SelectedFigure = null;
                        }
                    }
                }
                else
                {
                    bool AnyFigIsSelected = false;
                    foreach (var fig in Objects)
                    {
                        if (fig is ISelectable)
                        {
                            RectangleF FigRect = new RectangleF(Math.Min(fig.X1, fig.X2), Math.Min(fig.Y1, fig.Y2), Math.Abs(fig.Width), Math.Abs(fig.Height));
                            if (FigRect.IntersectsWith(eRect))
                            {
                                listBox1.SelectedIndex = Objects.IndexOf(fig);
                                AnyFigIsSelected = true;
                                SelectedFigure = fig;
                                break;
                            }
                        }
                    }
                    if (!AnyFigIsSelected)
                    {
                        listBox1.SelectedIndex = -1;
                        SelectedFigure = null;
                    }
                }

            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                if (radioButton1.Checked)
                {
                    Objects[Objects.Count - 1].X2 = e.X;
                    Objects[Objects.Count - 1].Y2 = e.Y;
                }
                if (radioButton2.Checked)
                {
                    var EditFigure = SelectedFigure as IEditable;
                    var SelFigure = SelectedFigure as ISelectable;
                    if (SelectedFigure.Moving)
                    {
                        EditFigure.Move(e.X, e.Y);
                        gr.Clear(Color.White);
                        SelFigure.Select(Objects[listBox1.SelectedIndex].Graph);
                    }
                    else
                    {
                        EditFigure.Resize(e.X, e.Y);
                        gr.Clear(Color.White);
                        SelFigure.Select(Objects[listBox1.SelectedIndex].Graph);
                    }

                }

                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && mouseDown)
            {
                mouseDown = false;

                if (radioButton1.Checked)
                {
                    if ((Objects[Objects.Count - 1].Width == 0) && (Objects[Objects.Count - 1].Height == 0))
                    {
                        Objects.RemoveAt(Objects.Count - 1);
                    }
                    else
                        if (((Objects[Objects.Count - 1].Width == 0) || (Objects[Objects.Count - 1].Height == 0)) && (Objects[Objects.Count - 1].GetType().Name != "Line"))
                        {
                            Objects.RemoveAt(Objects.Count - 1);
                        }
                        else
                        {
                            listBox1.Items.Add(comboBox1.SelectedItem.ToString());
                        }
                }

                if (radioButton2.Checked)
                {
                    if (SelectedFigure != null)
                    {
                        SelectedFigure.Moving = false;
                        SelectedFigure.Resizing = 0;
                    }
                }

                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Objects.Count > 0)
            {
                foreach (var fig in Objects)
                {
                    fig.Draw(e);
                }
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                gr = Graphics.FromImage(pictureBox1.Image);
                gr.Clear(Color.White);
                pictureBox1.Refresh();
                if (SelectedFigure != null)
                {
                    SelectedFigure.Moving = false;
                    SelectedFigure.Editable = false;
                    SelectedFigure.Resizing = 0;
                }
                SelectedFigure = null;
            }
            if (radioButton2.Checked && listBox1.SelectedIndex > -1)
            {
                {
                    if (Objects[listBox1.SelectedIndex] is ISelectable)
                    {
                        Objects[listBox1.SelectedIndex].Graph.Clear(Color.White);
                        var SelectFig = Objects[listBox1.SelectedIndex] as ISelectable;
                        SelectFig.Select(Objects[listBox1.SelectedIndex].Graph);
                        SelectedFigure = Objects[listBox1.SelectedIndex];
                        colorDialog1.Color = SelectedFigure.pen.Color;
                        panel1.BackColor = colorDialog1.Color;
                        comboBox2.Text = SelectedFigure.pen.Width.ToString();
                        pictureBox1.Refresh();
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            panel1.BackColor = colorDialog1.Color;
            pen.Color = colorDialog1.Color;
            if (SelectedFigure != null && SelectedFigure is IEditable)
            {
                var PenFigure = SelectedFigure as IEditable;
                PenFigure.ChangeColor(pen.Color);
                gr.Clear(Color.White);
                var SelFigure = SelectedFigure as ISelectable;
                SelFigure.Select(Objects[listBox1.SelectedIndex].Graph);
                pictureBox1.Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Objects.Clear();
            listBox1.Items.Clear();
            gr.Clear(Color.White);
            pictureBox1.Refresh();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pen.Width = comboBox2.Text[0] - 48;
            if (SelectedFigure != null && SelectedFigure is IEditable)
            {
                var PenFigure = SelectedFigure as IEditable;
                PenFigure.ChangeTol(pen.Width);
                gr.Clear(Color.White);
                var SelFigure = SelectedFigure as ISelectable;
                SelFigure.Select(Objects[listBox1.SelectedIndex].Graph);
                pictureBox1.Refresh();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            try
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.FileName);
                writer.AutoFlush = true;
                foreach (var fig in Objects)
                {
                    JObject jsonObject = new JObject();
                    jsonObject["Type"] = fig.GetType().ToString();
                    jsonObject["X1"] = fig.X1.ToString();
                    jsonObject["Y1"] = fig.Y1.ToString();
                    jsonObject["X2"] = fig.X2.ToString();
                    jsonObject["Y2"] = fig.Y2.ToString();
                    jsonObject["pen.Color"] = fig.pen.Color.ToString();
                    jsonObject["pen.Width"] = fig.pen.Width.ToString();
                    writer.WriteLine(jsonObject.ToString());
                }
                writer.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось десериализовать");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            try
            {
                StreamReader reader = new StreamReader(openFileDialog1.FileName);
                while (!reader.EndOfStream)
                {
                    string jsonObjectString = "";
                    for (int count = 0; count < 9; count++)
                    {
                        jsonObjectString = jsonObjectString + reader.ReadLine();
                    }
                    JObject jsonItem = JObject.Parse(jsonObjectString);
                    string FigType = jsonItem.Value<string>("Type");
                    int X1 = jsonItem.Value<int>("X1");
                    int Y1 = jsonItem.Value<int>("Y1");
                    int X2 = jsonItem.Value<int>("X2");
                    int Y2 = jsonItem.Value<int>("Y2");
                    string penColor = jsonItem.Value<string>("pen.Color");
                    float penWidth = jsonItem.Value<float>("pen.Width");
                    string PenColor = penColor.Substring(7, penColor.Length - 8);

                    foreach (Type TypeOfFigure in Types)
                    {
                        if (TypeOfFigure.FullName == FigType)
                        {
                            object fig = Activator.CreateInstance(TypeOfFigure);
                            Figure figure = (Figure)fig;
                            figure.Graph = Graphics.FromImage(pictureBox1.Image);
                            figure.X1 = X1;
                            figure.Y1 = Y1;
                            figure.X2 = X2;
                            figure.Y2 = Y2;
                            figure.pen.Color = Color.FromName(PenColor);
                            figure.pen.Width = penWidth;
                            figure.Draw();
                            Objects.Add(figure);
                            listBox1.Items.Add(TypeOfFigure.Name);
                        }
                    }
                }
                reader.Dispose();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            pictureBox1.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectedFigure != null)
            {
                listBox1.SelectedIndex = -1;
                gr.Clear(Color.White);
                pictureBox1.Refresh();
            }
        }
    }
}