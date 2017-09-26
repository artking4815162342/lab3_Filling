using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProject1
{
    public struct BorderPoint
    {
        public Point point;
        public int preDir;   //откуда пришли

        public BorderPoint(int x, int y, int pd)
        {
            point = new Point(x, y);
            preDir = pd;
        }
    }

    public partial class Form1 : Form
    {
        Graphics g;
        int sizePen;
        Bitmap bitmap;
        Bitmap myPicture;
        Point mouse;

        public Form1()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(this.MyMouseMove);
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            pictureBox1.Image = bitmap;
            
            sizePen = trackBar1.Value;
            label1.BackColor = colorDialog1.Color;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouse = e.Location;
        }

        private void Draw(MouseEventArgs e)
        {
            g.DrawLine(new Pen(colorDialog1.Color, sizePen), mouse, e.Location);
            mouse = e.Location;
            pictureBox1.Refresh();
        }

        private void MyMouseMove(object sender, MouseEventArgs e)
        {
            if (!radioButton3.Enabled || !radioButton3.Checked)
                return;
            if (e.Button == MouseButtons.Left)
                Draw(e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            sizePen = trackBar1.Value;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                (sender as Label).BackColor = colorDialog1.Color;
        }

        private void FillHard(Point e, Color fillingColor, Color backColor)
        {
            // processing
            int x1 = e.X, x2 = e.X;
            int y1 = e.Y, y2 = e.Y;
            Color pixelColor = new Color();
            pixelColor = bitmap.GetPixel(x1, y1);

            if (pixelColor.ToArgb() != backColor.ToArgb() || pixelColor.ToArgb() == fillingColor.ToArgb())
                return;
            // если цвет пиксела равен цвету фона
            while (pixelColor.ToArgb() == backColor.ToArgb() && x1 > 0)
            {
                // цвет левого пикселя
                --x1;
                pixelColor = bitmap.GetPixel(x1, y1);
            }
            pixelColor = backColor;
            while (pixelColor.ToArgb() == backColor.ToArgb() && x2 < pictureBox1.Image.Width - 1)
            {
                // цвет правого пикселя
                ++x2;
                pixelColor = bitmap.GetPixel(x2, y2);
            }

            // рисуем линию
            g.DrawLine(new Pen(fillingColor, 1), new Point(++x1, y1), new Point(--x2, y2));
            for (int i = x1; i < x2; ++i)
            {
                if (y1 < pictureBox1.Image.Height - 1)
                    FillHard(new Point(i, y1 + 1), fillingColor, backColor);
                if (y1 > 0)
                    FillHard(new Point(i, y2 - 1), fillingColor, backColor);
            }
        }

        private void FillPicture(Point e, int x, int y, Color backColor)
        {
            // processing

           // e.X += myPicture.Width / 2;
          //  e.Y += myPicture.Height / 2;

            int x1 = e.X, x2 = e.X;
            int y1 = e.Y, y2 = e.Y;
            Color pixelColor = new Color();
            pixelColor = bitmap.GetPixel(x1, y1);

            if (pixelColor.ToArgb() != backColor.ToArgb())
                return;
            // если цвет пиксела равен цвету фона
            while (pixelColor.ToArgb() == backColor.ToArgb() && x1 > 0)
            {
                // цвет левого пикселя
                --x1;
                pixelColor = bitmap.GetPixel(x1, y1);
            }
            pixelColor = backColor;
            while (pixelColor.ToArgb() == backColor.ToArgb() && x2 < pictureBox1.Image.Width - 1)
            {
                // цвет правого пикселя
                ++x2;
                pixelColor = bitmap.GetPixel(x2, y2);
            }

            // рисуем линию
            int newX;
            newX = myPicture.Width - (mouse.X - x1) % myPicture.Width;

            if (y == -1)
                y = myPicture.Height - 1;
            ++x1;
            --x2;
            for (int i = x1; i < x2; ++i)
            {
                bitmap.SetPixel(i, y1, myPicture.GetPixel((myPicture.Width / 2 + newX++ ) % myPicture.Width, (myPicture.Height / 2 + y) % myPicture.Height));
            }

            for (int i = x1; i < x2; ++i)
            {
                if (y1 < pictureBox1.Image.Height - 1)
                    FillPicture(new Point(i, y1 + 1), mouse.X, y + 1, backColor);
                if (y1 > 0)
                    FillPicture(new Point(i, y2 - 1), mouse.X, y - 1, backColor);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //Раскевич
            if (radioButton2.Checked && radioButton2.Enabled)
            {
                FillHard(new Point(e.X, e.Y), label2.BackColor, bitmap.GetPixel(e.X, e.Y));
            }//Корниенко
            else if (radioButton1.Checked && radioButton1.Enabled)
            {
                if (pictureBox2.Image == null)
                {
                    MessageBox.Show("Не установлен шаблон.");
                    return;
                }
                FillPicture(new Point(e.X, e.Y), int.MaxValue, 0, bitmap.GetPixel(e.X, e.Y));
            }//Авилов
            else if (findToolStripMenuItem.Checked)
            {
               GetBorderPoints(bitmap, Color.Blue);
               findToolStripMenuItem.Checked = false;
            }
            pictureBox1.Refresh();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                myPicture = new Bitmap(ofd.FileName);
                pictureBox2.Image = myPicture;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton3.Checked)
            {
                pictureBox1.MouseMove -= MyMouseMove;
            }
            else
            {
                pictureBox1.MouseMove += MyMouseMove;
            }
        }

        //Функция поиска начальной точки
        private BorderPoint FindStartPoint(Bitmap sourceImage)
        {
            int x1 = mouse.X;
            int y1 = mouse.Y;
            int borderX = -1, borderY = -1;
            Color backgroundColor = sourceImage.GetPixel(mouse.X, mouse.Y);
            Color pixelColor = backgroundColor;
            while (pixelColor.ToArgb() == backgroundColor.ToArgb() && y1 > 0)
            {
                while (pixelColor.ToArgb() == backgroundColor.ToArgb() && x1 < sourceImage.Width - 1)
                {
                    // цвет левого пикселя
                    ++x1;
                    pixelColor = sourceImage.GetPixel(x1, y1);
                }
                if (borderX <= x1)
                {
                    borderY = y1;
                    borderX = x1;
                }
                y1--;
                x1 = mouse.X;
                pixelColor = sourceImage.GetPixel(x1, y1);
            }
            return new BorderPoint(borderX, borderY, 6);
        }

        //Функция формирования списка граничных точек
        private List<BorderPoint> GetBorderPoints(Bitmap sourceImage, Color c)
        {
            List<BorderPoint> border = new List<BorderPoint>();
            BorderPoint cur = FindStartPoint(sourceImage);
            border.Add(cur);
            BorderPoint start = cur;
            BorderPoint next = new BorderPoint(-1, -1, -1);
            Color borderColor = sourceImage.GetPixel(cur.point.X, cur.point.Y);

            //Будем идти против часовой стрелке и ходить изнутри области
            int lookTo = -1;
            do
            {
                lookTo = cur.preDir - 2;
                //Отнимаем в случае границ (По модулю 8)
                if (lookTo == -1)
                    lookTo = 7;
                else if (lookTo == -2)
                    lookTo = 6;
                int t = lookTo;
                do
                {
                    next.point = cur.point;
                    switch (lookTo)
                    {
                        case 0: next.point.X++; next.preDir = 0; break;
                        case 1: next.point.X++; next.point.Y--; next.preDir = 1; break;
                        case 2: next.point.Y--; next.preDir = 2; break;
                        case 3: next.point.X--; next.point.Y--; next.preDir = 3; break;
                        case 4: next.point.X--; next.preDir = 4; break;
                        case 5: next.point.X--; next.point.Y++; next.preDir = 5; break;
                        case 6: next.point.Y++; next.preDir = 6; break;
                        case 7: next.point.X++; next.point.Y++; next.preDir = 7; break;
                    }
                    //Если не нашли - останавливаемся
                    if (next.point == start.point)
                        break;
                    if (sourceImage.GetPixel(next.point.X, next.point.Y) == borderColor)
                    {
                        //Кладем в список
                        border.Add(next);
                        if (cur.preDir == 2 && next.preDir == 6 || cur.preDir == 6 && next.preDir == 2)
                            border.Add(next);
                        else if (cur.preDir == 1 && next.preDir == 7 || cur.preDir == 5 && next.preDir == 3)
                            border.Add(next);
                        else if (cur.preDir == 4 && next.preDir == 0 || cur.preDir == 4 && next.preDir == 4)
                            border.Add(next);
                        cur = next;
                        next = new BorderPoint(-1, -1, -1);
                        break;
                    }
                    lookTo = (lookTo + 1) % 8;
                } while (lookTo != t);
            } while (next.point != start.point);

            if (c != Color.Empty)
            {
                foreach (var x in border)
                    sourceImage.SetPixel(x.point.X, x.point.Y, c);
                return null;
            }
            else
            {
                border.Sort(CompareBorderPoints);

                return border;
            }
        }

        private void findToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (findToolStripMenuItem.Checked)
            {
                panel1.Enabled = false;
                pictureBox1.Cursor = Cursors.Hand;
            }
            else 
            {
                panel1.Enabled = true;
                pictureBox1.Cursor = Cursors.Default;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Refresh();
        }

        private static int CompareBorderPoints(BorderPoint a, BorderPoint b)
        {
            if (a.point.Y == b.point.Y)
                return a.point.X.CompareTo(b.point.X);
            else if (a.point.Y < b.point.Y)
                return -1;
            else return 1;
        }

        private void границаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}