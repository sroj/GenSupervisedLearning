using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;
using System.Threading;

namespace GenSupervisedLearning
{
    public class FitnessFunction : IFitnessFunction
    {

        private int[][] trainingSet;
        private bool lengthPunishment;
        private int trainingSetSize;
        private const int MIN_AGE = 15;

        //El argumento trainingSet no esta codificado a GABIL.
        public FitnessFunction(int[][] trainingSet, bool lengthPunishment)
        {
            this.trainingSet = trainingSet;
            this.lengthPunishment = lengthPunishment;
            this.trainingSetSize = trainingSet.Length;
        }

        public double Evaluate(IChromosome c)
        {
            WAPChromosome wapC = (WAPChromosome)c;

            //El mejor numero para dos Cores, crea 2 threads:
            const int works = 800;

            Thread[] threads = new Thread[(int)Math.Ceiling((double)trainingSet.Length / (double)works)];
            testExamplesContext[] contexts = new testExamplesContext[threads.Length];


            for (int t = 0; t < threads.Length; t++)
                threads[t] = new Thread(this.testExamples);

            for (int t = 0; t < threads.Length; t++)
            {
                contexts[t] = new testExamplesContext(wapC, t * works, Math.Min((t+1)*works, trainingSet.Length));
                threads[t].Start(contexts[t]);
            }

            int classified = 0;
            for (int t = 0; t < threads.Length; t++)
            {
                threads[t].Join();

                //Penalizacion por postcondicion semánticamente incorrecta.
                if (contexts[t].classified < 0)
                    return 0.00000001;
                else
                    classified += contexts[t].classified;
            }
            
            double lp = (lengthPunishment ? (double)((WAPChromosome)c).numRules / (double)trainingSetSize : 0.0);
            return Math.Pow(Math.Max((double)classified / (double)trainingSetSize - lp / 2.0, 0.0), 2);
        }

        private class testExamplesContext
        {
            public WAPChromosome chromosome; public int begin; public int end; public int classified;
            public testExamplesContext(WAPChromosome chromosome, int begin, int end)
            {
                this.chromosome = chromosome; this.begin = begin; this.begin = begin; this.end = end; this.classified = 0;
            }
        }

        private void testExamples(object tec)
        {
            Thread.Sleep(0);
            bool preOK;
            testExamplesContext TEC = (testExamplesContext) tec;
            WAPChromosome c = TEC.chromosome;
            int begin = TEC.begin;
            int end   = TEC.end;
            ushort[] hypothesis = c.Value;

            TEC.classified = 0;

            for (int i = begin; i < end; i++)
            {
                int[] example = trainingSet[i];
                for (int j = 0; j < c.numRules; j++)
                {
                    //Indice base de la regla actual:
                    int b = j * WAPChromosome.RULE_LENGTH;
                    //Penalizacion por postcondicion semánticamente incorrecta.
                    if (hypothesis[41 + b] + hypothesis[42 + b] + hypothesis[43 + b] != 1)
                    {
                        TEC.classified = -1;
                        return;
                    }

                    preOK = true;
                    for (int k = 0; k < 9 && preOK; k++)
                    {
                        switch (k)
                        {
                            case 0:
                                //Con edades de 15 a 54 el rango queda de 0 a 7
                                int age_cod = (int)(Math.Floor((double)(example[0] - MIN_AGE)) / 5);
                                //Rango de precondicion 0<=i<=7 con edad = i*5+15 
                                if (hypothesis[0 + age_cod + b] == 0)
                                    preOK = false;
                                break;
                            case 1:
                                //Rango de precondicion 8<=i<=11 con educacion_esposa = i - 7
                                if (hypothesis[7 + example[1] + b] == 0)
                                    preOK = false;
                                break;
                            case 2:
                                //Rango de precondicion 12<=i<=15 con educacion_esposo = i - 11
                                if (hypothesis[11 + example[2] + b] == 0)
                                    preOK = false;
                                break;
                            case 3:
                                //Rango de precondicion 16<=i<=26 con hijos = i - 16
                                // (hypothesis[16 + (example[3] < 11 ? example[3] : 11) + b] == 0)
                                if (hypothesis[16 + (example[3] < 11 ? example[3] : 10) + b] == 0)
                                    preOK = false;
                                break;
                            case 4:
                                //Rango de precondicion 27<=i<=28 con religion = i - 27
                                if (hypothesis[27 + example[4] + b] == 0)
                                    preOK = false;
                                break;
                            case 5:
                                //Rango de precondicion 29<=i<=30 con esposa_empleada = i - 29
                                if (hypothesis[29 + example[5] + b] == 0)
                                    preOK = false;
                                break;
                            case 6:
                                //Rango de precondicion 31<=i<=34 con ocupacion_esposo = i - 30
                                if (hypothesis[30 + example[6] + b] == 0)
                                    preOK = false;
                                break;
                            case 7:
                                //Rango de precondicion 35<=i<=38 con standard_vida = i - 34
                                if (hypothesis[34 + example[7] + b] == 0)
                                    preOK = false;
                                break;
                            case 8:
                                //Rango de precondicion 39<=i<=40 con medios = i - 39
                                if (hypothesis[39 + example[8] + b] == 0)
                                    preOK = false;
                                break;
                        }
                    }

                    if (preOK)
                    {
                        //Rango de postcondicion 41<=i<=43 con metodo = i - 40
                        if (hypothesis[40 + example[9] + b] == 1)
                        {
                            TEC.classified++;
                            break;
                        }
                    }
                } //End Rules For
            }//End examples For
        }



    }
}