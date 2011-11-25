using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;


namespace GenSupervisedLearning
{
    public class WAPChromosome : ShortArrayChromosome
    {
        private int nRules;
        private static int RULE_LENGTH = 44;

        public int numRules
        {
            get { return nRules; }
        }

        public WAPChromosome(int length)
            : base(length, 1)
        {
            if (length % 44 != 0) throw new Exception("Tamaño invalido para un cromosoma, debe ser multiplo de 44.");
            nRules = length / 44;
        }

        public override void Crossover(IChromosome pair)
        {
            Console.WriteLine("En metodo Crossover correcto");

            //Arreglos originales antes de hacer crossover
            ushort[] pairArray = ((WAPChromosome)pair).val;
            ushort[] thisArray = this.val;

            //Arreglos resultantes despues de hacer crossover
            ushort[] XpairArray;
            ushort[] XthisArray;

            //Puntos de crossover en el cromosoma this
            int n0 = rand.Next(1, thisArray.Length);
            int n1 = rand.Next(1, thisArray.Length);

            //Puntos de crossover en el cromosoma pair
            int p0;
            int p1;

            //Distancias al inicio de la regla inmediata a la izquierda (ver Mitchell)
            int d0;
            int d1;

            if (n0 > n1)
            {
                int swap = n0;
                n0 = n1;
                n1 = swap;
            }

            d0 = n0 % RULE_LENGTH;
            d1 = n1 % RULE_LENGTH;
            List<KeyValuePair<int, int>> lista = new List<KeyValuePair<int, int>>();
            int i = d0;
            int j;
            int k = 0, l = 0;

            //Calculo de todos los pares validos en cromosoma pair (ver Mitchell)
            while (i < pairArray.Length)
            {
                j = d1 + k * RULE_LENGTH;
                l = k;
                while (j < pairArray.Length)
                {
                    lista.Add(new KeyValuePair<int, int>(i, j));
                    j += RULE_LENGTH;
                }
                i += RULE_LENGTH;
                ++k;
            }

            KeyValuePair<int, int> pairCrossoverPoints = lista.ElementAt(rand.Next(0, lista.Count));
            p0 = pairCrossoverPoints.Key;
            p1 = pairCrossoverPoints.Value;

            //Usando n0, n1, p0 y p1 hacer efectivamente el crossover entre this y pair
            int XthisArraySize = n0 + p1 - p0 + thisArray.Length - n1;
            int XpairArraySize = p0 + n1 - n0 + pairArray.Length - p1;
            XthisArray = new ushort[XthisArraySize];
            XpairArray = new ushort[XpairArraySize];

            //Creacion de XthisArray
            int offset = 0;
            for (int n = 0; n < XthisArraySize; n++)
            {
                if (n < n0)
                    XthisArray[n] = thisArray[n];
                else if (n >= p0 && n < p1)
                    XthisArray[n] = pairArray[n + p0 - 1];
                else
                {
                    XthisArray[n] = thisArray[n1 - 1 + offset];
                    offset++;
                }
            }

            //Creacion de XpairArray
            offset = 0;
            for (int n = 0; n < XpairArraySize; n++)
            {
                if (n < p0)
                    XpairArray[n] = pairArray[n];
                else if (n >= n0 && n < n1)
                    XpairArray[n] = thisArray[n + n0 - 1];
                else
                {
                    XpairArray[n] = pairArray[p1 - 1 + offset];
                    offset++;
                }
            }

            //Falta revisar si no hace daño acceder directamente
            //a la representacion de WAPChromosome
            this.val = XthisArray;
            this.length = XthisArraySize;

            ((WAPChromosome)pair).val = XpairArray;
            ((WAPChromosome)pair).length = XpairArraySize;
        }
    }
}
