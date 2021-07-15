using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class BlocksAssembly : MonoBehaviour
    {
        [Serializable]
        private class IncompatiblePair
        {
            public ColorKinds color1;
            public ColorKinds color2;
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private ColorBlock[] _colorBlocks;
        [SerializeField] private IncompatiblePair[] _incompatiblesColors;

        //==================================================
        // Methods
        //==================================================

        public void ConfigureColorBlocks(out ColorBlock block1, out ColorBlock block2)
        {
            List<ColorBlock> blocks = new List<ColorBlock>(_colorBlocks);
            block1 = ExtractRandomColorBlock(blocks);

            List<ColorKinds> removedColors = new List<ColorKinds>();            
            foreach (IncompatiblePair item in _incompatiblesColors)
            {
                if (block1.Kind == item.color1 || block1.Kind == item.color2)
                {
                    removedColors.Add(block1.Kind == item.color1 ? item.color2 : item.color1);
                }
            }
            
            List<ColorBlock> matchedBlocks = blocks.Where(x => !removedColors.Contains(x.Kind)).ToList();
            
            block2 = ExtractRandomColorBlock(matchedBlocks);
        }
                
        private ColorBlock ExtractRandomColorBlock(List<ColorBlock> blocks)
        {
            int index = UnityEngine.Random.Range(0, blocks.Count);
            ColorBlock target = blocks[index];
            blocks.RemoveAt(index);

            return target;
        }
    }
}