using System;
using System.Drawing;
using System.Windows.Forms;

namespace PerMonitorDemo
{
    public partial class CustomDrawing : Form
    {
        public CustomDrawing()
        {
            offset = LogicalToDeviceUnits(OFFSET);
            InitializeComponent();
        }

        private void pictureBox1_DpiChangedAfterParent(object sender, EventArgs e)
        {
            offset = LogicalToDeviceUnits(OFFSET);
            pictureBox1.Invalidate();
        }

        private const int OFFSET = 10;
        private int offset = OFFSET;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            Rectangle r = new Rectangle(new Point(0,0), pictureBox1.Size);
            r.Inflate(-pictureBox1.Width / 4, -pictureBox1.Height / 5);
            g.DrawRectangle(Pens.Black, r);

            r.Inflate(-offset, -offset);
            int thirdY = r.Height / 3;
            int thirdX = r.Width / 3;
            int forthY = r.Height / 4;
            int[,] coordinates = new int[,]
            {
                {r.Left, r.Top + forthY},
                {r.Left, r.Bottom - forthY},
                {r.Left + thirdX/4, r.Bottom - forthY },
                {r.Left + thirdX, r.Bottom - thirdY - thirdY/3 },
                {r.Right - thirdX/3, r.Bottom},
                {r.Right, r.Bottom},
                {r.Right, r.Top},
                {r.Right - thirdX/3, r.Top},
                {r.Left + thirdX, r.Top + thirdY + thirdY/3 },
                {r.Left + thirdX/4, r.Top + forthY }
            };
            Point[] points = CreatePointsArray(coordinates);
            g.FillPolygon(Brushes.Purple, points);

            coordinates = new int[,]
            {
                {r.Left + thirdX/3, r.Top+ forthY + r.Height/3},
                {r.Left + thirdX/3, r.Bottom - forthY - r.Height/3},
                {r.Left + 2*thirdX/3, r.Top+ r.Height/2}
            };
            points = CreatePointsArray(coordinates);

            using (Brush brush = new SolidBrush(SystemColors.Control))
            {
                g.FillPolygon(brush, points);

                coordinates = new int[,]
                 {
                    {r.Right - thirdX/3 - thirdX/6, r.Bottom - forthY},
                    {r.Right - thirdX/3 - thirdX/6, r.Top + forthY},
                    {r.Left + r.Width/2, r.Top+ r.Height/2}
                 };
                points = CreatePointsArray(coordinates);

                g.FillPolygon(brush, points);

            }
        }

        private Point[] CreatePointsArray(int[,] coordinates)
        {
            Point[] points = new Point[coordinates.GetLength(0)];
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = coordinates[i, 0];
                points[i].Y = coordinates[i, 1];
            }
            return points;
        }
    }
}
