using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grafic_lab4;

public partial class FigureMoveControl : UserControl
{
    public double X
    {
        get
        {
            double.TryParse(text_x.Text, out double x);
            return x;
        }
    }

    public double Y
    {
        get
        {
            double.TryParse(text_y.Text, out double y);
            return y;
        }
    }

    public int _number;

    public FigureMoveControl(int number)
    {
        InitializeComponent();

        _number = number;
        this.number.Text = number.ToString();
    }

    private void InputNumber(object sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
        {
            e.Handled = true;
        }
    }
}
