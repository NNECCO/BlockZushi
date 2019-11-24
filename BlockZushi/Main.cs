using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BlockZushi {

    public partial class Main : Form {

        private Rectangle mRect;
        private int mSxRect, mSyRect, mWidthRect, mHeightRect;
        private int mSpeedX;

        private void Main_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case 'a':
                    if (mRect.X - mSpeedX >= 0) mRect.X -= mSpeedX;
                    break;
                case 'd':
                    if (mRect.X + mSpeedX <= Width - (mWidthRect*1.3)) mRect.X += mSpeedX;
                    break;
            }
        }

        public Main() {
            InitializeComponent();
            mSxRect = Width / 2;
            mSyRect = Height * 4 / 5;
            mWidthRect = 50;
            mHeightRect = 10;
            mRect = new Rectangle(mSxRect-mWidthRect/2, mSyRect, mWidthRect, mHeightRect);
            mSpeedX = 5;
        }

        private void Main_Paint(object sender, PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.OrangeRed, mRect);
            Invalidate();
        }

    }
}
