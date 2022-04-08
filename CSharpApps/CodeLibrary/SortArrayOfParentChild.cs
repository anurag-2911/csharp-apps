using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLibrary
{
    class SortArrayOfParentChild
    {
        public void Test()
        {
            WrkSet baap = new WrkSet { name ="baap",
                                       MasterItemID = "ffe5722244b2a0999d8342fdaef358bf", 
                                       CurrentItemID = "ffe5722244b2a0999d8342fdaef358bf", 
                                       ImmedeateMasterID = "ffe5722244b2a0999d8342fdaef358bf"
            };
            WrkSet beta = new WrkSet { name="beta",
                                       MasterItemID = "ffe5722244b2a0999d8342fdaef358bf", 
                                       CurrentItemID = "089ad6a26bed7de1c1b2ec1d864e027d", 
                                       ImmedeateMasterID = "ffe5722244b2a0999d8342fdaef358bf"
                                     };
            WrkSet pota = new WrkSet { name = "pota",
                                       MasterItemID = "ffe5722244b2a0999d8342fdaef358bf", 
                                       CurrentItemID = "dc1bd9b0477d9796d6c31bf0c52778f3", 
                                       ImmedeateMasterID = "089ad6a26bed7de1c1b2ec1d864e027d"
                                     };
            WrkSet perpota = new WrkSet
            {
                name = "perpota",
                MasterItemID = "ffe5722244b2a0999d8342fdaef358bf",
                CurrentItemID = "zc1bd9b0477d9796d6c31bf0c52778f3",
                ImmedeateMasterID = "dc1bd9b0477d9796d6c31bf0c52778f3"
            };

            WrkSet[] wrkSets = new WrkSet[] { perpota,beta,pota,baap };
            InsertionSort(wrkSets);
            //List<WrkSet> list = new List<WrkSet>(wrkSets);
            //BuildTree(list);



        }
        static List<WrkSet> BuildTree(List<WrkSet> nodes)
        {
            var nodeMap = nodes.ToDictionary(node => node.CurrentItemID);
            var rootNodes = new List<WrkSet>();
            foreach (var node in nodes)
            {
                WrkSet parent;
                if (nodeMap.TryGetValue(node.ImmedeateMasterID, out parent))
                    parent.children.Add(node);
                else
                    rootNodes.Add(node);
            }
            return rootNodes;
        }

        private WrkSet[] Sort(WrkSet[] wrkSets)
        {
            WrkSet[] sorted = new WrkSet[wrkSets.Length];
            var ordered = wrkSets
                .Where(p => (p.MasterItemID == p.CurrentItemID))
                .OrderBy(p => p.CurrentItemID)
                .Select(p => wrkSets
                    .Where(c => c.ImmedeateMasterID == p.CurrentItemID)
                    .OrderBy(c => c.CurrentItemID))
                .ToList();

            return sorted;
        }

        public void InsertionSort(WrkSet[] wrkSets)
        {
            int input, output;
            for (output = 1; output < wrkSets.Length; output++) // out is dividing line
            {
                WrkSet temp = wrkSets[output]; // remove marked item
                input = output; // start shifts at out
                while (input > 0 && wrkSets[input].CurrentItemID  != temp.ImmedeateMasterID) // until one is smaller,
                {
                    wrkSets[input] = wrkSets[input - 1]; // shift item right,
                    --input; // go left one position
                }
                wrkSets[input] = temp; // insert marked item
            }// end for

        } // end insertionSort()
    } 

    internal class WrkSet
    {
        public string MasterItemID;
        public string CurrentItemID;
        public string ImmedeateMasterID;
        public string name;
        public List<WrkSet> children = new List<WrkSet>();
        public override string ToString()
        {
            return name + " : " + CurrentItemID + " : " + ImmedeateMasterID + " : " + MasterItemID;
        }

    }

   

}
