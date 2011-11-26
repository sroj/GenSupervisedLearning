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
                IFitnessFunction f = new FitnessFunction(examples, true);
                ISelectionMethod roulette = new RouletteWheelSelection();
                ISelectionMethod rank = new RankSelection();
                WAPChromosome c = new WAPChromosome(5 * 44);
                WAPChromosome legend = c;
                WAPPopulation pop = new WAPPopulation(10, c, f, roulette);
                pop.RandomSelectionPortion = 0.2;
                pop.MutationRate = 0.0;
                pop.parallelism = true;

                Console.WriteLine("Entrenando: selectP:{0} mutacion:{1}", pop.RandomSelectionPortion, pop.MutationRate);
                for (int i = 0; i < maxIter; i++)
                {
                    pop.RunEpoch();
                    int prom = 0;
                    for (int j = 0; j < pop.Size; j++)
                        prom += ((WAPChromosome)pop[j]).numRules;
                    prom = prom / pop.Size;
                    Console.WriteLine("Generacion {0} Mejor Fitness {1} prom: {2}, reglas:{3}", i, pop.BestChromosome.Fitness, pop.FitnessAvg, prom);
                    if (pop.BestChromosome.Fitness > legend.Fitness)
                        legend = (WAPChromosome)pop.BestChromosome;
                }
                //Console.WriteLine(pop.BestChromosome);
                Console.WriteLine("Mejor Cromosoma de la ultima generacion:");
                Console.WriteLine(pop.BestChromosome.Fitness);
                Console.WriteLine(Math.Sqrt(pop.BestChromosome.Fitness) + (double)((WAPChromosome)pop.BestChromosome).numRules / (double)examples.Length / 2.0);


                //Console.WriteLine(legend);
                Console.WriteLine("\nMejor Cromosoma de todas las generaciones:");
                Console.WriteLine(legend.Fitness);
                Console.WriteLine(Math.Sqrt(legend.Fitness) + (double)((WAPChromosome)legend).numRules / (double)examples.Length / 2.0); 
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
