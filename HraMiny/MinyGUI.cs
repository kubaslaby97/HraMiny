using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HraMiny
{
    public class MinyGUI
    {
        int pocet_radku, pocet_sloupcu, pocet_min, odkryto, nalezene_miny = 0;
        bool konec_hry = false;
        static Image kryci, vlajka, mina, objev_mina, neni_mina, prazdna;

        PictureBox[,] hraci_pole_GUI;
        PictureBox[] pole_GUI;

        public MinyGUI(int pocet_radku, int pocet_sloupcu, int pocet_min)
        {
            this.pocet_radku = pocet_radku;
            this.pocet_sloupcu = pocet_sloupcu;
            this.pocet_min = pocet_min;
            odkryto = (pocet_radku * pocet_sloupcu) - pocet_min;
        }

        private void HraciPoleGUI(PictureBox[] pole_GUI, Image[,] kryci_pole, Image[,] hraci_pole)
        {
            hraci_pole_GUI = new PictureBox[pocet_radku, pocet_sloupcu];

            //tvorba dvourozměrného pole obrázků v GUI
            for (int i = 0; i < pocet_radku; i++)
            {
                for (int j = 0; j < pocet_sloupcu; j++)
                {
                    hraci_pole_GUI[i, j] = pole_GUI[i * pocet_sloupcu + j];
                    hraci_pole_GUI[i, j].Image = kryci_pole[i, j];
                    hraci_pole_GUI[i, j].BackgroundImage = hraci_pole[i, j];
                    hraci_pole_GUI[i, j].Tag = i + "," + j;
                }
            }
        }

        private Image[] PoctyMin()
        {
            Image[] pocet = new Image[8];

            for (int i = 0; i < 8; i++)
            {
                if (File.Exists("obr\\" + i + ".png"))
                {
                    pocet[i] = Image.FromFile("obr\\" + i + ".png");
                }
            }

            return pocet;
        }

        private void KliknutiNaKarty(PictureBox[,] hraci_pole_GUI)
        {
            foreach (PictureBox polozka in pole_GUI)
            {
                polozka.MouseClick += (s, e) =>
                {
                    PictureBox vybrana_polozka = (PictureBox)s;
                    int x = int.Parse(vybrana_polozka.Tag.ToString().Split(',')[0]);
                    int y = int.Parse(vybrana_polozka.Tag.ToString().Split(',')[1]);

                    if (vybrana_polozka != null)
                    {
                        if (vybrana_polozka.Image == null || konec_hry)
                            return;

                        // pravé tlačítko myši
                        if (e.Button == MouseButtons.Right)
                        {
                            vybrana_polozka.Image = vlajka;

                            // kontrola výhry
                            KontrolaVyhry(x, y);
                        }
                        // levé tlačítko myši
                        else if (e.Button == MouseButtons.Left)
                        {
                            // odstranění vlajky
                            if (vybrana_polozka.Image == vlajka)
                            {
                                vybrana_polozka.Image = kryci;

                                // pokud byla odstraněna vlajka z nalezené miny, tak se od počtu min odečte 1
                                if (vybrana_polozka.BackgroundImage == mina)
                                {
                                    nalezene_miny--;
                                }
                            }
                            // bez vlajky
                            else if (vybrana_polozka.Image != vlajka)
                            {
                                // kliknutí na minu
                                if (vybrana_polozka.BackgroundImage == mina)
                                {
                                    vybrana_polozka.Image = objev_mina;
                                    OdhaleniMin();

                                    MessageBox.Show("Prohrál/a jste", "Prohra", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    konec_hry = true;
                                }
                                else
                                {
                                    // odkrytí sousedů
                                    OdkrytiSousedu(x, y);

                                    // kontrola výhry
                                    KontrolaVyhry(x, y);
                                }
                            }
                        }
                    }
                };
            }
        }

        private void KontrolaVyhry(int x, int y)
        {
            if (hraci_pole_GUI[x, y].BackgroundImage == mina && hraci_pole_GUI[x, y].Image == vlajka)
            {
                nalezene_miny++;
            }

            if (odkryto == 0)
            {
                Vlajky();
                if (nalezene_miny == pocet_min)
                {
                    MessageBox.Show("Vyhrál/a jste", "Gratulace", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    konec_hry = true;
                }
            }
            
        }

        private void OdhaleniMin()
        {
            for (int i = 0; i < pocet_radku; i++)
            {
                for (int j = 0; j < pocet_sloupcu; j++)
                {
                    // odhalení zbylých min
                    if (hraci_pole_GUI[i, j].BackgroundImage == mina && hraci_pole_GUI[i, j].Image != objev_mina)
                    {
                        // odhalení neoznačené miny vlajkou
                        if (hraci_pole_GUI[i, j].Image != vlajka)
                        {
                            hraci_pole_GUI[i, j].Image = null;
                        }
                    }

                    // špatně nalezená mina
                    if (hraci_pole_GUI[i, j].BackgroundImage != mina && hraci_pole_GUI[i, j].Image == vlajka)
                    {
                        hraci_pole_GUI[i, j].Image = neni_mina;
                    }
                }
            }
        }

        private void Vlajky()
        {
            for (int i = 0; i < pocet_radku; i++)
            {
                for (int j = 0; j < pocet_sloupcu; j++)
                {
                    // přidání vlajky
                    if (hraci_pole_GUI[i, j].BackgroundImage == mina && hraci_pole_GUI[i, j].Image == kryci)
                    {
                        hraci_pole_GUI[i, j].Image = vlajka;
                    }
                }
            }

            nalezene_miny = pocet_min;
        }

        private void OdkrytiSousedu(int x, int y)
        {
            // kontrola, zda je x v rozsahu
            if (x < 0 || x >= pocet_radku)
            {
                return;
            }

            // kontrola, zda je y v rozsahu
            if (y < 0 || y >= pocet_sloupcu)
            {
                return;
            }

            // navštívené políčko
            if (hraci_pole_GUI[x,y].Image == null)
            {
                return;
            }

            hraci_pole_GUI[x, y].Image = null;
            odkryto--;

            if (hraci_pole_GUI[x, y].BackgroundImage == prazdna)
            {
                OdkrytiSousedu(x + 1, y);
                OdkrytiSousedu(x, y + 1);
                OdkrytiSousedu(x - 1, y);
                OdkrytiSousedu(x, y - 1);
            }
        }

        private void ZobrazitHerniOkno(Icon ikona, string titulek, bool zmena_velikosti, bool minimalizace, bool maximalizace, int velikost_polozky)
        {
            Form form = new Form();
            TableLayoutPanel panel = new TableLayoutPanel();

            form.Icon = ikona;
            form.Text = titulek;
            form.ClientSize = new Size(pocet_radku * velikost_polozky, pocet_sloupcu * velikost_polozky);

            // pohyblivá velikost
            if (zmena_velikosti == true)
                form.FormBorderStyle = FormBorderStyle.Sizable;
            // pevná velikost
            else if (zmena_velikosti == false)
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
            // možnost minimalizace
            if (minimalizace == false)
                form.MinimizeBox = false;
            // možnost maximalizace
            if (maximalizace == false)
                form.MaximizeBox = false;

            form.StartPosition = FormStartPosition.CenterScreen;
            form.Load += (s, e) =>
            {
                kryci = Image.FromFile("obr\\vrsek.png");
                mina = Image.FromFile("obr\\mina.png");
                vlajka = Image.FromFile("obr\\vlajka.png");
                objev_mina = Image.FromFile("obr\\objev-mina.png");
                prazdna = Image.FromFile("obr\\prazdna.png");
                neni_mina = Image.FromFile("obr\\neni-mina.png");

                Miny miny = new Miny(pocet_min, mina, kryci, prazdna, PoctyMin());

                HraciPoleGUI(pole_GUI, miny.KryciPole(pocet_radku, pocet_sloupcu), miny.HraciPole(pocet_radku, pocet_sloupcu));
                KliknutiNaKarty(hraci_pole_GUI);
            };

            panel.AutoSize = true;
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = pocet_radku;
            panel.RowCount = pocet_sloupcu;

            for (int i = 0; i < pocet_radku; i++)
            {
                for (int j = 0; j < pocet_sloupcu; j++)
                {
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (pocet_radku * 100F) / form.Width));
                    panel.RowStyles.Add(new RowStyle(SizeType.Percent, (pocet_sloupcu * 100F) / form.Height));
                    panel.Controls.Add(new PictureBox() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.StretchImage, BackgroundImageLayout = ImageLayout.Stretch }, i, j);
                }
            }

            form.Controls.Add(panel);
            pole_GUI = panel.Controls.OfType<PictureBox>().ToArray();
            form.Show();
        }

        public void ZahajitHru(Icon ikona, string titulek, bool zmena_velikosti, bool minimalizace, bool maximalizace, int velikost_polozky)
        {
            if (File.Exists("obr\\vrsek.png"))
            {
                // kontrola zda byl zadán správný počet min
                if (pocet_min <= (pocet_radku * pocet_sloupcu))
                {
                    ZobrazitHerniOkno(ikona, titulek, zmena_velikosti, minimalizace, maximalizace, velikost_polozky);
                }
                else
                {
                    MessageBox.Show("Nesprávný počet min", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("Počet min musí být násobkem počtu řádků a sloupců", "Informace", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Nebyl nalezen soubor krycí karty", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Nápověda pro správné pojmenování\n\n" +
                            "- složka \'obr\' musí být ve stejné složce jako aplikace\n" +
                            "- soubor musí mít název \'vrsek.png\'\n" +
                            "- soubor musí být umístěn ve složce \'obr\'", "Informace",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}