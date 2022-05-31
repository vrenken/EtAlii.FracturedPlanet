namespace EtAlii.FracturedPlanet.Navigation
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Random = UnityEngine.Random;

    public class HeightMapBuilder
    {
        public HeightMap Build()
        {
            var structure = new int[Sector.TerrainVoxels * Sector.TerrainVoxels];

            for (var i = 0; i < 30; i++)
            {
                IterateInsideOut(structure);
                IterateOutsideIn(structure);
            }
            
            return new HeightMap(structure);
        }

        private void IterateInsideOut(int[] structure)
        {
            Debug.Assert(Sector.TerrainVoxels % 2 == 0, $"Only even sector sizes are supported by the {nameof(IterateInsideOut)} algorithm." );

            var positionX = Sector.TerrainVoxels / 2; // The middle of the matrix
            // ReSharper disable once UnreachableCode
            var positionY = Sector.TerrainVoxels % 2 == 0 ? (Sector.TerrainVoxels / 2) - 1 : (Sector.TerrainVoxels / 2);

            var direction = 0; // The initial direction is "down"
            var stepsCount = 1; // Perform 1 step in current direction
            var stepPosition = 0; // 0 steps already performed
            var stepChange = 0; // Steps count changes after 2 steps

            for (var i = 0; i < Sector.TerrainVoxels * Sector.TerrainVoxels; i++)
            {
                // Fill the current cell with the current value
                var average = GetAverage(structure, positionX, positionY, Sector.TerrainVoxels);
                var offset = (Random.value - 1f) * 2;
                structure[positionX + positionY * Sector.TerrainVoxels] = (int)(average + offset);

                // Check for direction / step changes
                if (stepPosition < stepsCount)
                {
                    stepPosition++;
                }
                else
                {
                    stepPosition = 1;
                    if (stepChange == 1)
                    {
                        stepsCount++;
                    }
                    stepChange = (stepChange + 1) % 2;
                    direction = (direction + 1) % 4;
                }

                // Move to the next cell in the current direction
                switch (direction)
                {
                    case 0: positionY++; break;
                    case 1: positionX--; break;
                    case 2: positionY--; break;
                    case 3: positionX++; break;
                }
            }
        }
        private void IterateOutsideIn(int[] structure)
        {
            for (int i = Sector.TerrainVoxels - 1, j = 0; i > 0; i--, j++)
            {
                for (var k = j; k < i; k++)
                {
                    var position = j + k * Sector.TerrainVoxels;
                    var average = GetAverage(structure, j, k, Sector.TerrainVoxels);
                    var offset = (Random.value - 1f) * 2;
                    structure[position] = (int)(average + offset);
                }

                for (var k = j; k < i; k++)
                {
                    var position = k + i * Sector.TerrainVoxels;
                    var average = GetAverage(structure, k, i, Sector.TerrainVoxels);
                    var offset = (Random.value - 1f) * 2;
                    structure[position] = (int)(average + offset);
                }

                for (var k = i; k > j; k--)
                {
                    var position = i + k * Sector.TerrainVoxels;
                    var average = GetAverage(structure, i, k, Sector.TerrainVoxels);
                    var offset = (Random.value - 1f) * 2;
                    structure[position] = (int)(average + offset);
                }

                for (var k = i; k > j; k--)
                {
                    var position = k + j * Sector.TerrainVoxels;
                    var average = GetAverage(structure, k, j, Sector.TerrainVoxels);
                    var offset = (Random.value - 1f) * 2;
                    structure[position] = (int)(average + offset);
                }
            }
        }

        private float GetAverage(IReadOnlyList<int> structure, int x, int y, int size)
        {
            float value = structure[x + y * size];
            
            var leftPosition = x - 1;
            if (leftPosition >= 0)
            {
                value = (value + structure[leftPosition + y * size]) / 2f;
            }
            var rightPosition = x + 1;
            if (rightPosition < size)
            {
                value = (value + structure[rightPosition + y * size]) / 2f;
            }
            
            var topPosition = y - 1;
            if (topPosition >= 0)
            {
                value = (value + structure[x + topPosition * size]) / 2f;
            }
            
            var bottomPosition = y + 1;
            if (bottomPosition < size)
            {
                value = (value + structure[x + bottomPosition * size]) / 2f;
            }

            return value;
        }
    }
}