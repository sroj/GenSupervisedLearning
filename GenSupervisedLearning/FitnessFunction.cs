using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;

namespace GenSupervisedLearning
{
    class FitnessFunction : IFitnessFunction
    {

        private int[][] trainingSet;
        private bool p;
        private int trainingSetSize;
        private const int MIN_AGE = 15;

        //El argumento trainingSet no esta codificado a GABIL.
        public FitnessFunction(int[][] trainingSet, bool lengthPunishment)
        {
            this.trainingSet = trainingSet;
            this.p = lengthPunishment;
            this.trainingSetSize = trainingSet.Length;
        }

        public double Evaluate(IChromosome c)
        {            
            int classified = 0;
            int[] example;
            ushort[] hypothesis = ((ShortArrayChromosome)c).Value;
            bool preOK;
            int numRules = ((ShortArrayChromosome)c).Length / 44;

            for (int i = 0; i < trainingSet.Length; i++)
            {
                example = trainingSet[i];
                
                for (int j = 0; j < numRules; j++)
                {
                    //Penalizacion por postcondicion semánticamente incorrecta.
                    if (hypothesis[41 + j * 44] + hypothesis[42 + j * 44] + hypothesis[43 + j * 44] != 1)
                        return 0.00000001;

                    preOK = true;
                    for (int k = 0; k < 9 && preOK; k++)
                    {
                        switch (k)
                        {
                            case 0:
                                if (hypothesis[(int)(Math.Floor((double)(example[0] - MIN_AGE)) / 5) + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 1:
                                if (hypothesis[7 + example[1] + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 2:
                                if (hypothesis[11 + example[2] + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 3:
                                if (hypothesis[16 + (example[3] < 11 ? example[3] : 11) + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 4:
                                if (hypothesis[27 + example[4] + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 5:
                                if (hypothesis[29 + example[5] + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 6:
                                if (hypothesis[30 + example[6] + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 7:
                                if (hypothesis[34 + example[7] + j * 44] == 0)
                                    preOK = false;
                                break;
                            case 8:
                                if (hypothesis[39 + example[8] + j * 44] == 0)
                                    preOK = false;
                                break;
                        }
                    }

                    if (preOK)
                        if (hypothesis[40 + example[9] + j * 44] == 1)
                        {
                            classified++;
                            break;
                        }
                }
            }
            return Math.Pow((double)classified / (double)trainingSetSize, 2);
        }
    }
}