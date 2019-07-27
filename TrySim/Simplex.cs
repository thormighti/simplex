using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrySim
{
  public  class Simplex
    {
        private double[] objectiveFunc; //objective fucntion
        private double[,] A;
        private double[] decisionConstraints;
        private List<int> NonBasicVar = new List<int>();   // storing the non basic variables
        private List<int> BasicVar = new List<int>();   // for storing the basic variable
        private double v = 0; // to store the optimal value
        
        public Simplex(double[] objectiveFunc, double[,] A, double[] decisionConstraints)   // constructr
        {
            int lengthOfObj = objectiveFunc.Length;
             int   lengthOfConstraint = decisionConstraints.Length;

            if (lengthOfObj != A.GetLength(1))
            {
                throw new Exception("Number of variables in c doesn't match number in A.");
            }
            
            if (lengthOfConstraint != A.GetLength(0))
            {
                throw new Exception("Number of constraints in A doesn't match number in Decision Constraints.");
            }

            // Extending the Obj coefficients vector with 0 padding
            this.objectiveFunc = new double[lengthOfObj + lengthOfConstraint];
            Array.Copy(objectiveFunc, this.objectiveFunc, lengthOfObj); // copying the element of c into the refrence, remaining length taken as zero. observes slack number == b.lenth
            
            // Extending the Variables abi A coefficient matrix with 0 padding
            this.A = new double[lengthOfObj + lengthOfConstraint, lengthOfObj + lengthOfConstraint];
            for (int i = 0; i < lengthOfConstraint; i++)
            {
                for (int j = 0; j < lengthOfObj; j++)
                {
                    this.A[i + lengthOfObj, j] = A[i, j];  // values of A stored in the exttension
                }
            }

            // Extending the constraint right-hand side vector with 0 padding
            this.decisionConstraints = new double[lengthOfObj + lengthOfConstraint];
            Array.Copy(decisionConstraints, 0, this.decisionConstraints, lengthOfObj, lengthOfConstraint); // extend from element index at 0 to index[obj length], with b.lenth as the length
            
            // Populating the non-basic and basic sets
            for (int i = 0; i < lengthOfObj; i++) // chek out why leobj
            {
                NonBasicVar.Add(i); /// [0] - ..... of the obj
            }
            
            for (int i = 0; i < lengthOfConstraint; i++) // consraints holds the decision var.
            {
                BasicVar.Add(lengthOfObj + i); // [lenghtobj + i]..... decision amounts
            }
        }
        private void Calculations(int enteringPoint, int leavingPoint)
        {
            NonBasicVar.Remove(enteringPoint);
            BasicVar.Remove(leavingPoint); /// mantaining the size of the variables

            decisionConstraints[enteringPoint] = decisionConstraints[leavingPoint] / A[leavingPoint, enteringPoint]; //  b of the pivot divide by pivot

            foreach (var j in NonBasicVar)
            {
                A[enteringPoint, j] = A[leavingPoint, j] / A[leavingPoint, enteringPoint]; // new var coff , dividing through by pivot elemet for nonbasic var
            }

            A[enteringPoint, leavingPoint] = 1 / A[leavingPoint, enteringPoint];  //  making the pivot element in unity , thats == 1; check this lera

            foreach (var i in BasicVar) 
            {
                decisionConstraints[i] -= A[i, enteringPoint] * decisionConstraints[enteringPoint]; // storing the new be's I multiplied the element in the pivot colomn by the pivot b

                foreach (var j in NonBasicVar) 
                {                                                 // nested loop here
                    A[i, j] -=  A[i, enteringPoint] * A[enteringPoint, j];   //doing the calculations
                }

            
            }

            v  += objectiveFunc[enteringPoint] * decisionConstraints[enteringPoint];   //   sores the objective function

            foreach (var j in NonBasicVar)
            {
                objectiveFunc[j] = objectiveFunc[j] - objectiveFunc[enteringPoint] * A[enteringPoint, j];
            }

      

            NonBasicVar.Add(leavingPoint);
            BasicVar.Add(enteringPoint);
        }
        public Tuple<double, double[]> Minimization()
        {
            while (true)
            {
                // Find highest coefficient for entering var
                int enteringPoint = -1; // entering location cant be negative, setting initial to -1;
                double ce = 0;
                foreach (int j in NonBasicVar) // finding the coffecient of the nonbasic variables in objfunc. j is d item index
                {
                    if (objectiveFunc[j] < ce)  // finds the most negative
                    {
                        ce = objectiveFunc[j];
                        enteringPoint = j;                           // e becomes the entering index
                    }
                }

                // If no coefficient < 0, there's no more mainimizing to do, and we're almost done
                if (enteringPoint == -1) break;   // if e == -1break.no negative index

                // Find lowest check ratio
                double minRatio = double.MaxValue; // setting to a very large number. Oko help
                int leavingPoint = -1;
                foreach (int i in BasicVar) // at the de decision constraints
                {
                    if (A[i, enteringPoint] > 0)  // element must be positive
                    {
                        double ratio = decisionConstraints[i] / A[i, enteringPoint];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            leavingPoint = i;
                        }
                    }
                }

                //checking for  Unboundedness
                if (minRatio < 0)
                {
                    Console.WriteLine("the solution is infeasible and unbounded");
                }

                Calculations(enteringPoint, leavingPoint);   // setting the pivot
            }

            // Extract amounts and slack for optimal solution
            double[] optimalAmounts = new double[decisionConstraints.Length];
            int n = decisionConstraints.Length;
            for (var i = 0; i < n; i++)
            {
                optimalAmounts[i] = BasicVar.Contains(i) ? decisionConstraints[i] : 0; // optimal checks returns the basic var
            }

            // Return the minimization and  the variables
            return Tuple.Create(v, optimalAmounts);
        }

        
        public Tuple<double, double[]> Mazimization() // need to return two values
        {
            while (true)
            {
                // Find highest coefficient for entering var
                int enteringPoint = -1; // standardizing the obj func
                double ce = 0;
                foreach (int j in NonBasicVar)
                {
                    if (objectiveFunc[j] > ce)   // finds the most positive
                    {
                        ce = objectiveFunc[j];
                        enteringPoint = j;
                    }
                }

                // If no coefficient > 0, there's no more maximizing to do, and we're almost done
                if (enteringPoint == -1)
                {
                    break;
                }

                // Find lowest check ratio
                double minRatio = double.MaxValue;
                int leavingPoint = -1;
                foreach (int i in BasicVar)
                {
                    if (A[i, enteringPoint] > 0)
                    {
                        double ratio = decisionConstraints[i] / A[i, enteringPoint];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            leavingPoint = i;
                        }
                    }
                }

                //checking for  Unboundedness
                if (minRatio < 0)
                {
                    Console.WriteLine("the solution is infeasible and unbounded");
                }

                Calculations(enteringPoint, leavingPoint);
            }

            // Extract amounts and slack for optimal solution
            double[] optimalAmounts = new double[decisionConstraints.Length];
            int n = decisionConstraints.Length;
            for (var i = 0; i < n; i++)
            {
                optimalAmounts[i] = BasicVar.Contains(i) ? decisionConstraints[i] : 0; // extracting our slack and surplus
            }
           
            // Return minimization and variables
            return Tuple.Create(v, optimalAmounts); // returns the objective and and array of the answers
        }

    }
}
