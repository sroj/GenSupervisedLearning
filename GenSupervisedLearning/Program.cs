using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AForge.Genetic;

namespace GenSupervisedLearning
{
    class Program
    {

        static void readFile(String fileName, int[][] examples)
        {
            StreamReader s = new StreamReader(fileName);
            String line;
            String[] tokens;
            char[] delimiterChars = { ',' };
            int i = 0;
            while ((line = s.ReadLine()) != null)
            {
                tokens = line.Split(delimiterChars);
                examples[i] = new int[tokens.Length];

                for (int j = 0; j < tokens.Length; j++)
                {
                    examples[i][j] = int.Parse(tokens[j]);
                }
                i++;
            }
            s.Close();
        }

        static void Main(string[] args)
        {
            try
            {
                int numLines = 1473;
                int maxIter = 1000;

                int[][] examples = new int[numLines][];
                readFile("cmc.data", examples);
                IFitnessFunction f = new FitnessFunction(examples, false);
                ISelectionMethod roulette = new RouletteWheelSelection();
                ISelectionMethod rank = new RankSelection();

                IChromosome c = new WAPChromosome(44 * 2);
                IChromosome c1 = new WAPChromosome(44 * 2);
                Console.WriteLine(c);
                Console.WriteLine(c1);
                c.Crossover(c1);
                /*
            Population pop = new Population(500, c, f, roulette);
            for (int i = 0; i < maxIter; i++)
                pop.RunEpoch();
            Console.WriteLine(pop.BestChromosome);
            Console.WriteLine(pop.BestChromosome.Fitness);
            Console.WriteLine(Math.Sqrt(pop.BestChromosome.Fitness));            
                  */            




            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
