using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HraMiny
{
    public partial class frmHlavni : Form
    {
        MinyGUI miny;
        bool zmenaVelikosti;
        public frmHlavni()
        {
            InitializeComponent();
        }

        private void btnSpustit_Click(object sender, EventArgs e)
        {
            if (rbPevna.Checked == true)
                zmenaVelikosti = false;
            else if (rbPohybliva.Checked == true)
                zmenaVelikosti = true;

            miny = new MinyGUI((int)numRadky.Value, (int)numSloupce.Value, (int)numPocetMin.Value);
            miny.ZahajitHru(this.Icon, txtTitulek.Text, zmenaVelikosti, chkMinimalizace.Checked, chkMaximalizace.Checked, (int)numVelikostPolozky.Value);
        }
    }
}
