using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CuatroNotBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hwnd, int x, int y);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private int Energy { get; set; } = 24;
        private Block1 block1 { get; set; }
        private Block2 block2 { get; set; }
        private Block3 block3 { get; set; }
        private Block4 block4 { get; set; }

        private System.Windows.Point[,] GeneralCoordinatesTheBlocksCells = new System.Windows.Point[32, 32];

        private System.Drawing.Color[,] TemplateColors = new System.Drawing.Color[32,32];

        private System.Drawing.Color CurrentSelectedColor { get; set; }

        private Dictionary<string, System.Windows.Point> AvailableColorPalette = new Dictionary<string, System.Windows.Point>
        {
            {"228:110:110",new System.Windows.Point(747,858)},
            {"255:214:53",new System.Windows.Point(793,858)},
            {"126:237:86",new System.Windows.Point(840,858)},
            {"0:204:192",new System.Windows.Point(888,858)},
            {"81:233:244",new System.Windows.Point(935,858)},
            {"148:179:255",new System.Windows.Point(981,858)},
            {"228:171:255",new System.Windows.Point(1029,858)},
            {"255:153:170",new System.Windows.Point(1076,858)},
            {"255:180:112",new System.Windows.Point(1123,858)},
            {"255:255:255",new System.Windows.Point(1170,858)},

            {"190:0:57",new System.Windows.Point(747,907)},
            {"255:150:0",new System.Windows.Point(793,907)},
            {"0:204:120",new System.Windows.Point(840,907)},
            {"0:158:170",new System.Windows.Point(888,907)},
            {"54:144:234",new System.Windows.Point(935,907)},
            {"106:92:255",new System.Windows.Point(981,907)},
            {"180:74:192",new System.Windows.Point(1029,907)},
            {"255:56:129",new System.Windows.Point(1076,907)},
            {"156:105:38",new System.Windows.Point(1123,907)},
            {"137:141:144",new System.Windows.Point(1170,907)},

            {"109:0:26",new System.Windows.Point(747,952)},
            {"191:67:0",new System.Windows.Point(793,952)},
            {"0:163:104",new System.Windows.Point(840,952)},
            {"0:117:111",new System.Windows.Point(888,952)},
            {"36:80:164",new System.Windows.Point(935,952)},
            {"73:58:193",new System.Windows.Point(981,952)},
            {"129:30:159",new System.Windows.Point(1029,952)},
            {"160:3:87",new System.Windows.Point(1076,952)},
            {"109:72:47",new System.Windows.Point(1123,952)},
            {"0:0:0",new System.Windows.Point(1170,952)}
        };


        public MainWindow()
        {
            InitializeComponent();
            InitializeBlocks();

            
        }

        // don't touch
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {

            }
        }
        private void InitializeBlocks()
        {
            block1 = new Block1();
            block2 = new Block2();
            block3 = new Block3();
            block4 = new Block4();

            block1.Show();
            block2.Show();
            block3.Show();
            block4.Show();
        }
        private void FillGeneralCoordinatesTheBlocksCells() {

            IBaseMethods baseMethods = (IBaseMethods)block1;
            System.Windows.Point[,] coordinatesBlock1Cells = baseMethods.GetCoordinatesTheBlockCells(block1.block1);

            baseMethods = (IBaseMethods)block2;
            System.Windows.Point[,] coordinatesBlock2Cells = baseMethods.GetCoordinatesTheBlockCells(block2.block2);

            baseMethods = (IBaseMethods)block3;
            System.Windows.Point[,] coordinatesBlock3Cells = baseMethods.GetCoordinatesTheBlockCells(block3.block3);

            baseMethods = (IBaseMethods)block4;
            System.Windows.Point[,] coordinatesBlock4Cells = baseMethods.GetCoordinatesTheBlockCells(block4.block4);

            for (int i = 0; i < 32; i++) {
                for (int j = 0; j < 32; j++) { 
                    if( i<16 && j <16)
                    {
                        GeneralCoordinatesTheBlocksCells[i,j] = coordinatesBlock1Cells[j,i];
                    }
                    if( i<16 && j >=16)
                    {
                        GeneralCoordinatesTheBlocksCells[i, j] = coordinatesBlock2Cells[j - 16,i];
                    }
                    if( i >= 16 && j < 16)
                    {
                        GeneralCoordinatesTheBlocksCells[i, j] = coordinatesBlock3Cells[j, i - 16];
                    }
                    if( i>= 16 && j >= 16)
                    {
                        GeneralCoordinatesTheBlocksCells[i, j] = coordinatesBlock4Cells[j - 16, i - 16];
                    }
                }
            }
        }
        private System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hDC = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hDC, x, y);
            ReleaseDC(IntPtr.Zero, hDC);

            byte r = (byte)(pixel & 0x000000FF);
            byte g = (byte)((pixel & 0x0000FF00) >> 8);
            byte b = (byte)((pixel & 0x00FF0000) >> 16);
            System.Drawing.Color pixelColor = System.Drawing.Color.FromArgb(r,g,b);
            return pixelColor;
        }
        private bool FillTemplateColorsArr()
        {
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    System.Windows.Point point = GeneralCoordinatesTheBlocksCells[i, j];
                    TemplateColors[i, j] = GetPixelColor((int)point.X, (int)point.Y);
                    SetCursorPos((int)point.X, (int)point.Y);
                    Thread.Sleep(5);
                }
            }
            return true;
        }
        private void BtnReadTemplate_Click(object sender, RoutedEventArgs e)
        {
            FillGeneralCoordinatesTheBlocksCells();
            block1.WindowState = WindowState.Minimized;
            block2.WindowState = WindowState.Minimized;
            block3.WindowState = WindowState.Minimized;
            block4.WindowState = WindowState.Minimized;
            bool result = FillTemplateColorsArr();
            if (result)
            {
                block1.WindowState = WindowState.Normal;
                block2.WindowState = WindowState.Normal;
                block3.WindowState = WindowState.Normal;
                block4.WindowState = WindowState.Normal;
            }

        }
        private void SelectColorInstruction(int x,int y) {
            SetCursorPos(750, 950);
            mouse_event((int)MouseEventFlags.LeftDown, 750, 950, 0, 0);
            Thread.Sleep(10);
            mouse_event((int)MouseEventFlags.LeftUp, 750, 950, 0, 0);
            Thread.Sleep(1000);
            SetCursorPos(x,y);
            mouse_event((int)MouseEventFlags.LeftDown, x, y, 0, 0);
            Thread.Sleep(10);
            mouse_event((int)MouseEventFlags.LeftUp, x, y, 0, 0);
            SetCursorPos(750, 775);
            mouse_event((int)MouseEventFlags.LeftDown, x, y, 0, 0);
            Thread.Sleep(10);
            mouse_event((int)MouseEventFlags.LeftUp, x, y, 0, 0);
            Thread.Sleep(1000);
        }
        private void RestartTelegramAppInstruction()
        {
            SetCursorPos(1880,35);
            mouse_event((int)MouseEventFlags.LeftDown, 0, 0, 0, 0);
            Thread.Sleep(10);
            mouse_event((int)MouseEventFlags.LeftUp, 0, 0, 0, 0);

            //Thread.Sleep(7500); //two hour and 5 min
            Thread.Sleep(3000);
            SetCursorPos(460, 990);
            mouse_event((int)MouseEventFlags.LeftDown, 0, 0, 0, 0);
            Thread.Sleep(10);
            mouse_event((int)MouseEventFlags.LeftUp, 0, 0, 0, 0);
            Thread.Sleep(10000);


        }
        private void BtnStartApp_Click(object sender, RoutedEventArgs e)
        {
            block1.WindowState = WindowState.Minimized;
            block2.WindowState = WindowState.Minimized;
            block3.WindowState = WindowState.Minimized;
            block4.WindowState = WindowState.Minimized;

            while (true)
            {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
            
                            
                            System.Windows.Point currentPixel = GeneralCoordinatesTheBlocksCells[i, j];
                            System.Drawing.Color currentPixelColor = GetPixelColor((int)currentPixel.X, (int)currentPixel.Y);
                            System.Drawing.Color templatePixelColor = TemplateColors[i, j];

                            if (currentPixelColor.R != templatePixelColor.R
                                && currentPixelColor.G != templatePixelColor.G
                                && currentPixelColor.B != templatePixelColor.B)
                            {
                                if (CurrentSelectedColor.R == templatePixelColor.R
                                    && CurrentSelectedColor.G == templatePixelColor.G
                                    && CurrentSelectedColor.B == templatePixelColor.B)
                                {
                                    SetCursorPos((int)currentPixel.X, (int)currentPixel.Y);
                                    mouse_event((int)MouseEventFlags.LeftDown, (int)currentPixel.X, (int)currentPixel.Y, 0, 0);
                                    Thread.Sleep(10);
                                    mouse_event((int)MouseEventFlags.LeftUp, (int)currentPixel.X, (int)currentPixel.Y, 0, 0);
                                    Energy--;
                                    continue;
                                }
                                if (CurrentSelectedColor.R != templatePixelColor.R
                                    || CurrentSelectedColor.G != templatePixelColor.G
                                    || CurrentSelectedColor.B != templatePixelColor.B)
                                {

                                    //awailable infinity updating
                                    bool succes = AvailableColorPalette.TryGetValue($"{templatePixelColor.R}:{templatePixelColor.G}:{templatePixelColor.B}",
                                            out System.Windows.Point point);
                                    if (succes)
                                    {
                                        SelectColorInstruction((int)point.X, (int)point.Y);
                                        CurrentSelectedColor = System.Drawing.Color.FromArgb(templatePixelColor.R, templatePixelColor.G, templatePixelColor.B);
                                        SetCursorPos((int)currentPixel.X, (int)currentPixel.Y);
                                        mouse_event((int)MouseEventFlags.LeftDown, (int)currentPixel.X, (int)currentPixel.Y, 0, 0);
                                        Thread.Sleep(10);
                                        mouse_event((int)MouseEventFlags.LeftUp, (int)currentPixel.X, (int)currentPixel.Y, 0, 0);
                                        Energy--;
                                        continue;
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                            }
                        
                        //else
                        //{
                        //    RestartTelegramAppInstruction();
                        //    return;
                        //}
                        
                        Thread.Sleep(2000);

                    }
                }
            }
        }
        // don't touch
    }
}