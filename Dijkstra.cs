using System;
using System.Collections;
using System.Collections.Generic;
using static MolaDirectedGraph;
using System.Linq;

namespace Mola
{
    public class Dijkstra
    {

        public float[] DistanceToAll(MolaDirectedGraph graph, int start)
        {
            float[] distances = new float[graph.NodesCount()];
            int[] candidates = new int[graph.NodesCount()];
            int[] nextCandidates = new int[graph.NodesCount()];
            candidates[0] = start;
            distances = Enumerable.Repeat((float)10000, graph.NodesCount()).ToArray();

            distances[start] = 0;
            int amountOfCandidates = 1;
            while (amountOfCandidates > 0)
            {
                int nextAmountOfCandidates = 0;
                //Debug.Log("amountOfCandidates: " + amountOfCandidates);
                for (int i = 0; i < amountOfCandidates; i++)
                {
                    int candidate = candidates[i];
                    float currentWeight = distances[candidate];
                    IEnumerable<DirectedEdge> cNbs = graph.GetDirectedEdges(candidate);
                    foreach (DirectedEdge edge in cNbs)
                    {
                        int nb = edge.n2;
                        if (nb >= 0)
                        {
                            float nextCost = edge.weight + currentWeight;
                            if (nextCost < distances[nb])
                            {
                                distances[nb] = nextCost;
                                nextCandidates[nextAmountOfCandidates] = nb;
                                nextAmountOfCandidates++;
                            }
                        }
                    }
                }
                for (int i = 0; i < nextAmountOfCandidates; i++)
                {
                    candidates[i] = nextCandidates[i];
                }
                amountOfCandidates = nextAmountOfCandidates;
            }
            return distances;
        }
    }
}