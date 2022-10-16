using System.Diagnostics;
using ZedGraph;

namespace Ustr_lab1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;

            zedGraphControl1.IsShowPointValues = true;
            zedGraphControl1.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);
        }
        string zedGraph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair point = curve[iPt];
            string result = string.Format("X: {0:F3}\nY: {1:F3}", point.X, point.Y);
            return result;
        }

        List<double> points = new List<double>();
        int counter = 0;
        int pe = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                Programm();
            }
            if (radioButton2.Checked == true)
            {
                try
                {
                    if (textBox13.Text == "")
                    {
                        textBox13.Focus();
                        throw new Exception("Введите g!");
                    }
                    if (textBox14.Text == "")
                    {
                        textBox14.Focus();
                        throw new Exception("Введите k!");
                    }
                    Graph();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void Programm()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            PointPairList list1 = new PointPairList();
            pane.CurveList.Clear();
            pane.XAxis.Title.Text = "P";
            pane.YAxis.Title.Text = "Pe/N";
            pane.Title.Text = "Вероятность обнаружения ошибки";
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();

            if (checkBox1.Checked == false)
            {
                points = new List<double>();
            }

            counter = 0;
            pe = 0;

            try
            {
                string mx;
                if (textBox1.Text == "")
                {
                    textBox1.Focus();
                    throw new Exception("Введите m!");
                }
                if (textBox13.Text == "")
                {
                    textBox13.Focus();
                    throw new Exception("Введите g!");
                }
                if (textBox14.Text == "")
                {
                    textBox14.Focus();
                    throw new Exception("Введите k!");
                }
                if (textBox2.Text == "")
                {
                    textBox2.Focus();
                    throw new Exception("Введите p!");
                }

                char[] g = textBox13.Text.ToCharArray();
                int r = g.Length - 1;
                int k = Convert.ToInt32(textBox14.Text);

                int g_int = ConvertCharToInt(g);

                int d = 4, n = k + r, mxxr;

                while (counter <= 100)
                {
                    int m10 = Convert.ToInt32(textBox1.Text);

                    if (m10 > (int)Math.Pow(2, k) && m10 < (int)Math.Pow(2, k - 1))
                    {
                        textBox1.Clear();
                        textBox1.Focus();
                        throw new Exception("m слишком большое!");
                    }

                    string BinaryCode = Convert.ToString(m10, 2);

                    char[] m = BinaryCode.ToCharArray();

                    FirstStep(m, out mx);

                    int cx = SecondStep(out mxxr, m10, r, g_int, n, k);

                    char[] ax2 = ThirdStep(mxxr, cx, n);
                    int ax_length = ax2.Length;


                    double p = Convert.ToDouble(textBox2.Text);

                    char[] E = FifthStep(n, p);

                    SixthStep(ax_length, ref E, ref ax2, ref g);

                    counter++;
                }

                double Pep = (double)pe / (double)counter;
                textBox12.Text = Pep.ToString();

                points.Add(Convert.ToDouble(textBox2.Text));
                points.Add(Pep);

                counter = 0;
                pe = 0;

                for (int i = 0; i < points.Count; i += 2)
                {
                    list1.Add(points[i], points[i + 1]);
                }

                LineItem myCurve1 = pane.AddCurve("", list1, Color.Blue, SymbolType.Circle);
                myCurve1.Symbol.Fill.Type = FillType.Solid;
                myCurve1.Symbol.Size = 5;

                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private int ConvertCharToInt(char[] g)
        {
            int g_int = 0;
            for (int i = g.Length - 1, j = 0; i >= 0; i--, j++)
            {
                g_int += (g[j] - '0') * Convert.ToInt32(Math.Pow(2, i));
            }
            return g_int;
        }

        private string ConversionToPolynomial(char[] a)
        {
            string str = new string(a);
            str = str.TrimStart('0');
            a = str.ToCharArray();

            string xx = "x^" + (a.Length - 1);

            int b = 0;
            for (int i = 0; i < a.Length; i++)
            {
                b += a[i] - '0';
            }

            if (b == 0)
            {
                xx = "0";
                return xx;
            }

            if (a.Length == 1)
            {
                xx = "1";
            }
            for (int i = a.Length - 2, j = 1; i >= 0; i--, j++)
            {
                if (i == 0 && a[j] == '1')
                {
                    xx = xx + " + 1";
                }
                else if (a[j] == '1')
                {
                    xx = xx + " + x^" + i;
                }
            }
            return xx;
        }

        private int Division(int mxxr, int g, int n, int k)
        {
            int l = g;
            l = l << (n - k);

            int degree_a = 0;
            int degree_l = 0;


            for (int i = 0; i <= n - k; i++)
            {
                if (mxxr == 0)
                {
                    return mxxr;
                }
                // проверка степени a
                int two = 1 << n;
                for (int j = n; j > 0; j--)
                {
                    if (mxxr >= (two >> 1) && mxxr < two)
                    {
                        degree_a = j;
                        break;
                    }
                    two = two >> 1;
                }

                // проверка степени l
                two = 1 << n;
                for (int j = n; j > 0; j--)
                {
                    if (l >= (two >> 1) && l < two)
                    {
                        degree_l = j;
                        break;
                    }
                    two = two >> 1;
                }

                if (degree_a >= degree_l)
                {
                    mxxr = mxxr ^ l;
                }

                l = l >> 1;
            }

            return mxxr;
        }
        
        private void FirstStep(char[] m, out string mx)
        {
            mx = ConversionToPolynomial(m);

            textBox3.Text = mx;
            textBox4.Text = new string(m);
        }

        private int SecondStep(out int mxxr, int m10, int r, int g, int n, int k)
        {
            mxxr = m10 << r;
            string mxxr_string = Convert.ToString(mxxr, 2);

            textBox5.Text = mxxr_string;

            int cx_int = Division(mxxr, g, n, r + 1);
            textBox6.Text = new string(Convert.ToString(cx_int, 2));

            return cx_int;
        }

        private char[] ThirdStep(int mxxr, int cx, int n)
        {
            int ax = mxxr + cx;
            string ax_string = Convert.ToString(ax, 2);

            if (ax_string.Length < n)
            {
                string h = "";
                for (int i = 0; i < n - ax_string.Length; i++)
                {
                    h += "0";
                }
                ax_string = h + ax_string;
            }

            char[] ax2 = ax_string.ToCharArray();
            textBox7.Text = ConversionToPolynomial(ax2);
            FourthStep(ax_string);
            return ax2;
        }

        private void FourthStep(string ax_string)
        {
            textBox8.Text = ax_string;
        }

        private char[] FifthStep(int n, double p)
        {
            char[] E = new char[n];

            for (int i = 0; i < n; i++)
            {
                E[i] = '0';
            }

            for (int i = 0; i < n; i++)
            {
                Random rand = new Random();
                double new_e;
                int rand_e = rand.Next(0, 2147483646);
                new_e = (double)rand_e;
                new_e = new_e / 2147483646.0;

                if (new_e < p)
                {
                    E[i] = '1';
                }
                else if (new_e >= p)
                {
                    E[i] = '0';
                }
            }
            return E;
        }

        private void SixthStep(int ax_length, ref char[] E, ref char[] ax2, ref char[] g)
        {
            char[] b = new char[ax_length];

            for (int i = 0; i < ax_length; i++)
            {
                if (E[i] == ax2[i])
                {
                    b[i] = '0';
                }
                else
                {
                    b[i] = '1';
                }
            }

            string e_string = new string(E);
            string b_string = new string(b);
            b = b_string.ToCharArray();

            textBox9.Text = e_string;
            textBox10.Text = b_string;
            textBox11.Text = ConversionToPolynomial(b);

            int b_int = ConvertCharToInt(b);
            int g_int = ConvertCharToInt(g);

            int s_int = Division(b_int, g_int, b.Length, g.Length);

            int e10 = Convert.ToInt32(e_string, 2);

            if (s_int != 0 && e10 > 0)
            {
                pe++;
            }
        }

        private void Graph()
        {
            Stopwatch sw = Stopwatch.StartNew();

            points = new List<double>();

            GraphPane pane = zedGraphControl1.GraphPane;
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();

            pane.XAxis.Title.Text = "P";
            pane.YAxis.Title.Text = "Pe/N";
            pane.Title.Text = "Вероятность обнаружения ошибки";

            if (checkBox1.Checked == false)
            {
                pane.CurveList.Clear();
                zedGraphControl1.GraphPane.CurveList.Clear();
                zedGraphControl1.GraphPane.GraphObjList.Clear();
            }

            char[] g = textBox13.Text.ToCharArray();
            int r = g.Length - 1;
            int k = Convert.ToInt32(textBox14.Text);
            int gx = ConvertCharToInt(g);

            counter = 0;
            pe = 0;

            double p = 0;

            sw.Start();
            while (p <= 1.01)
            {
                while (counter < 10000)
                {
                    int n = k + r;
                    int m10 = 0;
                    Random rand = new Random();

                    if (checkBox2.Checked == true)
                    {
                        m10 = Convert.ToInt32(textBox1.Text);
                    }
                    else
                    {
                        m10 = (int)rand.Next(0, (1 << k) - 1);
                    }

                    int mxxr = m10 << r;

                    int cx = Division(mxxr, gx, n, (r + 1));
                    
                    int ax = mxxr + cx;

                    // e
                    int ex = 0;
                    for (int i = n - 1; i >= 0; i--)
                    {
                        double new_e = rand.NextDouble();

                        if (new_e < p)
                        {
                            ex = ex + (1 << i);
                        }
                    }

                    int bx = ax ^ ex;

                    // проверка степени
                    int degree = 0;
                    uint two = (uint)(1 << n);
                    for (int j = n; j > 0; j--)
                    {
                        if (bx >= (two >> 1) && bx < two)
                        {
                            degree = j;
                            break;
                        }
                        two = two >> 1;
                    }

                    int s = Division(bx, gx, degree, (r + 1));

                    if (s != 0 && ex > 0)
                    {
                        pe++;
                    }
                    counter++;
                }

                double Pep = (double)pe / (double)counter;

                points.Add(p);
                points.Add(Pep);

                p = p + 0.01;

                counter = 0;
                pe = 0;
            }
            sw.Stop();

            textBox15.Text = sw.Elapsed.ToString();


            if (checkBox2.Checked == true)
            {
                for (int i = 0; i < points.Count; i += 2)
                {
                    list2.Add(points[i], points[i + 1]);
                }
            }
            else
            {
                for (int i = 0; i < points.Count; i += 2)
                {
                    list1.Add(points[i], points[i + 1]);
                }
            }

            LineItem myCurve1 = pane.AddCurve("", list1, Color.Blue, SymbolType.None);
            myCurve1.Symbol.Fill.Type = FillType.Solid;
            myCurve1.Symbol.Size = 1;

            LineItem myCurve2 = pane.AddCurve("", list2, Color.Red, SymbolType.None);
            myCurve2.Symbol.Fill.Type = FillType.Solid;
            myCurve2.Symbol.Size = 1;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 50) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != 44)
            {
                e.Handled = true;
            }
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
        }
    }
}