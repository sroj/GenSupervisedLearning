using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;


namespace GenSupervisedLearning
{
    public class WAPChromosome : ShortArrayChromosome
    {
        public static int RULE_LENGTH = 44;
        private int nRules;

        public int numRules
        {
            get { return nRules; }
        }

        public WAPChromosome(int length)
            : base(length, 1)
        {
            if (length % RULE_LENGTH != 0) throw new Exception("Tamaño invalido para un cromosoma, debe ser multiplo de 44.");
            nRules = length / RULE_LENGTH;
        }


        /* Initializes a new instance of the WAPChromosome class.
         * This is a copy constructor, which creates the exact copy of specified chromosome.
         */
        protected WAPChromosome(WAPChromosome source) : base(source)
        {
            maxValue = source.maxValue;
            nRules   = source.nRules;
        }

        public virtual void DropCondition()
        {
            int rule = rand.Next(0, nRules);
            int atribute = rand.Next(0, 9);
            int begin = 0, end = 0;

            switch (atribute)
            {
                case 0:
                    end = 8;
                    break;
                case 1:
                    begin = 8;
                    end = 12;
                    break;
                case 2:
                    begin = 12;
                    end = 16;
                    break;
                case 3:
                    begin = 16;
                    end = 27;
                    break;
                case 4:
                    begin = 27;
                    end = 29;
                    break;
                case 5:
                    begin = 29;
                    end = 31;
                    break;
                case 6:
                    begin = 31;
                    end = 35;
                    break;
                case 7:
                    begin = 35;
                    end = 39;
                    break;
                case 8:
                    begin = 39;
                    end = 41;
                    break;
            }

            int b = rule * RULE_LENGTH;
            for (; begin < end; begin++)
            {
                val[b + begin] = 1;
            }
        }

        public virtual void AddAlternative()
        {
            int index = rand.Next(0, length);
            int mod, real_index;
            for (int i = 0; i < length; i++, index++)
            {
                mod = index % RULE_LENGTH;
                real_index = index % length;
                if (mod < 41 && val[real_index] == 0)
                {
                    val[real_index] = 1;
                    break;
                }
            }
        }

        public override void Crossover(IChromosome pair)
        {
            WAPChromosome wap_pair = (WAPChromosome)pair;

            //Arreglos originales antes de hacer crossover
            ushort[] pairArray = wap_pair.Value;
            ushort[] thisArray = this.Value;
            //Arreglos resultantes despues de hacer crossover
            ushort[] XpairArray;
            ushort[] XthisArray;

            //Puntos de crossover en el cromosoma this
            int n0 = rand.Next(1, thisArray.Length);
            int n1 = rand.Next(1, thisArray.Length);
            n0 = Math.Min(n0, n1);
            n1 = Math.Max(n0, n1);

            //Distancias al inicio de la regla inmediata a la izquierda (ver Mitchell)
            int d0 = n0 % RULE_LENGTH;
            int d1 = n1 % RULE_LENGTH;

            //Regla inicial y regla final en las cuales se picarán los cromosomas:
            int ini_rule = rand.Next(0, wap_pair.nRules);
            int fin_rule = rand.Next(0, wap_pair.nRules);
            ini_rule = Math.Min(ini_rule, fin_rule);
            fin_rule = Math.Max(ini_rule, fin_rule);

            //Puntos de crossover en el cromosoma pair (p0 puede ser mayor que p1)
            int p0 = ini_rule * RULE_LENGTH + d0;
            int p1 = fin_rule * RULE_LENGTH + d1;
            bool inv = p0 >= p1;

            //Usando n0, n1, p0 y p1 hacer efectivamente el crossover entre this y pair
            int pair_cs = p1 - p0 + (inv? RULE_LENGTH : 0);
            XthisArray = new ushort[n0 + pair_cs + (thisArray.Length - n1)];
            XpairArray = new ushort[pairArray.Length - (p1 - p0) + (n1 - n0)];
            
            try
            {
                //Aisgnacion a el arreglo de este cromosoma:
                Array.Copy(thisArray, 0, XthisArray, 0, n0);
                if (inv)
                {
                    Array.Copy(pairArray, p0, XthisArray, n0, RULE_LENGTH - d0);
                    Array.Copy(pairArray, p1 - d1, XthisArray, n0 + RULE_LENGTH - d0, d1);
                }
                else
                    Array.Copy(pairArray, p0, XthisArray, n0, pair_cs);
                Array.Copy(thisArray, n1, XthisArray, n0 + pair_cs, thisArray.Length - n1);

                //Aisgnacion a el arreglo del cromosoma par:
                Array.Copy(pairArray, 0, XpairArray, 0, p0);
                Array.Copy(thisArray, n0, XpairArray, p0, (n1 - n0));
                Array.Copy(pairArray, p1, XpairArray, p0 + (n1 - n0), pairArray.Length - p1);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("n0:{0}, n1:{1}, d0:{2}, d1:{3}, p0:{4}, p1:{5}, pcs:{6}.", n0, n1, d0, d1, p0, p1, pair_cs);
                Console.WriteLine("XthisArrayL:{0}, XpairArrayL:{1}.", XthisArray.Length, XpairArray.Length);
                throw e;
            }

            this.setValue(XthisArray);
            wap_pair.setValue(XpairArray);
        }

        protected void setValue(ushort[] value)
        {
            val = value;
            length = value.Length;
            nRules = length / RULE_LENGTH;
        }

        public override IChromosome CreateNew()
        {
            WAPChromosome n = new WAPChromosome(length);
            return n;
        }
        
        public override IChromosome Clone()
        {
            return new WAPChromosome(this);
        }

        //Hacer que imprima cada regla por separado.
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(length + numRules);
            for (int r = 0; r < numRules; r++)
            {
                for (int i = 0; i < RULE_LENGTH; i++)
                {
                    sb.Append(val[r*RULE_LENGTH + i]);
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }

        public string StringMarcas(int m0, int m1)
        {
            StringBuilder sb = new StringBuilder(length + numRules + 2);
            for (int r = 0; r < numRules; r++)
            {
                for (int i = 0; i < RULE_LENGTH; i++)
                {
                    int index = r * RULE_LENGTH + i;
                    if (index == m1) sb.Append(']');
                    if (index == m0) sb.Append('[');
                    sb.Append(val[index]);
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }
}
