using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HraMiny
{
    public class Miny
    {
        Image mina, kryci, prazdna;
        Image[] pocty_min;
        int pocet_sousedu, pocet_min;

        public Miny(int pocet_min, Image mina, Image kryci, Image prazdna, Image[] pocty_min)
        {
            this.pocet_min = pocet_min;
            this.mina = mina;
            this.kryci = kryci;
            this.pocty_min = pocty_min;
            this.prazdna = prazdna;
        }

        private Image[,] PoleMin(int pocet_radku, int pocet_sloupcu, int pocet_min)
        {
            Random rnd = new Random();
            Image[,] pole = new Image[pocet_radku, pocet_sloupcu];

            for (int i = 0; i < pocet_radku; i++)
            {
                for (int j = 0; j < pocet_sloupcu; j++)
                {
                    if (pocet_min > 0)
                    {
                        int x = rnd.Next(0, pocet_radku);
                        int y = rnd.Next(0, pocet_sloupcu);

                        if (pole[x, y] == null)
                        {
                            pole[x, y] = mina;
                            pocet_min--;
                        }
                    }
                }
            }

            return pole;
        }

        public Image[,] HraciPole(int pocet_radku, int pocet_sloupcu)
        {
            Image[,] pole = PoleMin(pocet_radku, pocet_sloupcu, pocet_min);

            for (int i = 0; i < pocet_radku; i++)
            {
                for (int j = 0; j < pocet_sloupcu; j++)
                {
                    // kontrola existence sousedů
                    pocet_sousedu = 0;
                    for (int soused_i = -1; soused_i <= 1; soused_i++)
                    {
                        for (int soused_j = -1; soused_j <= 1; soused_j++)
                        {
                            // kontrola souřadnic [0 ; 0]
                            if ((i + soused_i) > -1 && (j + soused_j) > -1)
                            {
                                // kontrola souřadnic [řádky ; sloupce]
                                if ((i + soused_i) < pocet_radku && (j + soused_j) < pocet_sloupcu)
                                {
                                    // kontrola existence miny
                                    if (pole[i + soused_i, j + soused_j] == mina)
                                    {
                                        pocet_sousedu++;
                                    }
                                }
                            }
                        }
                    }

                    // počet sousedů
                    if (pole[i,j] == null)
                    {
                        if (pocet_sousedu == 0)
                        {
                            pole[i, j] = prazdna;
                        }
                        else if (pocet_sousedu > 0)
                        {
                            pole[i, j] = pocty_min[pocet_sousedu];
                        }
                    }
                }
            }
            return pole;
        }

        public Image[,] KryciPole(int pocet_radku, int pocet_sloupcu)
        {
            Image[,] pole = new Image[pocet_radku, pocet_sloupcu];

            for (int i = 0; i < pocet_radku; i++)
            {
                for (int j = 0; j < pocet_sloupcu; j++)
                {
                    pole[i, j] = kryci;
                }
            }

            return pole;
        }
    }
}
