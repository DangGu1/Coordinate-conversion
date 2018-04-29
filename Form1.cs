using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataGridViewTask
{
    public partial class Form1 : Form
    {
        double N;
        const double a = 6378137;
        const double b = 6356752.31414;
        double e = Math.Sqrt(Math.Pow(a,2)-Math.Pow(b,2))/a;
        bool SelectRadioJudgement;//用于判断是否选择了格式
        public Form1()
        {
            InitializeComponent();
            AddRow();
            SetFormat();
        }
        //设置DataGridView的默认格式
        private void SetFormat()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;//第一列不再显示
            dataGridView1.AllowUserToResizeColumns = false;//用户不可以自己调整列大小
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ReadOnly = false;
        }
        //添加行
        private int AddRow()
        {
                int index = dataGridView1.Rows.Add();
                return index;

        }
        //大地坐标系的结构体
        struct GC
        {
            public double latitude;
            public double longtitude;
            public double height;
        }
        //空间直角坐标系
        struct SpaceRC
        {
            public double x;
            public double y;
            public double z;

        }
        private void buttonAdd1_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
            else
            {
                MessageBox.Show("已经没有可以删除的行了！");
            }
        }

        private void buttonClear1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var temp = dataGridView1.Rows[0].Cells;
            temp[3].Value=30.3155;
            temp[4].Value = 114.2127;
            temp[5].Value = 134.6;
        }
        //角度化弧度  34°25′23″ = 34.2523以及格式识别
        private double ToRadian(double angle)
        {
            double degree;
            double minute;
            double second;
            double ret;
            if (radioButton10.Checked==true)//判断点击了哪个按钮
            {
                degree = (int)(angle);
                minute = (int)((angle - degree) * 100);
                second = (int)((angle - degree - minute / 100) * 10000);
                ret = (degree + minute / 60 + second / 3600) / 180 * Math.PI;
                SelectRadioJudgement = true;
                return ret;
            }
            else if (radioButton60.Checked==true)
            {
                ret = angle * Math.PI / 180;
                SelectRadioJudgement = true;
                return ret;
            }
            else
            {
                SelectRadioJudgement = false;
                return 0;
            }

        }
        //把输入的数据传入到变量中
        private void AssignGC1(ref GC gc)
        {
            var temp = dataGridView1.CurrentRow.Cells;
            gc.latitude = ToRadian(double.Parse(temp[3].Value.ToString()));
            gc.longtitude = ToRadian(double.Parse(temp[4].Value.ToString()));
            gc.height = double.Parse(temp[5].Value.ToString());
            
        }
        //计算X坐标
        private double calculateX(GC temp)
        {
            double B = temp.latitude;
            double L = temp.longtitude;
            double H = temp.height;
            N =  a / Math.Sqrt(1-Math.Pow(e,2)*Math.Sin(B));
            return(N + H) * Math.Cos(B) * Math.Cos(L);//返回N

        }
        //计算Y坐标
        private double calculateY(GC temp)
        {
            double B = temp.latitude;
            double L = temp.longtitude;
            double H = temp.height;
            return (N + H) * Math.Cos(B) * Math.Sin(L);
        }
        //计算Z坐标
        private double calculateZ(GC temp)
        {
            double B = temp.latitude;
            double L = temp.longtitude;
            double H = temp.height;
            return (N * (1.0 - FlatRate * (2 - FlatRate)) + H) * Math.Sin(B);
        }
        private void buttonCal1_Click(object sender, EventArgs e)
        {
            if (emptyCheck()&&inputCheck())
            {
                var temp = dataGridView1.CurrentRow.Cells;
                GC coordinateGC1 = new GC();
                AssignGC1(ref coordinateGC1);
                if (SelectRadioJudgement)//是否
                {
                    temp[0].Value = calculateX(coordinateGC1).ToString("F5");
                    temp[1].Value = calculateY(coordinateGC1).ToString("F5");
                    temp[2].Value = calculateZ(coordinateGC1).ToString("F5");
                }
                else
                {
                    MessageBox.Show("请先选择格式！");
                }
            }

        }

        //计算快捷键 Enter
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonCal1_Click(sender, e);
            } 
        }
        private bool inputCheck()
        {
            int j = 0;
            double temp;
            for (int i = 3; i < dataGridView1.ColumnCount; i++)
            {
                if (double.TryParse(dataGridView1.CurrentRow.Cells[i].Value.ToString(), out temp))
                {
                    j++;
                }
                else
                {
                    MessageBox.Show("第" + (i +1)+ "列的数据格式有误！");
                    break;
                }
            }
            if (j == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //对单元格内的数据进行非空检查
        private bool emptyCheck()
        {
            int i = 0;
            var tag = dataGridView1.CurrentRow.Cells;
            for (int j = 3; j < dataGridView1.ColumnCount ; j++)
            {
                if (tag[j].Value == null)
                {
                    MessageBox.Show("你的第" + (j +1)+ "格为空，请输入数据");
                    break;
                }
                i++;
            }
            if (i == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
