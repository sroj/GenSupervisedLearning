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
            
        }
    }
}
