using System.Collections.Generic;
using System.Numerics;

public interface IPathFinder
{
    List<HexCell> FindPath(HexCell from, HexCell to);
}