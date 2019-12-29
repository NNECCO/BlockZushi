using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace EnterOfGame {
    public partial class Form1 : Form {

        int x = 0; //フォームの横幅
        int y = 0; //フォームの縦幅
        int nowTop = 0; //上座標の差
        int nowLeft = 0; //左座標の差
        string direction = "左上"; //ボールの動く向き
        /// <summary>
        /// ボールが動く方向の定義
        /// </summary>
        private string[] directions = new string[] { "左上", "左下", "右上", "右下" };
        /// <summary>
        /// 寿司ブロックの数
        /// </summary>
        private int mNumBlock = 15;
        /// <summary>
        /// 寿司ブロックの幅
        /// </summary>
        private int mWidthBlock = 80;
        /// <summary>
        /// 寿司ブロックの高さ
        /// </summary>
        private int mHeightBlock = 25;
        /// <summary>
        /// 寿司ブロック同士の間隔
        /// </summary>
        private int mIntervalBlock = 10;
        /// <summary>
        /// 寿司ブロックのリスト
        /// </summary>
        private List<Label> mListBlocks;

        #region##反射板に関するフィールド##
        private int locationX;
        private int locationY;
        private System.Drawing.Point mPointReflecter;
        private int mSpeedX;
        #endregion

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            nowTop = ball.Top; //現在のボールの上辺の位置を取得
            nowLeft = ball.Left; //現在のボールの左辺の位置を取得

            //フォーム画面を縦長にする(9:16)
            Width = 360;
            Height = 640;

            x = Width; //フォームの横幅を取得
            y = Height; //フォームの高さを取得
            
            //スタートボタンの位置を画面中央にする
            int xBtnStart = (Width - BtnStart.Width) / 2;
            int yBtnStart = (Height - BtnStart.Height) / 2;
            BtnStart.Location = new System.Drawing.Point(xBtnStart, yBtnStart);

            //ボールの初期出現位置
            int xBallLocation = (Width - ball.Width) / 2;
            int yBallLocation = (Height - ball.Height) * 3 / 4;
            ball.Location = new System.Drawing.Point(xBallLocation, yBallLocation);

            //ボールの初期方向
            int index = new Random().Next(0, directions.Length - 1);
            direction = directions[index];

            //寿司ブロックの初期化
            initBlocks();

            //反射板の初期化
            initRectangle();
        }

        /// <summary>
        /// ゲーム画面の表示位置をディスプレイの中央にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e) {
            StartPosition = FormStartPosition.Manual;
            int x = (Screen.PrimaryScreen.Bounds.Width - Width) / 2;
            int y = (Screen.PrimaryScreen.Bounds.Height - Height) / 2;
            DesktopLocation = new System.Drawing.Point(x, y);
        }

        /// <summary>
        /// 反射板の各パラメータを初期化する
        /// </summary>
        private void initRectangle() {
            //初期位置
            locationX = (Width - lblReflecter.Width)  / 2;
            locationY = Height * 9 / 10;
            mPointReflecter = new System.Drawing.Point(locationX, locationY);
            lblReflecter.Location = mPointReflecter;
            //左右移動の速さ
            mSpeedX = 10;
        }

        /// <summary>
        /// 寿司ブロックを初期化する
        /// </summary>
        private void initBlocks() {
            mListBlocks = new List<Label>();
            //基準位置
            int baseX = 50;
            int baseY = 50;
            //行・列数
            int row = 0, column = 1;
            for (int i = 0; i < mNumBlock; i++) {
                Label block = new Label();
                //名前
                block.Name = $"block{i}";
                //大きさ
                block.Size = new System.Drawing.Size(mWidthBlock, mHeightBlock);
                //位置
                if (Width - baseX < (baseX + mWidthBlock * column + mIntervalBlock * (column - 1))) {
                    //改行
                    column = 1;
                    row++;
                }
                int x = baseX + (mWidthBlock + mIntervalBlock) * (column - 1);
                int y = baseY + (mIntervalBlock + mHeightBlock) * row;
                block.Location = new System.Drawing.Point(x, y);
                column++;
                //画像
                block.BackColor = Color.White;
                //非表示
                block.Visible = false;
                //リストに追加
                mListBlocks.Add(block);
                //コントロールとしてフォームに追加
                Controls.Add(block);
            }
        }

        /// <summary>
        /// キー(a or ←キー と d or →キー)による反射板の左右移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyCode) {
                //左に移動
                case Keys.A:
                    //移動後の値が枠内であれば移動
                    if (locationX - mSpeedX >= 0) locationX -= mSpeedX;
                    break;
                case Keys.Left:
                    if (locationX - mSpeedX >= 0) locationX -= mSpeedX;
                    break;
                //右に移動
                case Keys.D:
                    if (locationX + lblReflecter.Width + mSpeedX <= Width) locationX += mSpeedX;
                    break;
                case Keys.Right:
                    if (locationX + lblReflecter.Width + mSpeedX <= Width) locationX += mSpeedX;
                    break;
            }
            mPointReflecter.X = locationX;
            lblReflecter.Location = mPointReflecter;
        }

        /// <summary>
        /// スタートボタンクリック時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStart_Click(object sender, EventArgs e) {
            timer1.Enabled = true; //タイマー起動
            BtnStart.Visible = false; //ボタンを見えないようにする

            //ボールを表示
            ball.Visible = true;

            //寿司ブロックを表示
            foreach (Label block in mListBlocks) block.Visible = true;

            //反射板を表示
            lblReflecter.Visible = true;
        }

        /// <summary>
        /// 2つのベクトルの内積を返す
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double dotProduct(Vector a, Vector b) {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// ボールと反射板との当たり判定結果を返す(true:衝突、false:非衝突)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private bool collisionPaddle(Vector v1, Vector v2, Vector center, double radius) {
            //反射板の方向ベクトル
            Vector paddle = v2 - v1;
            //反射板の法線
            Vector n = new Vector(paddle.Y, -paddle.X);
            //法線ベクトルを正規化(その方向の単位ベクトル化)
            n.Normalize();

            //反射板の左端からボールの中心へのベクトル
            Vector dir1 = center - v1;
            //反射板の右端からボールの中心へのベクトル
            Vector dir2 = center - v2;

            //反射板とボールの中心との距離
            double dist = Math.Abs(dotProduct(dir1, n));
            //鋭角ならば正、鈍角ならば負の値
            double angle1 = dotProduct(dir1, paddle);
            double angle2 = dotProduct(dir2, paddle);

            //どちらも鋭角かつボールと反射板との距離がボールの半径の長さ未満ならば当たり判定
            return (angle1 * angle2 < 0 && dist < radius) ? true : false;
        }

        /// <summary>
        /// ボールの描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e) {
            int zahyouX = 1; //ずらしたいx座標の数値
            int zahyouY = 3; //ずらしたいy座標の数値

            //反射板との当たり判定
            if (collisionPaddle(new Vector(lblReflecter.Location.X, lblReflecter.Location.Y), new Vector(lblReflecter.Location.X + lblReflecter.Width, lblReflecter.Location.Y), new Vector(ball.Location.X + ball.Width / 2, ball.Location.Y + ball.Width / 2), ball.Width / 2)) {
                if ("右下".Equals(direction)) direction = "右上";
                else if ("左下".Equals(direction)) direction = "左上";
            }

            if (ball.Left >= zahyouX && ball.Top >= zahyouY && direction == "左上") {
                ball.Top -= zahyouY; //ボールの左辺の座標をzahyou分ずらす　上方向へ
                ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左方向へ
            }
            else if (ball.Left >= zahyouX && ball.Bottom <= y - zahyouY && direction == "左下")
            {
                ball.Top += zahyouY; //ボールの上辺の座標をzahyou分ずらす　下方向へ
                ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左方向へ
            }
            else if (ball.Right <= x -zahyouX && ball.Bottom <= y - zahyouY && direction == "右下")
            {
                ball.Top += zahyouY; //ボールの上辺の座標をzahyou分ずらす　下方向へ
                ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右方向へ
            }
            else if (ball.Right <= x - zahyouX && ball.Top >= zahyouY && direction == "右上")
            {
                ball.Top -= zahyouY; //ボールの上辺の座標をzahyou分ずらす　上方向へ
                ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右方向へ
            }
            else if (0 < ball.Top && ball.Top < zahyouY) //もしボールの上辺の座標がForm上辺に達しそうになったら
            {
                ball.Top = 0; //ボールの上辺をFormの上辺にくっつける
                if (direction == "左上") //ボールが左上に向かっていたら
                {
                    ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左へ
                }
                else if (direction == "右上") //ボールが右上に向かっていたら
                {
                    ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右へ
                }
            }
            else if (ball.Top == 0) //ボールがFormの上辺についたら
            {
                if (direction == "左上") //ボールが左上に向かっていたら
                {
                    ball.Top += zahyouY; //ボールの左辺の座標をzahyou分ずらす　下方向へ
                    ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左方向へ
                    if (ball.Left < 0) //ボールがFormの左辺を超える場合
                    {
                        ball.Left = 0; //ボールをFormの左辺にくっつける
                    }
                    direction = "左下";
                }
                else if (direction == "右上") //ボールが右上に向かっていたら
                {
                    ball.Top += zahyouY; //ボールの左辺の座標をzahyou分ずらす　下方向へ
                    ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右方向へ
                    if (ball.Right > x) //ボールがFormの右辺を超える場合
                    {
                        ball.Left = x - ball.Size.Width; //ボールの右辺をFormの右辺にくっつける
                    }
                    direction = "右下";
                }
            }
            else if (0 < ball.Left && ball.Left < zahyouX) //もしボールの左辺の座標がForm左辺に達しそうになったら
            {
                ball.Left = 0; //ボールの左辺をFormの左辺にくっつける

                if (direction == "左下") //ボールが左下に向かっていたら
                {
                    ball.Top += zahyouY; //ボールの上辺の座標をzahyou分ずらす　下へ
                }
                else if (direction == "左上") //ボールが左上に向かっていたら
                {
                    ball.Top -= zahyouY; //ボールの上辺の座標をzahyou分ずらす　上へ
                }
            }
            else if (ball.Left == 0) //ボールがFormの左辺についたら
            {
                if (direction == "左下") //ボールが左下に向かっていたら
                {
                    ball.Top += zahyouY; //ボールの上辺の座標をzahyou分ずらす　下方向へ
                    ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右方向へ
                    if (ball.Bottom > y) //ボールがFormの底辺を超える場合
                    {
                        ball.Top = y - ball.Size.Height; //ボールの底辺をFormの底辺にくっつける
                    }
                    direction = "右下";
                }
                else if (direction == "左上") //ボールが左上に向かっていたら
                {
                    ball.Top -= zahyouY; //ボールの上辺の座標をzahyou分ずらす　上方向へ
                    ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右方向へ
                    if (ball.Top < 0) //ボールがFormの上辺を超える場合
                    {
                        ball.Top = 0; //ボールの上辺をFormの上辺にくっつける
                    }
                    direction = "右上";
                }
            }
            else if (y > ball.Bottom && ball.Bottom > y - zahyouY) //もしボールの底辺の座標がForm底辺に達しそうになったら
            {
                ball.Top = y - ball.Size.Height ; //ボールの底辺をFormの底辺にくっつける

                if (direction == "右下") //ボールが右下に向かっていたら
                {
                    ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右へ
                }
                else if (direction == "左下") //ボールが左下に向かっていたら
                {
                    ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左へ
                }
            }
            else if (ball.Bottom == y) //ボールがFormの底辺についたら
            {
                if (direction == "左下" && ball.Left - zahyouX >= 0) //ボールが左下に向かっていたら
                {
                    ball.Top -= zahyouY; //ボールの上辺の座標をzahyou分ずらす　上方向へ
                    ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左方向へ
                    if (ball.Left < 0) //ボールがFormの左辺を超える場合
                    {
                        ball.Left = 0; //ボールをFormの左辺にくっつける
                    }
                    direction = "左上";
                }
                else if (direction == "右下") //ボールが右下に向かっていたら
                {
                    ball.Top -= zahyouY; //ボールの上辺の座標をzahyou分ずらす　上方向へ
                    ball.Left += zahyouX; //ボールの左辺の座標をzahyou分ずらす　右方向へ
                    if (ball.Right > x) //ボールがFormの右辺を超える場合
                    {
                        ball.Left = x - ball.Size.Width; //ボールの右辺をFormの右辺にくっつける
                    }                                                                          
                    direction = "右上";
                }
            }
            else if (x > ball.Right && ball.Right > x - zahyouX) //もしボールの右辺の座標がForm右辺に達しそうになったら
            {
                ball.Left = x - ball.Size.Width; //ボールの右辺をFormの右辺にくっつける

                if (direction == "右上") //ボールが右上に向かっていたら
                {
                    ball.Top -= zahyouY; //ボールの上辺の座標をzahyou分ずらす　上へ
                }
                else if (direction == "右下") //ボールが右下に向かっていたら
                {
                    ball.Top += zahyouY; //ボールの上辺の座標をzahyou分ずらす　下へ
                }
            }
            else if (ball.Right == x) //ボールがFormの右辺についたら
            {
                if (direction == "右上") //ボールが右上に向かっていたら
                {
                    ball.Top -= zahyouY; //ボールの上辺の座標をzahyou分ずらす　上方向へ
                    ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左方向へ
                    if (ball.Top < 0) //ボールがFormの上辺を超える場合
                    {
                        ball.Top = 0; //ボールの上辺をFormの上辺にくっつける
                    }
                    direction = "左上";
                }
                else if (direction == "右下") //ボールが右下に向かっていたら
                {
                    ball.Top += zahyouY; //ボールの上辺の座標をzahyou分ずらす　下方向へ
                    ball.Left -= zahyouX; //ボールの左辺の座標をzahyou分ずらす　左方向へ
                    if (ball.Bottom > y) //ボールがFormの底辺を超える場合
                    {
                        ball.Top = y - ball.Size.Height; //ボールの底辺をFormの底辺にくっつける
                    }
                    direction = "左下";
                }
            }
            
        }

    }
}
