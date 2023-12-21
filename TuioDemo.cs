using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using TUIO;
using System.Net.Sockets;
using System.Text;

namespace TuioDemo
{
    public class TuioDemo : Form, TuioListener
    {
        public class soc
        {

            public float x = 0;
            public float y = 0;
            int port;
            NetworkStream stream;
            byte[] sendData;
            TcpClient client;
            String host;
            public soc(int port, string message, string host)
            {
                this.port = port;
                this.host = host;
            }
            public void con()
            {
                try
                {
                    client = new TcpClient(host, port);
                    Console.WriteLine("connection made");
                }
                catch (System.Net.Sockets.SocketException)
                {
                    Console.WriteLine("Connection Failed");
                }
            }
            public void RecieveMessage()
            {
                if (client.Available > 0)
                {
                    try
                    {
                        byte[] RecBuffer = new byte[1024];
                        stream = client.GetStream();
                        int MessageInt = stream.Read(RecBuffer, 0, RecBuffer.Length);
                        string Message = Encoding.UTF8.GetString(RecBuffer, 0, MessageInt);
                        Console.WriteLine(Message);
                        x = float.Parse(Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("this one failed");
                    }
                }
            }
            public void SendResponse()
            {
                string response = "1";
                sendData = Encoding.ASCII.GetBytes(response);
                stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);
                Console.WriteLine("Response: " + response);
            }
            private void close()
            {
                client.Close();
                Console.WriteLine("Connection terminated");
            }
        }
        soc socket = new soc(65434, "hi", "localhost");
        private TuioClient client;
        private Dictionary<long, TuioObject> objectList;
        private Dictionary<long, TuioCursor> cursorList;
        private Dictionary<long, TuioBlob> blobList;

        public static int width, height;
        private int window_width = 640;
        private int window_height = 480;
        private int window_left = 0;
        private int window_top = 0;
        private int screen_width = Screen.PrimaryScreen.Bounds.Width;
        private int screen_height = Screen.PrimaryScreen.Bounds.Height;

        private bool fullscreen;
        private bool verbose;

        Font font = new Font("Arial", 10.0f);
        SolidBrush fntBrush = new SolidBrush(Color.White);
        SolidBrush bgrBrush = new SolidBrush(Color.FromArgb(0, 0, 64));
        SolidBrush curBrush = new SolidBrush(Color.FromArgb(192, 0, 192));
        SolidBrush objBrush = new SolidBrush(Color.FromArgb(0, 100, 0));
        SolidBrush blbBrush = new SolidBrush(Color.FromArgb(0, 0, 64));
        private Button button1;
        Pen curPen = new Pen(new SolidBrush(Color.Blue), 1);

        public TuioDemo(int port)
        {

            fullscreen = false;
            width = window_width;
            height = window_height;

            ClientSize = new Size(width, height);
            Name = "Tuio";
            Text = "Tuio";

            Closing += new CancelEventHandler(Form_Closing);
            KeyDown += new KeyEventHandler(Form_KeyDown);

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                            ControlStyles.UserPaint |
                            ControlStyles.DoubleBuffer, true);

            objectList = new Dictionary<long, TuioObject>(128);
            cursorList = new Dictionary<long, TuioCursor>(128);
            blobList = new Dictionary<long, TuioBlob>(128);

            client = new TuioClient(port);
            client.addTuioListener(this);

            client.connect();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyData == Keys.F1)
            {
                if (fullscreen == false)
                {

                    width = screen_width;
                    height = screen_height;

                    window_left = Left;
                    window_top = Top;

                    FormBorderStyle = FormBorderStyle.None;
                    Left = 0;
                    Top = 0;
                    Width = screen_width;
                    Height = screen_height;

                    fullscreen = true;
                }
                else
                {

                    width = window_width;
                    height = window_height;

                    FormBorderStyle = FormBorderStyle.Sizable;
                    Left = window_left;
                    Top = window_top;
                    Width = window_width;
                    Height = window_height;

                    fullscreen = false;
                }
            }
            else if (e.KeyData == Keys.Escape)
            {
                Close();

            }
            else if (e.KeyData == Keys.V)
            {
                verbose = !verbose;
            }

        }

        private void Form_Closing(object sender, CancelEventArgs e)
        {
            client.removeTuioListener(this);
            client.disconnect();
            Environment.Exit(0);
        }

        public void addTuioObject(TuioObject o)
        {
            lock (objectList)
            {
                objectList.Add(o.SessionID, o);
            }
            if (verbose) Console.WriteLine("add obj " + o.SymbolID + " (" + o.SessionID + ") " + o.X + " " + o.Y + " " + o.Angle);
        }

        public void updateTuioObject(TuioObject o)
        {

            if (verbose) Console.WriteLine("set obj " + o.SymbolID + " " + o.SessionID + " " + o.X + " " + o.Y + " " + o.Angle + " " + o.MotionSpeed + " " + o.RotationSpeed + " " + o.MotionAccel + " " + o.RotationAccel);
            if (o.SymbolID == 1)
            {
                OpenTaskForm(1, o.AngleDegrees);
                objectList.Remove(o.SessionID);
                refresh(new TuioTime());
            }
            if (o.SymbolID == 2)
            {
                OpenTaskForm(2, o.AngleDegrees);
            }

            if (o.SymbolID == 3)
            {
                OpenTaskForm(3, o.AngleDegrees);
            }

            if (o.SymbolID == 4)
            {
                OpenTaskForm(4, o.AngleDegrees);
            }

        }
        public void OpenTaskForm(int ID, float Angle)
        {
            if (ID == 1 && (Angle > 87 && Angle < 100) || (Angle > 265 && Angle < 280))
            {
                string text = "The rook, is a piece in the game of chess. It may move any number of squares horizontally or vertically without jumping, and it may capture an enemy piece on its path; additionally, it may participate in castling. Each player starts the game with two rooks, one in each corner on their own side of the board.";
                Form5 TaskForm = new Form5(text);
                TaskForm.ShowDialog();
                objectList.Remove(ID);
                Console.WriteLine(Angle.ToString());

            }
            if (ID == 2 && (Angle > 87 && Angle < 100) || (Angle > 265 && Angle < 280))
            {
                string text = "The queen, is the most powerful piece in the game of chess. It can move any number of squares vertically, horizontally or diagonally, combining the powers of the rook and bishop. Each player starts the game with one queen, placed in the middle of the first rank next to the king";
                Form5 TaskForm = new Form5(text);
                TaskForm.ShowDialog();
            }
            if (ID == 3 && (Angle > 87 && Angle < 100) || (Angle > 265 && Angle < 280))
            {
                string text = "The king, is the most important piece in the game of chess. It may move to any adjoining square; it may also perform, in tandem with the rook, a special move called castling";
                Form5 TaskForm = new Form5(text);
                TaskForm.ShowDialog();
            }

        }
        public void removeTuioObject(TuioObject o)
        {
            lock (objectList)
            {
                objectList.Remove(o.SessionID);
            }
            if (verbose) Console.WriteLine("del obj " + o.SymbolID + " (" + o.SessionID + ")");
        }

        public void addTuioCursor(TuioCursor c)
        {
            lock (cursorList)
            {
                cursorList.Add(c.SessionID, c);
            }
            if (verbose) Console.WriteLine("add cur " + c.CursorID + " (" + c.SessionID + ") " + c.X + " " + c.Y);
        }

        public void updateTuioCursor(TuioCursor c)
        {
            if (verbose) Console.WriteLine("set cur " + c.CursorID + " (" + c.SessionID + ") " + c.X + " " + c.Y + " " + c.MotionSpeed + " " + c.MotionAccel);
        }

        public void removeTuioCursor(TuioCursor c)
        {
            lock (cursorList)
            {
                cursorList.Remove(c.SessionID);
            }
            if (verbose) Console.WriteLine("del cur " + c.CursorID + " (" + c.SessionID + ")");
        }

        public void addTuioBlob(TuioBlob b)
        {
            lock (blobList)
            {
                blobList.Add(b.SessionID, b);
            }
            if (verbose) Console.WriteLine("add blb " + b.BlobID + " (" + b.SessionID + ") " + b.X + " " + b.Y + " " + b.Angle + " " + b.Width + " " + b.Height + " " + b.Area);
        }

        public void updateTuioBlob(TuioBlob b)
        {

            if (verbose) Console.WriteLine("set blb " + b.BlobID + " (" + b.SessionID + ") " + b.X + " " + b.Y + " " + b.Angle + " " + b.Width + " " + b.Height + " " + b.Area + " " + b.MotionSpeed + " " + b.RotationSpeed + " " + b.MotionAccel + " " + b.RotationAccel);
        }

        public void removeTuioBlob(TuioBlob b)
        {
            lock (blobList)
            {
                blobList.Remove(b.SessionID);
            }
            if (verbose) Console.WriteLine("del blb " + b.BlobID + " (" + b.SessionID + ")");
        }

        public void refresh(TuioTime frameTime)
        {
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Getting the graphics object
            Graphics g = pevent.Graphics;
            g.FillRectangle(bgrBrush, new Rectangle(0, 0, width, height));

            // draw the cursor path
            if (cursorList.Count > 0)
            {
                lock (cursorList)
                {
                    foreach (TuioCursor tcur in cursorList.Values)
                    {
                        List<TuioPoint> path = tcur.Path;
                        TuioPoint current_point = path[0];

                        for (int i = 0; i < path.Count; i++)
                        {
                            TuioPoint next_point = path[i];
                            g.DrawLine(curPen, current_point.getScreenX(width), current_point.getScreenY(height), next_point.getScreenX(width), next_point.getScreenY(height));
                            current_point = next_point;
                        }
                        g.FillEllipse(curBrush, current_point.getScreenX(width) - height / 100, current_point.getScreenY(height) - height / 100, height / 50, height / 50);
                        g.DrawString(tcur.CursorID + "", font, fntBrush, new PointF(tcur.getScreenX(width) - 10, tcur.getScreenY(height) - 10));
                    }
                }
            }

            // draw the objects
            if (objectList.Count > 0)
            {
                lock (objectList)
                {
                    foreach (TuioObject tobj in objectList.Values)
                    {
                        int ox = tobj.getScreenX(width);
                        int oy = tobj.getScreenY(height);
                        int size = height / 10;

                        g.TranslateTransform(ox, oy);
                        g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
                        g.TranslateTransform(-ox, -oy);

                        g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));

                        g.TranslateTransform(ox, oy);
                        g.RotateTransform(-1 * (float)(tobj.Angle / Math.PI * 180.0f));
                        g.TranslateTransform(-ox, -oy);

                        g.DrawString(tobj.SymbolID + "", font, fntBrush, new PointF(ox - 10, oy - 10));
                    }
                }
            }

            // draw the blobs
            if (blobList.Count > 0)
            {
                lock (blobList)
                {
                    foreach (TuioBlob tblb in blobList.Values)
                    {
                        int bx = tblb.getScreenX(width);
                        int by = tblb.getScreenY(height);
                        float bw = tblb.Width * width;
                        float bh = tblb.Height * height;

                        g.TranslateTransform(bx, by);
                        g.RotateTransform((float)(tblb.Angle / Math.PI * 180.0f));
                        g.TranslateTransform(-bx, -by);

                        g.FillEllipse(blbBrush, bx - bw / 2, by - bh / 2, bw, bh);

                        g.TranslateTransform(bx, by);
                        g.RotateTransform(-1 * (float)(tblb.Angle / Math.PI * 180.0f));
                        g.TranslateTransform(-bx, -by);

                        g.DrawString(tblb.BlobID + "", font, fntBrush, new PointF(bx, by));
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TuioDemo
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.button1);
            this.Name = "TuioDemo";
            this.ResumeLayout(false);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            socket.SendResponse();
        }

        public static void Main(string[] argv)
        {
            int port = 0;
            switch (argv.Length)
            {
                case 1:
                    port = int.Parse(argv[0], null);
                    if (port == 0) goto default;
                    break;
                case 0:
                    port = 3333;
                    break;
                default:
                    Console.WriteLine("usage: mono TuioDemo [port]");
                    Environment.Exit(0);
                    break;
            }

            TuioDemo app = new TuioDemo(port);
            Application.Run(app);
        }
    }
}