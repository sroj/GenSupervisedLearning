using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;


namespace GenSupervisedLearning
{
    public class WAPChromosome : ShortArrayChromosome
    {
        public static int Size = 44;
        private int nRules;

        public int numRules
        {
            get { return nRules; }
        }

        public WAPChromosome(int length)
            : base(length, 1)
        {
            if (length % Size != 0) throw new Exception("Tamaño invalido para un cromosoma, debe ser multiplo de 44.");
            nRules = length / Size;
        }


        /* Initializes a new instance of the WAPChromosome class.
         * This is a copy constructor, which creates the exact copy of specified chromosome.
         */
        protected WAPChromosome(WAPChromosome source) : base(source)
        {
            maxValue = source.maxValue;
        }

        public override void Crossover(IChromosome pair)
        {
            //Console.WriteLine("Crossover llamado a {0} con {1}", GetHashCode(), pair.GetHashCode());
            base.Crossover(pair);
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
            return base.ToString();
        }
    }
}
